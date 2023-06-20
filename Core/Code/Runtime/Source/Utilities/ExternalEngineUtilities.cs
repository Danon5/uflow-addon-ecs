using System;

namespace UFlow.Core.Runtime {
    public static class ExternalEngineUtilities {
        internal static event Action ClearStaticCacheEvent;
        
        public static void ClearStaticCache() {
            ClearStaticCacheEvent?.Invoke();
        }
    }
}