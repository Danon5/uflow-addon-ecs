using System.Runtime.CompilerServices;
using UnityEngine;

namespace UFlow.Addon.ECS.Core.Runtime {
    public static class GameObjectExtensions {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject Instantiate(this GameObject gameObject) => 
            Object.Instantiate(gameObject);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject Instantiate(this GameObject gameObject, Transform parent) => 
            Object.Instantiate(gameObject, parent);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject Instantiate(this GameObject gameObject, Vector3 position) => 
            Object.Instantiate(gameObject, position, Quaternion.identity);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject Instantiate(this GameObject gameObject, Vector3 position, Quaternion rotation) => 
            Object.Instantiate(gameObject, position, rotation);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Entity AsEntity(this GameObject gameObject) {
            if (!gameObject.TryGetComponent(out SceneEntity sceneEntity))
                sceneEntity = gameObject.AddComponent<SceneEntity>();
            return sceneEntity.Entity.IsAlive() ? sceneEntity.Entity : sceneEntity.CreateEntity();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Entity AsEntityWithIdAndGen(this GameObject gameObject, int id, ushort gen) {
            if (!gameObject.TryGetComponent(out SceneEntity sceneEntity))
                sceneEntity = gameObject.AddComponent<SceneEntity>();
            return sceneEntity.CreateEntityWithIdAndGen(id, gen);
        }
    }
}