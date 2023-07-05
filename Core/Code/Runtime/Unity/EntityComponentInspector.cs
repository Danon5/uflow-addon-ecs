using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UFlow.Addon.Ecs.Core.Runtime {
    [Serializable]
    internal sealed class EntityComponentInspector {
        [SerializeField, FoldoutGroup("Components"), HideLabel, LabelText("Authoring")] 
        [ListDrawerSettings(ShowFoldout = false)]
        [HideIf(nameof(ShouldDisplayRuntime))]
        private List<EntityComponent> m_authoring = new();

        [ShowInInspector, FoldoutGroup("Components"), HideLabel, LabelText("Runtime")] 
        [ListDrawerSettings(ShowFoldout = false, CustomAddFunction = nameof(Add))]
        [OnCollectionChanged(nameof(ApplyRuntimeComponents))]
        [ShowIf(nameof(ShouldDisplayRuntime))]
        private List<EntityComponent> m_runtime = new();

        private Dictionary<Type, EntityComponent> m_typeMap;
        private Queue<Type> m_typesToSet;
        private Queue<Type> m_typesToRemove;
        private Entity m_entity;
        private World m_world;

        private bool ShouldDisplayRuntime => Application.isPlaying && m_entity.IsAlive();

        public void BakeAuthoringComponents(in Entity entity) {
            foreach (var component in m_authoring)
                entity.SetRaw(component.value, component.enabled);
            m_typeMap = new Dictionary<Type, EntityComponent>();
            m_typesToSet = new Queue<Type>();
            m_typesToRemove = new Queue<Type>();
            m_entity = entity;
            m_world = entity.World;
        }

        public void RetrieveRuntimeComponents() {
            if (!m_entity.IsAlive()) return;
            if (m_world == null) return;
            
            var componentTypes = m_world.GetEntityComponentTypes(m_entity);

            // enqueue removes
            foreach (var (type, component) in m_typeMap) {
                if (component.value == null) continue;
                if (!componentTypes.Contains(type))
                    m_typesToRemove.Enqueue(type);
            }
            
            // apply removes
            while (m_typesToRemove.TryDequeue(out var type)) {
                m_runtime.Remove(m_typeMap[type]);
                m_typeMap.Remove(type);
            }

            // enqueue sets
            foreach (var type in componentTypes) {
                m_typesToSet.Enqueue(type);
            }

            // apply sets
            while (m_typesToSet.TryDequeue(out var type)) {
                var componentValue = m_entity.GetRaw(type);
                if (!m_typeMap.ContainsKey(type)) {
                    var component = new EntityComponent(this, componentValue);
                    m_typeMap.Add(type, component);
                    m_runtime.Add(component);
                }
                else
                    m_typeMap[type].value = componentValue;
                m_typeMap[type].enabled = m_entity.IsEnabledRaw(type);
            }
        }

        public void ApplyRuntimeComponents() {
            if (!m_entity.IsAlive()) return;
            if (m_world == null) return;
            
            // enqueue removes
            foreach (var (type, component) in m_typeMap) {
                if (component.value == null || !m_runtime.Contains(component) || component.value.GetType() != type)
                    m_typesToRemove.Enqueue(type);
            }
            
            // apply removes
            while (m_typesToRemove.TryDequeue(out var type)) {
                m_typeMap.Remove(type);
                m_entity.RemoveRaw(type);
            }
            
            // enqueue sets
            foreach (var component in m_runtime) {
                if (component.value == null) continue;
                var type = component.value.GetType();
                if (!m_typeMap.ContainsKey(type))
                    m_typeMap.Add(type, component);
                m_typesToSet.Enqueue(type);
            }

            // apply sets
            while (m_typesToSet.TryDequeue(out var type)) {
                var component = m_typeMap[type];
                m_entity.SetRaw(type, component.value, component.enabled);
            }
        }

        private void Add() {
            m_runtime.Add(new EntityComponent(this, default));
        }

        private void SetEnabled(in Type type, bool enabled) {
            m_entity.SetEnabledRaw(type, enabled);
        }

        [Serializable]
        [HideReferenceObjectPicker]
        private sealed class EntityComponent {
            [ToggleGroup("enabled", "$Name", CollapseOthersOnExpand = false)]
            [OnValueChanged(nameof(EnabledStateChanged))]
            public bool enabled;

            [ToggleGroup("enabled", "$Name", CollapseOthersOnExpand = false)]
            [SerializeReference, InlineProperty, HideLabel]
            public IEcsComponent value;

            [NonSerialized] 
            public EntityComponentInspector inspector;

            [UsedImplicitly]
            private string Name => value != null ? value.GetType().Name : "None";

            public EntityComponent() {
                enabled = true;
            }

            public EntityComponent(in EntityComponentInspector inspector, in IEcsComponent value) {
                this.inspector = inspector;
                this.value = value;
                enabled = true;
            }

            private void EnabledStateChanged() {
                if (value == null) return;
                inspector.SetEnabled(value.GetType(), !enabled); // no clue why this has to be inverted
            }
        }
    }
}