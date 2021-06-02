using System.Collections.Generic;
using System.Linq;
using UnityConstants;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.SceneTemplate;
using UnityEngine.SceneManagement;
using UnityEngine;

using UnityToolbag;

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
            var rootGameObjects = new List<GameObject>();
            scene.GetRootGameObjects(rootGameObjects);
            GameObject firstRoot = rootGameObjects[0];
            GameObject managerRoot = rootGameObjects[1];
            
            var levelIdentifier = firstRoot.GetComponent<LevelIdentifier>();
            var inGameManager = managerRoot.GetComponentInChildren<InGameManager>();

            if (levelIdentifier == null)
            {
                Debug.LogErrorFormat(sceneTemplateAsset.templateScene, "{0}'s template scene {1}'s first root {2} has no LevelIdentifier component.",
                    sceneTemplateAsset, sceneTemplateAsset.templateScene, firstRoot);
                return;
            }
            
            LevelData templateLevelData = levelIdentifier.levelData;
            
            if (templateLevelData == null)
            {
                Debug.LogErrorFormat(levelIdentifier, "{0} has no Level Data set.", levelIdentifier);
                return;
            }
            
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
                newLevelData.sceneEnum = templateLevelData.sceneEnum + newLevelIndex - templateLevelData.levelIndex;
                
                // Copy asset labels from original level data, as CopyAsset, like Duplicate in editor, doesn't do it
                AssetDatabase.SetLabels(newLevelData, AssetDatabase.GetLabels(templateLevelData));

                // Add level data to list
                List<LevelData> newLevelDataList = levelDataList.levelDataArray.ToList();
                newLevelDataList.Add(newLevelData);
                levelDataList.levelDataArray = newLevelDataList.ToArray();
                
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
                
                // Finally, regenerate Unity constants so the new scene gets its enum and Level Data's Scene Enum
                // shows something
                UnityConstantsGenerator.Generate();
            }
        }
        else
        {
            Debug.LogErrorFormat(sceneTemplateAsset.templateScene, "{0}'s template scene {1} has only {2} roots.",
                sceneTemplateAsset, sceneTemplateAsset.templateScene, scene.rootCount);
        }
    }
}
