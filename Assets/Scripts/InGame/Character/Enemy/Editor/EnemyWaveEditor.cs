using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

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
    
    /// Color used for the handle that allows to batch move all spawn points at the same time
    private readonly Color batchHandleColor = new Color(0.78f, 0.39f, 0.26f);
    

    private void OnSceneGUI()
    {
        var script = (EnemyWave) target;
        
        // Scale offset with handle size so it remains constant on screen
        float handleSize = HandlesUtil.Get2DPixelSize();

        // In order to draw the batch handle (that moves all spawn points at the same time), we must determine
        // the minimum bounding box containing all the spawn points
        Rect boundingBox = new Rect
        {
            min = Vector2.positiveInfinity,
            max = Vector2.negativeInfinity
        };
        
        // we're modifying indirect (deep) members of the script, and besides applying custom rounding,
        // so changes are not trivial and need Undo Record Object
        Undo.RecordObject(script, "Changed Enemy Wave Data");
      
        foreach (EnemySpawnData enemySpawnData in script.EnemySpawnDataArray)
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                HandlesUtil.DrawFreeMoveHandle(ref enemySpawnData.spawnPosition, spawnPointColor, manualSnapValue * Vector2.one, HandlesUtil.CrossedCircleHandleCap, 2f);

                if (check.changed)
                {
                    enemySpawnData.spawnPosition = VectorUtil.RoundVector2(enemySpawnData.spawnPosition, autoSnapValue);
                }
            }
            
            // Expand bounding box to contain any (rounded) spawn point not inside it already
            // Note that we are following 2D space convention, not UI convention, so +Y is up
            boundingBox.min = Vector2.Min(boundingBox.min, enemySpawnData.spawnPosition);
            boundingBox.max = Vector2.Max(boundingBox.max, enemySpawnData.spawnPosition);
            
            // Draw preview texture first, so in case it's too big, the enemy name label will be on top
            Texture previewTexture = enemySpawnData.enemyData.editorSpawnPreviewTexture;
            if (previewTexture != null)
            {
                Vector3 labelPosition = new Vector3(enemySpawnData.spawnPosition.x - (previewTexture.width / 2f) * handleSize, enemySpawnData.spawnPosition.y - (15f + previewTexture.height / 2f) * handleSize, 0f);
                Handles.Label(labelPosition, previewTexture);
            }
            
            string enemyName = enemySpawnData.enemyData ? enemySpawnData.enemyData.enemyName : "NONE";
            HandlesUtil.Label2D(new Vector3(enemySpawnData.spawnPosition.x - 24f * handleSize, enemySpawnData.spawnPosition.y - 32f * handleSize, 0f), enemyName, 2f, true, spawnPointColor);
        }

        // Ignore infinite bounding box (only happens when EnemySpawnDataArray is empty)
        if (!float.IsInfinity(boundingBox.width) && !float.IsInfinity(boundingBox.height))
        {
            // Place batch move handle above the bounding box (remember 2D convention is +Y up)
            // Since we are using 2D space coordinates, we don't need fixed screen scaling, so no need to * handleSize
            Vector2 batchMoveHandleStartPosition = new Vector2(boundingBox.center.x, boundingBox.yMax + 1f);
            Vector2 batchMoveHandleCurrentPosition = batchMoveHandleStartPosition;
            
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                HandlesUtil.DrawFreeMoveHandle(ref batchMoveHandleCurrentPosition, batchHandleColor,
                    manualSnapValue * Vector2.one, Handles.RectangleHandleCap, 2f);

                if (check.changed)
                {
                    // Calculate handle move
                    Vector2 handleDelta = batchMoveHandleCurrentPosition - batchMoveHandleStartPosition;

                    // Move all spawn points the same way we moved the batch handle
                    foreach (EnemySpawnData enemySpawnData in script.EnemySpawnDataArray)
                    {
                        enemySpawnData.spawnPosition = VectorUtil.RoundVector2(enemySpawnData.spawnPosition + handleDelta, autoSnapValue);
                    }
                }
            }

            HandlesUtil.Label2D(new Vector3(batchMoveHandleCurrentPosition.x - 63f * handleSize, batchMoveHandleCurrentPosition.y + 52f * handleSize, 0f), "Batch move", 2f, true, batchHandleColor);
        }
    }
}
