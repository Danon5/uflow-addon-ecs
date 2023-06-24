using UFlow.Core.Runtime;

namespace UFlow.Addon.Ecs.Core.Runtime.Modules {
    public abstract class BaseEcsModule : BaseModule {
        public World LocalWorld { get; private set; }
        
        
    }
}