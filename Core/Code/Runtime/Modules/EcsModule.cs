using UFlow.Core.Runtime;

namespace UFlow.Addon.ECS.Core.Runtime {
    public sealed class EcsModule : BaseBehaviourModule {
        public World World { get; private set; }
        
        public override void Load() {
            World = EcsUtils.Worlds.CreateWorldFromType<DefaultWorld>();
            World.SetupSystemGroups();
            World.WhenReset(() => World.ResetSystemGroups());
        }

        public override void Unload() {
            World.Destroy();
        }

        public override void Update() {
            World.RunSystemGroup<FrameSimulationSystemGroup>();
            World.RunSystemGroup<FrameRenderSystemGroup>();
        }

        public override void FixedUpdate() {
            World.RunSystemGroup<FixedSimulationSystemGroup>();
            World.RunSystemGroup<FixedRenderSystemGroup>();
        }

        public override void LateUpdate() {
            World.RunSystemGroup<LateFrameSimulationSystemGroup>();
            World.RunSystemGroup<LateFrameRenderSystemGroup>();
        }
    }
}