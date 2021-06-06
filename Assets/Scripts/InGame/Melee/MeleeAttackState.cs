/// State enum for the Melee Attack action
public enum MeleeAttackState
{
    /// Not attacking, no animation (another script may be doing another action)
    Idle,
    /// Attacking, cannot cancel yet
    Attacking,
    /// Attacking and finishing the animation, can cancel any time
    AttackingCanCancel
}
