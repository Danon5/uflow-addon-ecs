using System;
using System.Collections.Generic;
using System.Reflection;
using UFlow.Core.Runtime;

namespace UFlow.Addon.ECS.Core.Runtime {
    public static partial class EcsUtils {
        public static class Worlds {
            public static World CreateWorldFromType<T>() where T : BaseWorldType {
                var world = new World();
                var systemInfos = GetSystemInfosForWorldType(typeof(T));
                var defaultGroupType = typeof(DefaultSystemGroup);
                foreach (var systemInfo in systemInfos) {
                    var group = world.GetOrCreateSystemGroup(systemInfo.groupType ?? defaultGroupType);
                    var system = Activator.CreateInstance(systemInfo.systemType, world);
                    group.Add(system as ISystem);
                }
                Systems.SortSystems(world.id);
                LogicHook<WorldCreatedFromTypeHook>.Execute(new WorldCreatedFromTypeHook(world, typeof(T)));
                return world;
            }

            private static IEnumerable<ReflectedSystemInfo> GetSystemInfosForWorldType(in Type worldType) {
                var baseSystemType = typeof(ISystem);
                var systems = new List<ReflectedSystemInfo>();
                var types = UFlowUtils.Reflection.GetAllTypes(UFlowUtils.Reflection.CommonExclusionNamespaces);
                foreach (var type in types) {
                    if (!baseSystemType.IsAssignableFrom(type)) continue;
                    var attribute = type.GetCustomAttribute<ExecuteInWorldAttribute>();
                    if (attribute == null) continue;
                    foreach (var systemWorldType in attribute.WorldTypes) {
                        if (systemWorldType != worldType) continue;
                        if (type.GetConstructors()[0].GetParameters().Length > 1)
                            throw new Exception($"System {type} has more than just the World parameter in the constructor! " +
                                "Cannot automatically create an instance of it. " +
                                "Remove the [ExecuteInWorld] attribute if you do not want it to be created automatically, or remove the " +
                                "extra parameters from the constructor to resolve the issue.");
                        var groupAttribute = type.GetCustomAttribute<ExecuteInGroupAttribute>();
                        if (groupAttribute != null) {
                            foreach (var groupType in groupAttribute.GroupTypes)
                                systems.Add(new ReflectedSystemInfo(type, groupType));
                        }
                        else
                            systems.Add(new ReflectedSystemInfo(type, default));
                    }
                }
                return systems;
            }

            private readonly struct ReflectedSystemInfo {
                public readonly Type systemType;
                public readonly Type groupType;
                
                public ReflectedSystemInfo(in Type systemType, in Type groupType) {
                    this.systemType = systemType;
                    this.groupType = groupType;
                }
            }
        }
    }
}