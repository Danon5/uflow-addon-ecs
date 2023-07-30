using UFlow.Core.Runtime;

namespace UFlow.Addon.ECS.Core.Runtime {
    public readonly struct PrefabSceneEntityCreatedHook : IHook {
        public readonly SceneEntity sceneEntity;
        public readonly string persistentKey;

        public PrefabSceneEntityCreatedHook(in SceneEntity sceneEntity, in string persistentKey) {
            this.sceneEntity = sceneEntity;
            this.persistentKey = persistentKey;
        }
    }
}