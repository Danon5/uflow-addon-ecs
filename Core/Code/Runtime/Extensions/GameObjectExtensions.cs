using UnityEngine;

namespace UFlow.Addon.ECS.Core.Runtime {
    public static class GameObjectExtensions {
        public static Entity AsEntity(this GameObject gameObject) {
            return gameObject.GetComponent<SceneEntity>().Entity;
        }
    }
}