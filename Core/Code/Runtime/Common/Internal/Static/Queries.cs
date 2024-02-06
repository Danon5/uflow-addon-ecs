using System;
using System.Collections.Generic;
using System.Linq;

namespace UFlow.Addon.Entities.Core.Runtime {
#if IL2CPP_ENABLED
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
#endif
    internal static class Queries {
        private static readonly Dictionary<string, Predicate<Bitset>> s_filterCache;

        static Queries() {
            s_filterCache = new Dictionary<string, Predicate<Bitset>>();
            ExternalEngineEvents.clearStaticCachesEvent += ClearStaticCache;
        }

        public static Predicate<Bitset> GetFilter(Bitset withSet,
                                                           Bitset withoutSet,
                                                           List<Bitset> withEitherSets,
                                                           List<Bitset> withoutEitherSets) {
            withSet = withSet.Copy();
            withoutSet = withoutSet.Copy();
            withEitherSets = withEitherSets?.ToList();
            withoutEitherSets = withoutEitherSets?.ToList();

            var key = $"{withSet} {withoutSet} " +
                $"{string.Join(" ", withEitherSets ?? Enumerable.Empty<Bitset>())} " +
                $"{string.Join(" ", withoutEitherSets ?? Enumerable.Empty<Bitset>())}";

            if (s_filterCache.TryGetValue(key, out var filter)) return filter;

            var singleWithEitherSet = withEitherSets?.FirstOrDefault() ?? default;
            var singleWithoutEitherSet = withoutEitherSets?.FirstOrDefault() ?? default;

            filter = (withoutSet.IsNull, withEitherSets?.Count, withoutEitherSets?.Count) switch {
                (true, null, null) => c => c.Contains(withSet),
                (true, 1, null)    => c => c.Contains(withSet) && !c.DoesNotContain(singleWithEitherSet),
                (true, _, null)    => c => c.Contains(withSet) && withEitherSets!.All(f => !c.DoesNotContain(f)),
                (true, null, 1)    => c => c.Contains(withSet) && !c.Contains(singleWithoutEitherSet),
                (true, null, _)    => c => c.Contains(withSet) && withoutEitherSets!.All(f => !c.Contains(f)),
                (true, 1, 1) => c => c.Contains(withSet) && !c.DoesNotContain(singleWithEitherSet) && !c.Contains(singleWithoutEitherSet),
                (true, 1, _) => c => c.Contains(withSet) && !c.DoesNotContain(singleWithEitherSet) && withoutEitherSets!.All(f => !c.Contains(f)),
                (true, _, 1) => c => c.Contains(withSet) && withEitherSets!.All(f => !c.DoesNotContain(f)) && !c.Contains(singleWithoutEitherSet),
                (true, _, _) => c => c.Contains(withSet) && withEitherSets!.All(f => !c.DoesNotContain(f)) && withoutEitherSets!.All(f => !c.Contains(f)),
                (false, null, null) => c => c.Contains(withSet) && c.DoesNotContain(withoutSet),
                (false, 1, null) => c => c.Contains(withSet) && c.DoesNotContain(withoutSet) && !c.DoesNotContain(singleWithEitherSet),
                (false, _, null) => c => c.Contains(withSet) && c.DoesNotContain(withoutSet) && withEitherSets!.All(f => !c.DoesNotContain(f)),
                (false, null, 1) => c => c.Contains(withSet) && c.DoesNotContain(withoutSet) && !c.Contains(singleWithoutEitherSet),
                (false, null, _) => c => c.Contains(withSet) && c.DoesNotContain(withoutSet) && withoutEitherSets!.All(f => !c.Contains(f)),
                (false, 1, 1) => c => c.Contains(withSet) && c.DoesNotContain(withoutSet) && !c.DoesNotContain(singleWithEitherSet) && !c.Contains(singleWithoutEitherSet),
                (false, 1, _) => c => c.Contains(withSet) && c.DoesNotContain(withoutSet) && !c.DoesNotContain(singleWithEitherSet) && withoutEitherSets!.All(f => !c.Contains(f)),
                (false, _, 1) => c => c.Contains(withSet) && c.DoesNotContain(withoutSet) && withEitherSets!.All(f => !c.DoesNotContain(f)) && !c.Contains(singleWithoutEitherSet),
                (false, _, _) => c => c.Contains(withSet) && c.DoesNotContain(withoutSet) && withEitherSets!.All(f => !c.DoesNotContain(f)) && withoutEitherSets!.All(f => !c.Contains(f)),
            };

            s_filterCache.Add(key, filter);

            return filter;
        }

        private static void ClearStaticCache() {
            s_filterCache.Clear();
        }
    }
}