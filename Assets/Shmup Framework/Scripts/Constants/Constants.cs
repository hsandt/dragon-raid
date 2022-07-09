using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using CommonsHelper;

/// Data that maps Shmup Framework constants (tags and layers) to actual project values
/// It replaces Unity Constants when we are working on non-project-specific code such as framework code,
/// where we cannot access the generated UnityConstants.cs, which is project-specific.
/// Create one asset at path indicated by ConstantsManager.constantsPathInResources
/// (should be Resources/Shmup Framework/Constants) to let the ConstantsManager script access it via Resources.Load.
[CreateAssetMenu(fileName = "Constants", menuName = "Shmup Framework/Constants", order = 1)]
public class Constants : ScriptableObject
{
    #region EmbeddedClasses

    /// Class storing tags required by the Shmup Framework
    /// Each member follows a convention by using a default string value that matches the member value.
    /// However, projects that use different tags can change them by mapping the members to different values.
    /// Thanks to the TagField attribute, invalid tags will be shown in red, and a dropdown will allow developer
    /// to select a valid one.
    [Serializable]
    public class TagsMapping
    {
        [Tooltip("Tag used to find Projectile Pool")]
        [TagField]
        public string ProjectilePool = "ProjectilePool";

        [Tooltip("Tag used to find FX Pool")]
        [TagField]
        public string FXPool = "FXPool";

        [Tooltip("Tag used to find PlayerSpawnPosition")]
        [TagField]
        public string PlayerSpawnPosition = "PlayerSpawnPosition";

        [Tooltip("Tag used to find LevelIdentifier")]
        [TagField]
        public string LevelIdentifier = "LevelIdentifier";

        [Tooltip("Tag used to find SpatialEvents")]
        [TagField]
        public string SpatialEvents = "SpatialEvents";

        [Tooltip("Tag used to find EnemyWaves")]
        [TagField]
        public string EnemyWaves = "EnemyWaves";

        [Tooltip("Tag used to find CanvasTitleMenu")]
        [TagField]
        public string CanvasTitleMenu = "CanvasTitleMenu";

        [Tooltip("Tag used to find CanvasPauseMenu")]
        [TagField]
        public string CanvasPauseMenu = "CanvasPauseMenu";

        [Tooltip("Tag used to find Background")]
        [TagField]
        public string Background = "Background";

        [Tooltip("Tag used to find CanvasLevel")]
        [TagField]
        public string CanvasLevel = "CanvasLevel";

        [Tooltip("Tag used to find CameraStartPosition")]
        [TagField]
        public string CameraStartPosition = "CameraStartPosition";

        [Tooltip("Tag used to find PickUpPool")]
        [TagField]
        public string PickUpPool = "PickUpPool";

        [Tooltip("Tag used to find PlayerCharacterPool")]
        [TagField]
        public string PlayerCharacterPool = "PlayerCharacterPool";

        [Tooltip("Tag used to find EnemyPool")]
        [TagField]
        public string EnemyPool = "EnemyPool";
    }

    /// Class storing layers required by the Shmup Framework
    /// We recommend following the convention of defining layers named like the members, so that the default values
    /// (which are defined in constructor, not in front of members)
    /// based on LayerMask.NameToLayer are immediately filled with matching layer indices on Constants asset creation.
    /// However, projects may use different layer names (or they may have not been defined at time of asset creation).
    /// In this case, NameToLayer will return -1 and developer will have to fill the layer themselves later.
    /// Thanks to the LayerField attribute, they will be able to select a valid layer via a dropdown.
    [Serializable]
    public class LayersMapping
    {
        [Tooltip("Layer containing PlayerCharacterHurtBox")]
        [LayerField]
        public int PlayerCharacterHurtBox;

        [Tooltip("Layer containing PlayerProjectileIntangible")]
        [LayerField]
        public int PlayerProjectileIntangible;

        [Tooltip("Layer containing PlayerMeleeHitBox")]
        [LayerField]
        public int PlayerMeleeHitBox;

        [Tooltip("Layer containing EnemyHurtBox")]
        [LayerField]
        public int EnemyHurtBox;

        [Tooltip("Layer containing EnemyProjectileIntangible")]
        [LayerField]
        public int EnemyProjectileIntangible;

        [Tooltip("Layer containing EnemyBodyHitBox")]
        [LayerField]
        public int EnemyBodyHitBox;

