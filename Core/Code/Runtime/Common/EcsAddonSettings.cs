using System;
using Sirenix.OdinInspector;
using UFlow.Core.Runtime;
using UnityEngine;

namespace UFlow.Addon.Entities.Core.Runtime {
    [Serializable]
    public sealed class EcsAddonSettings : BaseAddonSettings {
        public override string AddonName => "ECS";
        public override string Name => "ECS Settings";
        [field: SerializeField, LabelWidth(LABEL_WIDTH)] public Setting<bool> RealtimeInspectorEnabled { get; private set; } = new();

#if UNITY_EDITOR
        public override void Apply() => RealtimeInspectorEnabled.ApplyProposedValue();

        public override void Revert() => RealtimeInspectorEnabled.RevertProposedValue();
        
        public override bool HasUnappliedChanges() => 
            RealtimeInspectorEnabled.ProposedValueDiffersFromCurrent();
#endif
    }
}