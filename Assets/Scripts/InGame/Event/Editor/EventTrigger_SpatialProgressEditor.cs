using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using UnityConstants;

[CustomEditor(typeof(EventTrigger_SpatialProgress))]
public class EventTrigger_SpatialProgressEditor : Editor
{
    /* Cached scene references */

    /// Cached camera start position reference
    private Transform m_CameraStartTransform;

    public override void OnInspectorGUI()
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            // This basically shows requiredSpatialProgress, which is the field we want to track for changes
            base.OnInspectorGUI();

            if (check.changed)
            {
                MoveGameObjectToSpatialProgressFrontLine();
            }
        }
    }

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

    private void MoveGameObjectToSpatialProgressFrontLine()
    {
        if (m_CameraStartTransform == null)
        {
            CacheSceneReferences();

            if (m_CameraStartTransform == null)
            {
                return;
            }
        }

        Camera camera = Camera.main;
        if (camera != null)
        {
            // Retrieve target script and transform
            var script = (EventTrigger_SpatialProgress)target;
            Transform spatialEventTransform = script.transform;

            // Get camera view dimensions
            float cameraHalfHeight = camera.orthographicSize;
            float cameraHalfWidth = camera.aspect * cameraHalfHeight;

            // Get center position where camera will be when the spatial event is triggered
            // Only support scrolling to the right for now, so just add required spatial progress
            float triggerCameraPositionX = m_CameraStartTransform.position.x + script.RequiredSpatialProgress;
            float triggerCameraFrontline = triggerCameraPositionX + cameraHalfWidth;

            // Move game object to frontline so its label can be seen around where spatial event will trigger
            Undo.RecordObject(spatialEventTransform, "Move spatial event to match spatial progress");
            Vector3 newPosition = spatialEventTransform.position;
            newPosition.x = triggerCameraFrontline;
            spatialEventTransform.position = newPosition;
        }
    }
}
