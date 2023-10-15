using System;
using Sirenix.OdinInspector;

namespace UFlow.Addon.ECS.Core.Runtime {
    [Serializable]
    [HideLabel]
    public struct SceneEntityRef {
        [LabelText("@$property.Parent.NiceName")] 
        [SuffixLabel("@this.IsPlaying ? this.EntityName : string.Empty")]
        public SceneEntity sceneEntity;
        
        public Entity Entity => sceneEntity == null ? default : sceneEntity.Entity;
        private bool IsPlaying => sceneEntity != null && sceneEntity.IsPlaying;
        private string EntityName => Entity.ToString();
    }
}