        [Tooltip("Layer containing EnemyMeleeHitBox")]
        [LayerField]
        public int EnemyMeleeHitBox;

        [Tooltip("Layer containing EnemyProjectileTangible")]
        [LayerField]
        public int EnemyProjectileTangible;

        [Tooltip("Layer containing PlayerProjectileTangible")]
        [LayerField]
        public int PlayerProjectileTangible;

        [Tooltip("Layer containing PlayerCharacterMoveBox")]
        [LayerField]
        public int PlayerCharacterMoveBox;

        [Tooltip("Layer containing EnemyMoveBox")]
        [LayerField]
        public int EnemyMoveBox;

        [Tooltip("Layer containing InvisibleWall")]
        [LayerField]
        public int InvisibleWall;

        [Tooltip("Layer containing PickUpIntangible")]
        [LayerField]
        public int PickUpIntangible;

        [Tooltip("Layer containing PickUpTangible")]
        [LayerField]
        public int PickUpTangible;

        [Tooltip("Layer containing LivingZone")]
        [LayerField]
        public int LivingZone;

        [Tooltip("Layer containing SolidEnvironment")]
        [LayerField]
        public int SolidEnvironment;

        [Tooltip("Layer containing DamagingEnvironment")]
        [LayerField]
        public int DamagingEnvironment;

        [Tooltip("Layer containing EnvironmentProp")]
        [LayerField]
        public int EnvironmentProp;

        [Tooltip("Layer containing Background")]
        [LayerField]
        public int Background;

        [Tooltip("Layer containing Camera")]
        [LayerField]
        public int Camera;

        // If you add a new layer here, make sure to add corresponding mask getter below,
        // and default value in Constants constructor.

        /// Bitmask of layer 'PlayerCharacterHurtBox'.
        public int PlayerCharacterHurtBoxMask => 1 << PlayerCharacterHurtBox;

        /// Bitmask of layer 'PlayerProjectileIntangible'.
        public int PlayerProjectileIntangibleMask => 1 << PlayerProjectileIntangible;

        /// Bitmask of layer 'PlayerMeleeHitBox'.
        public int PlayerMeleeHitBoxMask => 1 << PlayerMeleeHitBox;

        /// Bitmask of layer 'EnemyHurtBox'.
        public int EnemyHurtBoxMask => 1 << EnemyHurtBox;

        /// Bitmask of layer 'EnemyProjectileIntangible'.
        public int EnemyProjectileIntangibleMask => 1 << EnemyProjectileIntangible;

        /// Bitmask of layer 'EnemyBodyHitBox'.
        public int EnemyBodyHitBoxMask => 1 << EnemyBodyHitBox;

        /// Bitmask of layer 'EnemyMeleeHitBox'.
        public int EnemyMeleeHitBoxMask => 1 << EnemyMeleeHitBox;

        /// Bitmask of layer 'EnemyProjectileTangible'.
        public int EnemyProjectileTangibleMask => 1 << EnemyProjectileTangible;

        /// Bitmask of layer 'PlayerProjectileTangible'.
        public int PlayerProjectileTangibleMask => 1 << PlayerProjectileTangible;

        /// Bitmask of layer 'PlayerCharacterMoveBox'.
        public int PlayerCharacterMoveBoxMask => 1 << PlayerCharacterMoveBox;

        /// Bitmask of layer 'EnemyMoveBox'.
        public int EnemyMoveBoxMask => 1 << EnemyMoveBox;

        /// Bitmask of layer 'InvisibleWall'.
        public int InvisibleWallMask => 1 << InvisibleWall;

        /// Bitmask of layer 'PickUpIntangible'.
        public int PickUpIntangibleMask => 1 << PickUpIntangible;

        /// Bitmask of layer 'PickUpTangible'.
        public int PickUpTangibleMask => 1 << PickUpTangible;

        /// Bitmask of layer 'LivingZone'.
        public int LivingZoneMask => 1 << LivingZone;

        /// Bitmask of layer 'SolidEnvironment'.
        public int SolidEnvironmentMask => 1 << SolidEnvironment;

        /// Bitmask of layer 'DamagingEnvironment'.
        public int DamagingEnvironmentMask => 1 << DamagingEnvironment;

        /// Bitmask of layer 'EnvironmentProp'.
        public int EnvironmentPropMask => 1 << EnvironmentProp;

