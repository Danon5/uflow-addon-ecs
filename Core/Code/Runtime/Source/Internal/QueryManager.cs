using System;
using System.Collections.Generic;
using UFlow.Core.Runtime.DataStructures;

namespace UFlow.Core.Runtime {
    internal static class QueryManager {
        private static readonly Dictionary<string, Predicate<ComponentBitset>> s_queryCache;

        static QueryManager() {
            s_queryCache = new Dictionary<string, Predicate<ComponentBitset>>();
            ExternalEngineUtilities.ClearStaticCacheEvent += ClearStaticCache;
        }

        public static Predicate<ComponentBitset> GetQuery(ComponentBitset withSet, ComponentBitset withoutSet) {
            withSet = withSet.Copy();
            withoutSet = withoutSet.Copy();

            var key = $"{withSet} {withoutSet}";
            if (s_queryCache.TryGetValue(key, out var query)) return query;

            query = withoutSet.IsNull switch {
                true  => set => set.Contains(withSet),
                false => set => set.Contains(withSet) && set.DoesNotContain(withoutSet)
            };

            s_queryCache.Add(key, query);
            return query;
        }

        private static void ClearStaticCache() {
            s_queryCache?.Clear();
        }
    }
}