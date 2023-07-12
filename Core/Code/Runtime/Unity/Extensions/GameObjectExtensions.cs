using UnityEngine;

namespace UFlow.Addon.Ecs.Core.Runtime {
    public static class GameObjectExtensions {
        public static Entity AsEntity(this GameObject gameObject) {
            return gameObject.GetComponent<SceneEntity>().Entity;
        }
    }
}