using System;
using Sirenix.OdinInspector;

namespace UFlow.Addon.ECS.Core.Runtime {
    [Serializable]
    [HideLabel]
    public struct SceneEntityLink {
#if UNITY_EDITOR
        [LabelText("@$property.Parent.NiceName")] 
        [SuffixLabel("@" + nameof(IsPlaying) + " ? " + nameof(EntityName) + " : string.Empty")]
        [Required]
#endif
        public SceneEntity sceneEntity;
        
        public Entity Entity => sceneEntity == null ? default : sceneEntity.Entity;
#if UNITY_EDITOR
        private bool IsPlaying => sceneEntity != null && sceneEntity.IsPlaying;
        private string EntityName => Entity.ToString();
#endif
            
        public static implicit operator Entity(SceneEntityLink link) => link.Entity;
    }
}