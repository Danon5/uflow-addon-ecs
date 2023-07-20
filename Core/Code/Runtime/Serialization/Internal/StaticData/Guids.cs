using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UFlow.Addon.Ecs.Core.Runtime {
    internal static class Guids {
        private static readonly Dictionary<string, int> s_guidToHash = new();
        private static readonly Dictionary<int, string> s_hashToGuid = new();

        static Guids() {
            ExternalEngineEvents.clearStaticCachesEvent += ClearStaticCaches;
        }

        public static void RegisterGuid(in string guid) {
            var hash = guid.GetHashCode();
            s_guidToHash.Add(guid, hash);
            s_hashToGuid.Add(hash, guid);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetHash(in string guid) => s_guidToHash[guid];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetGuid(int hash) => s_hashToGuid[hash];

        private static void ClearStaticCaches() {
            s_guidToHash.Clear();
            s_hashToGuid.Clear();
        }
    }
}