using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using UnityEngine.Serialization;

/// Action to move flying character along a relative path at constant world speed (not parameter speed).
/// It supports any child class of Path 2D Component.
/// Note that this ignores the Is Relative flag on the associated Path 2D Component.
/// Instead, the movement will always start from the character position at the start of the action,
/// as long as the first point coordinate is (0, 0).
/// We still recommend to set this flag as it's more convenient to edit a curve from a visible character
/// than around the origin.
[AddComponentMenu("Game/Action: Move Along Path")]
public class Action_MoveAlongPath : BehaviourAction
{
    /// Small value (compared to 1) used to predict a point on the path in the near future,
    /// in order to estimate the natural point speed along the path for current parameter
    private const float PARAMETER_EPSILON = 0.1f;


    [Header("Parameters")]

    [Tooltip("Component containing path to follow")]
    [FormerlySerializedAs("bezierPath2DComponent")]
    public Path2DComponent path2DComponent;

    [SerializeField, Tooltip("Motion speed (m/s) of the anchor. If tangentLocalOffset is zero, this is also " +
         "the character speed.")]
    [Min(0f)]
    [FormerlySerializedAs("speed")]
    private float anchorSpeed = 4f;

#if UNITY_EDITOR
    public float AnchorSpeed { get => anchorSpeed; set => anchorSpeed = value; }
#endif

    [SerializeField, Tooltip("Offset (m) from a fictive anchor moving along the path over time, " +
         "in the local space where the tangent forward vector of the path at this anchor point is Vector2.right. " +
         "In other words, an offset of (1, 0) will place the character 1m forward along the current anchor tangent, " +
         "(0, 1) will place it 1m to the left. This allows us to have characters stay in the inner or outer curve of " +
         "a path, and create enemy squads with a natural curve motion. Note that this will affect the character speed, " +
         "which will effectively be locally faster (on outer curve) or slower (on inner curve) than the anchor moving " +
         "at base speed.")]
    private Vector2 localOffsetFromAnchor ;

#if UNITY_EDITOR
    public Vector2 LocalOffsetFromAnchor { get => localOffsetFromAnchor; set => localOffsetFromAnchor = value; }
#endif


    /* Owner sibling components */

    private MoveFlying m_MoveFlying;
    private MoveFlyingIntention m_MoveFlyingIntention;


    /* Derived parameters */

    /// Number of curves in the path
    /// We could only work with Normalized Parameter so we don't have to use this, but it's a little easier
    /// to read a non-normalized parameter in debug (1 means we finished the first curve)
    /// than a normalized one (0.333... means we finished the first curve, if there are 3 curves...).
    private float m_CurvesCount;


    /* State */

    /// Start anchor position: either spawn position or end position of the previous action
    /// Note that we will apply the local offset from anchor on top of it, so if it is not zero,
    /// character will warp quickly by that local offset. Therefore, it is not recommended to use an offset
    /// for an action played in the middle of an Action Sequence, where the character could be visible on screen
    /// and the player notice the warp. So keep local offset for spawning of enemy squads.
    private Vector2 m_StartAnchorPosition;

    /// Current anchor position: this is tracked because the character may be offset from it,
    /// but to evaluate tangent (via local position epsilon), we need to remember to anchor position of last frame.
    private Vector2 m_CurrentAnchorPosition;

    /// Accumulated scrolling motion since the start of the action, used to adjust the target position
    /// in case entity is moving relatively to screen
    private Vector2 m_AccumulatedScrolling;

    /// Current parameter on the path (between 0 and #curves, +1 for every curve completed)
    private float m_CurrentParameter;


    protected override void OnInit()
    {
        m_MoveFlying = m_EnemyCharacterMaster.GetComponentOrFail<MoveFlying>();
        m_MoveFlyingIntention = m_MoveFlying.MoveFlyingIntention;

        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.AssertFormat(path2DComponent, this,
            "[Action_MoveAlongPath] Path 2D Component not set on {0}", this);
        #endif

        // We are working with non-normalized path parameter, which evolves from 0 to #curves
        // Since we cached derived parameters on Init, do not add/remove key points while Behaviour Tree is running
        // (moving key points is okay, but may lead to high speed motions for catch-up)
        m_CurvesCount = path2DComponent.Path.GetCurvesCount();
    }

    public override void OnStart ()
    {
        // No need to calculate estimated start position with local offset from anchor and warp character there;
        // indeed, RunUpdate will set velocity appropriately, making character move super fast to the target position
        // next frame, effectively warping it, if needed.
        m_StartAnchorPosition = (Vector2) m_MoveFlyingIntention.transform.position;
        m_CurrentAnchorPosition = m_StartAnchorPosition;
        m_AccumulatedScrolling = Vector2.zero;
        m_CurrentParameter = 0f;
    }

    private Vector2 ComputeAnchorPosition(float parameter)
    {
        // There is already a method Path2DComponent.InterpolatePathByParameter that adds
        // its owner transform position as offset. However, Action nodes are often placed under
        // the moving entity itself, which means the owner position would change over time,
        // causing the path to be unstable. Therefore, we must add the stored start position instead.
        // In addition, we must add any accumulated scrolling due to moving relatively to screen,
        // so we are targeting the right position on screen (if m_MoveFlying.moveFlyingParameters.moveRelativelyToScreen
        // is false, m_AccumulatedScrolling = 0).
        return m_StartAnchorPosition + m_AccumulatedScrolling + path2DComponent.Path.InterpolatePathByParameter(parameter);
    }

