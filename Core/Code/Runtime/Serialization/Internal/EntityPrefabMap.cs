using System.Collections.Generic;

namespace UFlow.Addon.Ecs.Core.Runtime {
    internal sealed class EntityPrefabMap {
        private readonly Dictionary<string, int> m_guidToHash;
        private readonly Dictionary<int, string> m_hashToGuid;
    }
}