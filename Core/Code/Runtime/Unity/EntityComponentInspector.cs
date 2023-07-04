using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UFlow.Addon.Ecs.Core.Runtime {
    [Serializable]
    internal sealed class EntityComponentInspector {
        [SerializeField, FoldoutGroup("Components"), HideLabel, LabelText("Authoring"), ListDrawerSettings(ShowFoldout = false)] 
        [HideIf(nameof(IsPlaying))]
        private List<EntityComponent> m_authoring = new();
        
        [ShowInInspector, FoldoutGroup("Components"), HideLabel, LabelText("Runtime"), ListDrawerSettings(ShowFoldout = false), ReadOnly] 
        [ShowIf(nameof(IsPlaying))]
        private List<EntityComponent> m_runtime = new();

        private bool IsPlaying => Application.isPlaying;
        
        public void BakeAuthoringComponents(in Entity entity) {
            foreach (var component in m_authoring)
                entity.SetRaw(component.value);
        }

        public void UpdateRuntimeComponents(in Entity entity) {
            var componentTypes = entity.World.GetEntityComponentTypesEnumerable(entity);
            if (componentTypes == null) return;
            m_runtime.Clear();
            foreach (var componentType in componentTypes)
                m_runtime.Add(new EntityComponent(entity.GetRaw(componentType)));
        }

        [Serializable]
        public struct EntityComponent {
            [SerializeReference, FoldoutGroup("Default", GroupName = "@this.value.GetType().Name"), InlineProperty, HideLabel]
            public object value;

            public EntityComponent(in object value) {
                this.value = value;
            }
        }
    }
}