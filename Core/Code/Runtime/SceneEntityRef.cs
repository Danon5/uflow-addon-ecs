using System;
using Sirenix.OdinInspector;

namespace UFlow.Addon.ECS.Core.Runtime {
    [Serializable]
    [HideLabel]
    public struct SceneEntityRef {
#if UNITY_EDITOR
        [LabelText("@$property.Parent.NiceName")] 
        [SuffixLabel("@" + nameof(IsPlaying) + " ? " + nameof(EntityName) + " : string.Empty")]
#endif
        public SceneEntity sceneEntity;
        
        public Entity Entity => sceneEntity == null ? default : sceneEntity.Entity;
#if UNITY_EDITOR
        private bool IsPlaying => sceneEntity != null && sceneEntity.IsPlaying;
        private string EntityName => Entity.ToString();
#endif
    }
}