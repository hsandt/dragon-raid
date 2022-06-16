using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using UnityConstants;
using CommonsHelper;

[CustomEditor(typeof(EnemyWave))]
public class EnemyWaveEditor : Editor
{
    /* Parameters */

    /// Position snapping distance. Set to 1px for pixel perfect placement.
    private const float autoSnapValue = 1f / 16f;

    /// Position snapping distance when holding Ctrl. Bigger than auto-snap, used for broad placement.
    private const float manualSnapValue = 1f;

    /// Color used for spawn point debug and enemy label
    private readonly Color spawnPointColor = new Color(0.78f, 0.21f, 0.42f);

    /// Color used for spawn point debug and enemy label for chain spawn
    private readonly Color chainSpawnPointColor = new Color(0.62f, 0.05f, 0.78f);

    /// Color used for anchor spawn point debug and enemy label for squad spawn
    private readonly Color squadAnchorSpawnPointColor = new Color(0.12f, 0.78f, 0.05f);

    /// Color used for unit spawn point debug and enemy label for squad spawn
    private readonly Color squadUnitSpawnPointColor = new Color(0.05f, 0.56f, 0.78f);

    /// Color used for the rectangle that represents the camera at time of event trigger
    private readonly Color triggerCameraColor = new Color(0.78f, 0.66f, 0.23f);


    /* Cached scene references */

    /// Cached camera start position reference
    private Transform m_CameraStartTransform;


    private void CacheSceneReferences()
    {
        // We cache scene references on window open, but also on scene change.
        // In addition, if a tagged object found previously has been destroyed, we will search for a tagged object
        // one last time and return early if we still find nothing. So we are safe against missing object null references.
        // However, if you retag objects after opening this window, this will not be detected, and our references may
        // be outdated (e.g. using the wrong transform). This should be a rare case, so just reopen the window,
        // or make sure that the previously tagged object was destroyed, to force cache reference refresh.

        m_CameraStartTransform = GameObject.FindWithTag(Tags.CameraStartPosition)?.transform;
        if (m_CameraStartTransform == null)
        {
            Debug.LogError("[LevelEditor] Could not find Game Object tagged CameraStartPosition");
        }
    }

    private void OnSceneGUI()
    {
        var script = (EnemyWave) target;

        // Scale offset with handle size so it remains constant on screen
        float handleSize = HandlesUtil.Get2DPixelSize();

        // We're modifying indirect (deep) members of the script, and besides applying custom rounding,
        // so changes are not trivial and need Undo Record Object
        Undo.RecordObject(script, "Changed Enemy Wave Data");

        DrawSpatialProgressCameraBounds(script);
        DrawEnemySpawnHandles(script, handleSize);
        DrawChainSpawnHandles(script, handleSize);
        DrawSquadSpawnHandles(script, handleSize);
    }

    private void DrawSpatialProgressCameraBounds(EnemyWave script)
    {
        if (m_CameraStartTransform == null)
        {
            CacheSceneReferences();

            if (m_CameraStartTransform == null)
            {
                return;
            }
        }

        var eventTriggerSpatialProgress = script.GetComponent<EventTrigger_SpatialProgress>();
        Camera camera = Camera.main;

        if (eventTriggerSpatialProgress != null && camera != null)
        {
            // Get camera view dimensions
            float cameraHalfHeight = camera.orthographicSize;
            float cameraHalfWidth = camera.aspect * cameraHalfHeight;
            Vector2 cameraHalfExtent = new Vector2(cameraHalfWidth, cameraHalfHeight);

            // Only support scrolling to the right for now
            Vector2 triggerCameraPosition = (Vector2) m_CameraStartTransform.position + eventTriggerSpatialProgress.RequiredSpatialProgress * Vector2.right;

            // Draw rectangle representing camera at the time of event trigger
            Rect rect = new Rect( triggerCameraPosition - cameraHalfExtent, 2f * cameraHalfExtent);
            HandlesUtil.DrawRect(rect, triggerCameraColor);
        }
    }

    private static Vector2 ComputeBaseIconPosition(Vector2 spawnPosition, Texture previewTexture, float handleSize)
    {
        return new Vector2(spawnPosition.x - (previewTexture.width / 2f) * handleSize,
            spawnPosition.y - (15f + previewTexture.height / 2f) * handleSize);
    }

