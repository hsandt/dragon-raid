using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.SceneTemplate;
using UnityEngine.SceneManagement;
using UnityEngine;

using UnityToolbag;

/// Scene Template Pipeline for a level-based game
/// We follow convention to put levels in: Assets/Scenes
/// named "Level_00.unity", "Level_01.unity", etc.
public class LevelSceneTemplatePipeline : ISceneTemplatePipeline
{
    public virtual bool IsValidTemplateForInstantiation(SceneTemplateAsset sceneTemplateAsset)
    {
        return true;
    }

    public virtual void BeforeTemplateInstantiation(SceneTemplateAsset sceneTemplateAsset, bool isAdditive, string sceneName)
    {

    }

    public virtual void AfterTemplateInstantiation(SceneTemplateAsset sceneTemplateAsset, Scene scene, bool isAdditive, string sceneName)
    {
        if (scene.rootCount > 1)
        {
            // In theory, we should get objects by using:
            // var rootGameObjects = new List<GameObject>();
            // scene.GetRootGameObjects(rootGameObjects);
            // GameObject go = rootGameObjects.Find(/*predicate*/);

            // but in practice, the scene has been loaded, so GameObject.Find methods just work fine without
            // having go through `scene`.

            Constants constants = ConstantsManager.GetOrCreateConstants();
            GameObject levelIdentifierGameObject = GameObject.FindWithTag(constants.Tags.LevelIdentifier);
            // alternative:
            // GameObject levelIdentifierGameObject = rootGameObjects.Find(gameObject => gameObject.tag == Tags.LevelIdentifier);

            if (levelIdentifierGameObject == null)
            {
                Debug.LogErrorFormat(sceneTemplateAsset.templateScene, "{0}'s template scene {1}'s has no game object tagged LevelIdentifier.",
                    sceneTemplateAsset, sceneTemplateAsset.templateScene);
                return;
            }

            var levelIdentifier = levelIdentifierGameObject.GetComponent<LevelIdentifier>();

            if (levelIdentifier == null)
            {
                Debug.LogErrorFormat(levelIdentifierGameObject, "{0} has no LevelIdentifier component.",
                    levelIdentifierGameObject);
                return;
            }

            LevelData templateLevelData = levelIdentifier.levelData;

            if (templateLevelData == null)
            {
                Debug.LogErrorFormat(levelIdentifier, "{0} has no Level Data set.", levelIdentifier);
                return;
            }

            GameObject managerRoot = GameObject.Find("_Managers");
            // alternative:
            // GameObject managerRoot = rootGameObjects.Find(gameObject => gameObject.name == "_Managers");

            if (managerRoot == null)
            {
                Debug.LogErrorFormat(sceneTemplateAsset.templateScene, "{0}'s template scene {1}'s has no game object named _Managers.",
                    sceneTemplateAsset, sceneTemplateAsset.templateScene);
                return;
            }

            var inGameManager = managerRoot.GetComponentInChildren<InGameManager>();

            if (inGameManager == null)
            {
                Debug.LogErrorFormat(sceneTemplateAsset.templateScene, "{0}'s template scene {1}'s second root {2} has no InGameManager component in children.",
                    sceneTemplateAsset, sceneTemplateAsset.templateScene, managerRoot);
                return;
            }

            LevelDataList levelDataList = inGameManager.levelDataList;

            if (levelDataList == null)
            {
                Debug.LogErrorFormat(inGameManager, "{0} has no Level Data List set.", inGameManager);
                return;
            }

            // Ex: 2 if last level was 1
            int newLevelIndex = levelDataList.levelDataArray.Length;
            string newScenePath = $"Assets/Scenes/Level_{newLevelIndex:00}.unity";

            var existingMainAssetType = AssetDatabase.GetMainAssetTypeAtPath(newScenePath);
            if (existingMainAssetType != null)
            {
                Debug.LogErrorFormat("Asset already exists at {0}, cannot save new Scene there.", newScenePath);
                return;
            }

            // Should be Assets/Data/Levels/LevelData_00
            string levelDataAssetPath = AssetDatabase.GetAssetPath(templateLevelData);
            // Ex: Assets/Data/Levels/LevelData_02
            string newLevelDataAssetPath = levelDataAssetPath.Replace("00", $"{newLevelIndex:00}");
            existingMainAssetType = AssetDatabase.GetMainAssetTypeAtPath(newLevelDataAssetPath);
            if (existingMainAssetType != null)
            {
                Debug.LogErrorFormat("Asset already exists at {0}, cannot create new Level Data there.", newLevelDataAssetPath);
                return;
            }

            bool success = AssetDatabase.CopyAsset(levelDataAssetPath, newLevelDataAssetPath);
            if (success)
            {
                // Increment level index and scene enum
                // Note that scene enum will be out of range just for a moment, until we actually save this new scene
                // and add it to EditorBuildSettings, then regenerate UnityConstants
                LevelData newLevelData = AssetDatabase.LoadAssetAtPath<LevelData>(newLevelDataAssetPath);
                newLevelData.levelIndex = newLevelIndex;
                // There may be scenes before the first level scene in editor build settings, such as the Title scene
                // So we need to offset the scene enum to match original scene enum (level 0) + index delta since level 0
                newLevelData.sceneIndex = templateLevelData.sceneIndex + newLevelIndex - templateLevelData.levelIndex;

                // Copy asset labels from original level data, as CopyAsset, like Duplicate in editor, doesn't do it
                AssetDatabase.SetLabels(newLevelData, AssetDatabase.GetLabels(templateLevelData));

                // Add level data to list
                List<LevelData> newLevelDataList = levelDataList.levelDataArray.ToList();
                newLevelDataList.Add(newLevelData);
                levelDataList.levelDataArray = newLevelDataList.ToArray();

                // Set it dirty to make sure it's saved on SaveAssets
                EditorUtility.SetDirty(levelDataList);

                // Replace name level data on level identifier with new level index and data
                levelIdentifier.name = levelIdentifier.name.Replace("00", $"{newLevelIndex:00}");
                levelIdentifier.levelData = newLevelData;

                // Save scene
                EditorSceneManager.SaveScene(scene, newScenePath);

                // Now we can add this scene to Editor Build Settings using that path
                List<EditorBuildSettingsScene> newBuildSettingsScenes = EditorBuildSettings.scenes.ToList();
                EditorBuildSettingsScene newBuildSettingsScene = new EditorBuildSettingsScene(newScenePath, true);
                newBuildSettingsScenes.Add(newBuildSettingsScene);
                EditorBuildSettings.scenes = newBuildSettingsScenes.ToArray();

                // Regenerate Unity constants so the new scene gets its enum and Level Data's Scene Enum
                // shows something
                UnityConstantsGenerator.Generate();

                // Finally, save all assets, esp. EditorBuildSettings which contains the new scene,
                // and the Level Data List which contain the new Level Data
                AssetDatabase.SaveAssets();
            }
        }
        else
        {
            Debug.LogErrorFormat(sceneTemplateAsset.templateScene, "{0}'s template scene {1} has only {2} roots.",
                sceneTemplateAsset, sceneTemplateAsset.templateScene, scene.rootCount);
        }
    }
}
