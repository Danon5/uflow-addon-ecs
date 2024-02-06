using UnityEngine;

namespace UFlow.Addon.Entities.Core.Runtime {
    public interface ISceneEntity {
        Entity Entity { get; }
        GameObject GameObject { get; }
        Entity CreateEntity();
        Entity CreateEntityWithIdAndGen(int id, ushort gen);
        void DestroyEntity();
        World GetWorld();
    }
}