    private static Vector3 ComputeLabelPosition(Vector2 spawnPosition, float handleSize)
    {
        return new Vector3(spawnPosition.x - 24f * handleSize,
            spawnPosition.y - 32f * handleSize,
            0f);
    }

    private void DrawEnemySpawnHandles(EnemyWave script, float handleSize)
    {
        // In order to draw the batch handle (that moves all spawn points at the same time), we must determine
        // the minimum bounding box containing all the spawn points
        Rect boundingBox = new Rect
        {
            min = Vector2.positiveInfinity,
            max = Vector2.negativeInfinity
        };

        foreach (EnemySpawnData enemySpawnData in script.EnemySpawnDataArray)
        {
            Vector2 spawnPosition = enemySpawnData.spawnPosition;

            DrawSpawnPositionHandle(ref enemySpawnData.spawnPosition, spawnPointColor);

            // Expand bounding box to contain any (rounded) spawn point not inside it already
            // Note that we are following 2D space convention, not UI convention, so +Y is up
            boundingBox.Expand(spawnPosition);

            // Draw preview texture first, so in case it's too big, the enemy name label will be on top
            if (enemySpawnData.enemyData != null)
            {
                Texture previewTexture = enemySpawnData.enemyData.editorSpawnPreviewTexture;
                if (previewTexture != null)
                {
                    Vector2 iconPosition = ComputeBaseIconPosition(spawnPosition, previewTexture, handleSize);
                    Handles.Label(iconPosition, previewTexture);
                }
            }

            string enemyName = enemySpawnData.enemyData ? enemySpawnData.enemyData.enemyName : "NONE";
            HandlesUtil.Label2D(ComputeLabelPosition(spawnPosition, handleSize), enemyName, 2f,
                true, spawnPointColor);
        }

        DrawBatchMoveHandle(handleSize, boundingBox, script.EnemySpawnDataArray, spawnPointColor);
    }

    private void DrawChainSpawnHandles(EnemyWave script, float handleSize)
    {
        Rect chainSpawnBoundingBox = new Rect
        {
            min = Vector2.positiveInfinity,
            max = Vector2.negativeInfinity
        };

        foreach (EnemyChainSpawnData enemyChainSpawnData in script.EnemyChainSpawnDataArray)
        {
            Vector2 spawnPosition = enemyChainSpawnData.spawnPosition;

            DrawSpawnPositionHandle(ref enemyChainSpawnData.spawnPosition, chainSpawnPointColor);

            // Expand bounding box to contain any (rounded) spawn point not inside it already
            // Note that we are following 2D space convention, not UI convention, so +Y is up
            chainSpawnBoundingBox.Expand(spawnPosition);

            // Draw preview texture first, so in case it's too big, the enemy name label will be on top
            if (enemyChainSpawnData.enemyData != null)
            {
                Texture previewTexture = enemyChainSpawnData.enemyData.editorSpawnPreviewTexture;
                if (previewTexture != null)
                {
                    Vector2 baseIconPosition = ComputeBaseIconPosition(spawnPosition, previewTexture, handleSize);

                    // Draw as many icons as enemies spawned, aligned horizontally and centered around
                    // the base icon position
                    for (int i = 0; i < enemyChainSpawnData.spawnCount; i++)
                    {
                        // Mind the 2f to ensure float division
                        Vector2 iconPosition = baseIconPosition + (i - (enemyChainSpawnData.spawnCount - 1) / 2f) * 15f *
                            handleSize * Vector2.right;
                        Handles.Label(iconPosition, previewTexture);
                    }
                }
            }

            string enemyName = enemyChainSpawnData.enemyData
                ? $"{enemyChainSpawnData.enemyData.enemyName} ({enemyChainSpawnData.spawnCount})"
                : "NONE";
            HandlesUtil.Label2D(ComputeLabelPosition(spawnPosition, handleSize), enemyName, 2f,
                true, chainSpawnPointColor);
        }

        DrawBatchMoveHandle(handleSize, chainSpawnBoundingBox, script.EnemyChainSpawnDataArray, chainSpawnPointColor);
    }

