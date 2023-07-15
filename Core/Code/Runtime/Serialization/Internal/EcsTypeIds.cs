using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("UFlow.Addon.SaveState.Core.Runtime")]
[assembly: InternalsVisibleTo("UFlow.Addon.NetSync.Core.Runtime")]
namespace UFlow.Addon.Ecs.Core.Runtime {
    internal enum EcsTypeIds : ushort {
        Entity = 0,
        World = 1
    }
}