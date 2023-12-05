using UFlow.Core.Runtime;
using UnityEngine;

// ReSharper disable Unity.PerformanceCriticalCodeInvocation

namespace UFlow.Addon.ECS.Core.Runtime {
    public sealed class EcsModule<T> : BaseBehaviourModule<EcsModule<T>> where T : BaseWorldType {
        public World World { get; private set; }
        
        public override void LoadDirect() {
            World = EcsUtils.Worlds.CreateWorldFromType<T>();
            World.SetupSystemGroups();
            World.SubscribeReset(() => World.ResetSystemGroups());
        }

        public override void UnloadDirect() {
            World.Destroy();
        }

        public override void Update() {
            World.RunSystemGroup<FrameSimulationSystemGroup>(Time.deltaTime);
            World.RunSystemGroup<FrameRenderSystemGroup>(Time.deltaTime);
        }

        public override void FixedUpdate() {
            World.RunSystemGroup<FixedSimulationSystemGroup>(Time.fixedDeltaTime);
            World.RunSystemGroup<FixedRenderSystemGroup>(Time.fixedDeltaTime);
        }

        public override void LateUpdate() {
            World.RunSystemGroup<LateFrameSimulationSystemGroup>(Time.deltaTime);
            World.RunSystemGroup<LateFrameRenderSystemGroup>(Time.deltaTime);
        }

        public override void OnDrawGizmos() => World.RunSystemGroup<GizmoSystemGroup>(Time.deltaTime);

        public override void OnGUI() => World.RunSystemGroup<GUISystemGroup>(Time.deltaTime);
    }
}