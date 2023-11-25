using UFlow.Core.Runtime;
using UnityEngine;

namespace UFlow.Addon.ECS.Core.Runtime {
    [CreateAssetMenu(
        menuName = MENU_NAME + nameof(EcsAddonSettings), 
        fileName = FILE_NAME + nameof(EcsAddonSettings))]
    public sealed class EcsAddonSettings : BaseAddonSettings {
        public override string AddonName => "ECS";
        [field: SerializeField] public bool EnableRealtimeInspector { get; private set; }
    }
}