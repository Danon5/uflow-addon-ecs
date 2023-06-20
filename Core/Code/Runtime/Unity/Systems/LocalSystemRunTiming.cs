namespace UFlow.Core.Runtime {
    public enum LocalSystemRunTiming : byte {
        Update,
        FixedUpdate,
        LateUpdate,
        OnDrawGizmos,
        OnGUI
    }
}