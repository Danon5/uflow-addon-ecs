using UFlow.Core.Runtime;
using UnityEngine;

// ReSharper disable Unity.PerformanceCriticalCodeInvocation

namespace UFlow.Addon.Entities.Core.Runtime {
    public sealed class EcsModule<T> : BaseBehaviourModule<EcsModule<T>> where T : BaseWorldType {
        public World World { get; private set; }
        public bool AutoRunSystemGroups { get; set; } = true;
        
        public override void LoadDirect() {
            World = EcsUtils.Worlds.CreateWorldFromType<T>();
            World.SetupSystemGroups();
            World.SubscribeReset(World.ResetSystemGroups);
        }

        public override void UnloadDirect() {
            World.Destroy();
        }

        public override void Update() {
            if (!AutoRunSystemGroups) return;
            World.RunSystemGroup<UpdateSystemGroup>(Time.deltaTime);
        }

        public override void FixedUpdate() {
            if (!AutoRunSystemGroups) return;
            World.RunSystemGroup<FixedUpdateSystemGroup>(Time.fixedDeltaTime);
        }

        public override void LateUpdate() {
            if (!AutoRunSystemGroups) return;
            World.RunSystemGroup<LateUpdateSystemGroup>(Time.deltaTime);
        }

        public override void OnDrawGizmos() {
            if (!AutoRunSystemGroups) return;
            World.RunSystemGroup<GizmoSystemGroup>(Time.deltaTime);
        }

        public override void OnGUI() {
            if (!AutoRunSystemGroups) return;
            World.RunSystemGroup<GUISystemGroup>(Time.deltaTime);
        }
    }
}