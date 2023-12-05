using UFlow.Core.Runtime;

namespace UFlow.Addon.ECS.Core.Runtime {
    public readonly struct PrefabSceneEntityCreatedHook : IHook {
        public readonly SceneEntity sceneEntity;

        public PrefabSceneEntityCreatedHook(in SceneEntity sceneEntity) {
            this.sceneEntity = sceneEntity;
        }
    }
}