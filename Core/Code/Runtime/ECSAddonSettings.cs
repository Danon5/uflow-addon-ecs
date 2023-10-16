using UFlow.Core.Runtime;
using UnityEngine;

namespace UFlow.Addon.ECS.Core.Runtime {
    [CreateAssetMenu(
        menuName = MENU_NAME + nameof(ECSAddonSettings), 
        fileName = FILE_NAME + nameof(ECSAddonSettings))]
    public sealed class ECSAddonSettings : BaseAddonSettings {
        public override string AddonName => "ECS";
        [field: SerializeField] public bool EnableSerialization { get; private set; }
        [field: SerializeField] public bool EnableRealtimeInspector { get; private set; }
    }
}