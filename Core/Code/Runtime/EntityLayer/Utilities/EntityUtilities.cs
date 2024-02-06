namespace UFlow.Addon.Entities.Core.Runtime {
    public static partial class EcsUtils {
        public static class Entities {
            public static void RemoveComponentDerivingFromType<T>(in Entity entity) where T : IEcsComponentData {
                var type = typeof(T);
                foreach (var componentType in entity.World.GetEntityComponentTypes(entity)) {
                    if (!type.IsAssignableFrom(componentType)) continue;
                    entity.RemoveRaw(componentType);
                    return;
                }
            }
            
            public static IEcsComponentData GetComponentDerivingFromType<T>(in Entity entity) where T : IEcsComponentData {
                var type = typeof(T);
                foreach (var componentType in entity.World.GetEntityComponentTypes(entity)) {
                    if (!type.IsAssignableFrom(componentType)) continue;
                    return entity.GetRaw(componentType);
                }
                return default;
            }
            
            public static bool HasComponentDerivingFromType<T>(in Entity entity) where T : IEcsComponentData {
                var type = typeof(T);
                foreach (var componentType in entity.World.GetEntityComponentTypes(entity)) {
                    if (!type.IsAssignableFrom(componentType)) continue;
                    return true;
                }
                return false;
            }
        }
    }
}