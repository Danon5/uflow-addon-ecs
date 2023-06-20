namespace UFlow.Addon.Ecs.Core.Runtime {
    public enum LocalSystemRunTiming : byte {
        Update,
        FixedUpdate,
        LateUpdate,
        OnDrawGizmos,
        OnGUI
    }
}