    private void DrawSquadSpawnHandles(EnemyWave script, float handleSize)
    {
        Rect squadAnchorSpawnBoundingBox = new Rect
        {
            min = Vector2.positiveInfinity,
            max = Vector2.negativeInfinity
        };

        foreach (EnemySquadSpawnData enemySquadSpawnData in script.EnemySquadSpawnDataArray)
        {
            Vector2 anchorSpawnPosition = enemySquadSpawnData.spawnPosition;

            // Draw handle for anchor spawn position
            DrawSpawnPositionHandle(ref enemySquadSpawnData.spawnPosition, squadAnchorSpawnPointColor);

            // Expand bounding box to contain any (rounded) anchor spawn point not inside it already
            // Note that we are following 2D space convention, not UI convention, so +Y is up
            squadAnchorSpawnBoundingBox.Expand(anchorSpawnPosition);

            // Draw preview texture first, so in case it's too big, the enemy name label will be on top
            if (enemySquadSpawnData.enemyData != null)
            {
                Texture previewTexture = enemySquadSpawnData.enemyData.editorSpawnPreviewTexture;

                // Draw handle and spawn icon for each formation offset
                Vector2[] formationOffsets = enemySquadSpawnData.formationOffsets;
                for (int index = 0; index < formationOffsets.Length; index++)
                {
                    Vector2 formationOffset = formationOffsets[index];

                    Vector2 squadUnitSpawnPosition = anchorSpawnPosition + formationOffset;
                    DrawSpawnPositionHandle(ref squadUnitSpawnPosition, squadUnitSpawnPointColor);
                    formationOffsets[index] = squadUnitSpawnPosition - anchorSpawnPosition;

                    if (previewTexture != null)
                    {
                        Vector2 baseIconPosition = ComputeBaseIconPosition(anchorSpawnPosition, previewTexture, handleSize);
                        Vector2 iconPosition = baseIconPosition + formationOffset;
                        Handles.Label(iconPosition, previewTexture);
                    }
                }
            }

            string enemyName = enemySquadSpawnData.enemyData
                ? $"{enemySquadSpawnData.enemyData.enemyName} ({enemySquadSpawnData.SpawnCount})"
                : "NONE";
            HandlesUtil.Label2D(ComputeLabelPosition(anchorSpawnPosition, handleSize), enemyName, 2f,
                true, squadAnchorSpawnPointColor);
        }

        DrawBatchMoveHandle(handleSize, squadAnchorSpawnBoundingBox, script.EnemySquadSpawnDataArray, squadAnchorSpawnPointColor);
    }

    private void DrawSpawnPositionHandle(ref Vector2 spawnPosition, Color color)
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            HandlesUtil.DrawSlider2D(ref spawnPosition, color,
                manualSnapValue * Vector2.one, HandlesUtil.CrossedCircleHandleCap, 2f);

            if (check.changed)
            {
                spawnPosition = VectorUtil.RoundVector2(spawnPosition, autoSnapValue);
            }
        }
    }

    private void DrawBatchMoveHandle<TSpawnData>(float handleSize, Rect chainSpawnBoundingBox, IEnumerable<TSpawnData> spawnDataArray, Color color) where TSpawnData : EnemyBaseSpawnData
    {
        // Ignore infinite bounding box (only happens when EnemySpawnDataArray is empty)
        if (!float.IsInfinity(chainSpawnBoundingBox.width) && !float.IsInfinity(chainSpawnBoundingBox.height))
        {
            // Place batch move handle above the bounding box (remember 2D convention is +Y up)
            // Since we are using 2D space coordinates, we don't need fixed screen scaling, so no need to * handleSize
            Vector2 batchMoveHandleStartPosition =
                new Vector2(chainSpawnBoundingBox.center.x, chainSpawnBoundingBox.yMax + 1f);
            Vector2 batchMoveHandleCurrentPosition = batchMoveHandleStartPosition;

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                HandlesUtil.DrawSlider2D(ref batchMoveHandleCurrentPosition, color,
                    manualSnapValue * Vector2.one, Handles.RectangleHandleCap, 2f);

                if (check.changed)
                {
                    // Calculate handle move
                    Vector2 handleDelta = batchMoveHandleCurrentPosition - batchMoveHandleStartPosition;

                    // Move all spawn points the same way we moved the batch handle
                    foreach (TSpawnData enemyChainSpawnData in spawnDataArray)
                    {
                        enemyChainSpawnData.spawnPosition =
                            VectorUtil.RoundVector2(enemyChainSpawnData.spawnPosition + handleDelta, autoSnapValue);
                    }
                }
            }

            HandlesUtil.Label2D(
                new Vector3(batchMoveHandleCurrentPosition.x - 63f * handleSize,
                    batchMoveHandleCurrentPosition.y + 52f * handleSize, 0f), "Batch move", 2f, true, color);
        }
    }
}