        /// Bitmask of layer 'Background'.
        public int BackgroundMask => 1 << Background;

        /// Bitmask of layer 'Camera'.
        public int CameraMask => 1 << Camera;
    }

    /// Class storing references to scenes required by the Shmup Framework
    /// Each member follows a convention indicated in the default value, but projects that use a different convention
    /// (scene name doesn't really matter here, but rather build scene order) can change these values in the Constants
    /// asset.
    [Serializable]
    public class ScenesMapping
    {
        // For now, we just use int, but later we can use scene references with a plugin:
        // https://github.com/JohannesMP/unity-scene-reference

        [Tooltip("ID of Title scene")]
        public int Title = 0;
    }

    #endregion

    #region MappingMembers

    public TagsMapping Tags = new();
    public LayersMapping Layers = new();
    public ScenesMapping Scenes = new();

    #endregion

    #region Reset

    #if UNITY_EDITOR

    private void Reset()
    {
        // LayerMask.NameToLayer is not supported in default member assignment (will cause error on asset creation)
        // so set default layers here.
        // To avoid log error spamming when no matching layer is found ("Layer index out of bounds" in inspector),
        // fall back to default 0 in this case (but still with a warning to notify developer they should change it
        // manually).
        Layers.PlayerCharacterHurtBox = GetLayerFromNameOrDefaultWithWarning("PlayerCharacterHurtBox");
        Layers.PlayerProjectileIntangible = GetLayerFromNameOrDefaultWithWarning("PlayerProjectileIntangible");
        Layers.PlayerMeleeHitBox = GetLayerFromNameOrDefaultWithWarning("PlayerMeleeHitBox");
        Layers.EnemyHurtBox = GetLayerFromNameOrDefaultWithWarning("EnemyHurtBox");
        Layers.EnemyProjectileIntangible = GetLayerFromNameOrDefaultWithWarning("EnemyProjectileIntangible");
        Layers.EnemyBodyHitBox = GetLayerFromNameOrDefaultWithWarning("EnemyBodyHitBox");
        Layers.EnemyMeleeHitBox = GetLayerFromNameOrDefaultWithWarning("EnemyMeleeHitBox");
        Layers.EnemyProjectileTangible = GetLayerFromNameOrDefaultWithWarning("EnemyProjectileTangible");
        Layers.PlayerProjectileTangible = GetLayerFromNameOrDefaultWithWarning("PlayerProjectileTangible");
        Layers.PlayerCharacterMoveBox = GetLayerFromNameOrDefaultWithWarning("PlayerCharacterMoveBox");
        Layers.EnemyMoveBox = GetLayerFromNameOrDefaultWithWarning("EnemyMoveBox");
        Layers.InvisibleWall = GetLayerFromNameOrDefaultWithWarning("InvisibleWall");
        Layers.PickUpIntangible = GetLayerFromNameOrDefaultWithWarning("PickUpIntangible");
        Layers.PickUpTangible = GetLayerFromNameOrDefaultWithWarning("PickUpTangible");
        Layers.LivingZone = GetLayerFromNameOrDefaultWithWarning("LivingZone");
        Layers.SolidEnvironment = GetLayerFromNameOrDefaultWithWarning("SolidEnvironment");
        Layers.DamagingEnvironment = GetLayerFromNameOrDefaultWithWarning("DamagingEnvironment");
        Layers.EnvironmentProp = GetLayerFromNameOrDefaultWithWarning("EnvironmentProp");
        Layers.Background = GetLayerFromNameOrDefaultWithWarning("Background");
        Layers.Camera = GetLayerFromNameOrDefaultWithWarning("Camera");
    }

    /// Return layer from name, or Default layer (0) if not matching layer is found
    private int GetLayerFromNameOrDefaultWithWarning(string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);

        // NameToLayer returns -1 (invalid layer) if layer cannot be found,
        // else something between 0 and 31
        if (layer >= 0)
        {
            return layer;
        }

        Debug.LogWarningFormat(this, "[Constants] GetLayerFromNameOrDefaultWithWarning: during Reset of {0}, " +
            "no layer found with name '{1}', falling back to Default (0). " +
            "Make sure to either add a layer with that name and Reset the Constants asset again, " +
            "or add an equivalent layer with a different name and manually select it in the corresponding dropdown",
            AssetDatabase.GetAssetPath(this), layerName);

        return 0;
    }

    #endif

    #endregion
}