    private void SetCurrentParameterAndUpdateAnchorPosition(float parameter)
    {
        // Set parameter to clamped value
        m_CurrentParameter = Mathf.Clamp(parameter, 0f, m_CurvesCount);

        // Update anchor target position for this new parameter, taking start and accumulated scrolling into account
        m_CurrentAnchorPosition = ComputeAnchorPosition(m_CurrentParameter);
    }

    public override void RunUpdate ()
    {
        /* Anchor motion */

        // To make our anchor move at uniform speed, we must take into account the parametric speed,
        // i.e. the "natural speed" of a point along the path, when the parameter increases at constant rate.
        // Mathematically, this is the norm of the derivative of the point position relative to the parameter,
        // and since no time is involved, its unit is m/1.
        // To avoid using the exact derivative formula, we just estimate the derivative by computing:
        // || local_anchor_position_epsilon || / parameter_epsilon where local_position_delta is a small segment approximately
        // along the local tangent direction, linking the current position to a position in the near future on the path,
        // parameter_epsilon (small value) units in the future.
        float nearFutureParameter = Mathf.Min(m_CurrentParameter + PARAMETER_EPSILON, m_CurvesCount);
        Vector2 nearFutureAnchorPosition = ComputeAnchorPosition(nearFutureParameter);
        Vector2 localAnchorPositionEpsilon = nearFutureAnchorPosition - m_CurrentAnchorPosition;
        float parametricSpeed = localAnchorPositionEpsilon.magnitude / PARAMETER_EPSILON;

        // IsOver avoids the case where parameter has reached the end and nearFutureParameter == m_CurrentParameter
        // causing parametricSpeed == 0f. But it may still be 0 if the path is degenerated (control points at the
        // same position), or simply very small if the parameter was very close the end (m_CurvesCount), so check this.
        if (parametricSpeed < float.Epsilon)
        {
            // Set current parameter and anchor position to the end of path, to ensure IsOver returns true next time
            // Otherwise, if this is run as part of controller node such as an Action Sequence, it may get stuck.
            SetCurrentParameterAndUpdateAnchorPosition(m_CurvesCount);
            m_MoveFlyingIntention.moveVelocity = Vector2.zero;
            return;
        }

        // We now divide the wanted anchor world speed (m/s) by the parametric speed (m/1) to get the local parameter derivative (1/s)
        // needed to have a point moving at this world speed.
        float parameterDerivative = anchorSpeed / parametricSpeed;

        // Finally, we multiply this by delta time to get the parameter increase required this frame
        float parameterIncrease = parameterDerivative * Time.deltaTime;

        // Apply the increase and clamp
        float newParameter = Mathf.Clamp(m_CurrentParameter + parameterIncrease, 0f, m_CurvesCount);

        // Set new parameter and update anchor position accordingly
        SetCurrentParameterAndUpdateAnchorPosition(newParameter);

        /* Target motion based on anchor */

        // Determine target position for actual character by adding offset to new anchor position in local anchor space,
        // i.e. rotated by the angle between Vector2.right and local tangent forward, around the anchor.
        // We computed localPositionEpsilon earlier, which is an approximation of small tangent segment, so we can
        // get the tangent forward direction and angle from right by using it.
        float tangentForwardSignedAngle = Vector2.SignedAngle(Vector2.right, localAnchorPositionEpsilon);
        Vector2 target = m_CurrentAnchorPosition + VectorUtil.Rotate(localOffsetFromAnchor, tangentForwardSignedAngle);

        // Calculate vector from current position to target and set velocity so we arrive just on target next frame.
        // We assume we have a proper path that starts at (relative) (0, 0), so the entity position is continuous,
        // and the entity stays on track (cannot be pushed away by another entity), so we will never set a crazy speed
        // just to catch up a target position far away.
        // If this proves too unstable, prefer adding an alternative motion mode which takes a target position directly
        // and Rigidbody2D.MovePosition the entity to this target.
        Vector2 currentPosition = (Vector2) m_MoveFlyingIntention.transform.position;
        Vector2 toTarget = target - currentPosition;
        Vector2 nextVelocity = toTarget / Time.deltaTime;

        m_MoveFlyingIntention.moveVelocity = nextVelocity;

        // Since MoveFlying with moveRelativelyToScreen: true adds scrolling speed to velocity each frame
        // to make the character move with the screen, we must take this into account in the final target position,
        // else the character will deviate as if following the world, not screen (see InterpolatePathByParameterWithOffset).
        if (m_MoveFlying.moveFlyingParameters.moveRelativelyToScreen)
        {
            m_AccumulatedScrolling += ScrollingManager.Instance.ScrollingSpeed * Vector2.right * Time.deltaTime;
        }
    }

    protected override bool IsOver()
    {
        return m_CurrentParameter >= (float) m_CurvesCount;
    }

    public override void OnEnd()
    {
        m_MoveFlyingIntention.moveVelocity = Vector2.zero;
    }

    #if UNITY_EDITOR
    public override string GetNodeName()
    {
        return $"Move Along Path at {anchorSpeed} m/s";
    }
    #endif
}
