namespace UFlow.Addon.ECS.Core.Runtime {
    public static partial class EcsUtils {
        public static class Entities {
            public static void RemoveComponentDerivingFromType<T>(in Entity entity) where T : IEcsComponent {
                var type = typeof(T);
                foreach (var componentType in entity.World.GetEntityComponentTypes(entity)) {
                    if (!type.IsAssignableFrom(componentType)) continue;
                    entity.RemoveRaw(componentType);
                    return;
                }
            }
            
            public static IEcsComponent GetComponentDerivingFromType<T>(in Entity entity) where T : IEcsComponent {
                var type = typeof(T);
                foreach (var componentType in entity.World.GetEntityComponentTypes(entity)) {
                    if (!type.IsAssignableFrom(componentType)) continue;
                    return entity.GetRaw(componentType);
                }
                return default;
            }
            
            public static bool HasComponentDerivingFromType<T>(in Entity entity) where T : IEcsComponent {
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