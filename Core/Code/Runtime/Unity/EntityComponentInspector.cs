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
        [HideIf(nameof(IsPlaying))]
        private List<EntityComponent> m_authoring = new();

        [ShowInInspector, FoldoutGroup("Components"), HideLabel, LabelText("Runtime")] 
        [ListDrawerSettings(ShowFoldout = false, CustomAddFunction = nameof(Add))]
        [OnCollectionChanged(nameof(ApplyRuntimeComponents))]
        [ShowIf(nameof(IsPlaying))]
        private List<EntityComponent> m_runtime = new();

        private Dictionary<Type, EntityComponent> m_typeMap;
        private Queue<Type> m_typesToSet;
        private Queue<Type> m_typesToRemove;
        private Entity m_entity;
        private World m_world;

        private bool IsPlaying => Application.isPlaying;

        public void BakeAuthoringComponents(in Entity entity) {
            foreach (var component in m_authoring)
                entity.SetRaw(component.value);
            m_typeMap = new Dictionary<Type, EntityComponent>();
            m_typesToSet = new Queue<Type>();
            m_typesToRemove = new Queue<Type>();
            m_entity = entity;
            m_world = entity.World;
        }

        public void RetrieveRuntimeComponents() {
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
                    var component = new EntityComponent(componentValue);
                    m_typeMap.Add(type, component);
                    m_runtime.Add(component);
                }
                else
                    m_typeMap[type].value = componentValue;
            }
        }

        public void ApplyRuntimeComponents() {
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
                m_entity.SetRaw(m_typeMap[type].value, type);
            }
        }

        private void Add() {
            m_runtime.Add(new EntityComponent(default));
        }

        [Serializable]
        [HideReferenceObjectPicker]
        private sealed class EntityComponent {
            [SerializeReference, FoldoutGroup("Default", GroupName = "@this.Name"), InlineProperty, HideLabel]
            public object value;

            [UsedImplicitly]
            private string Name => value != null ? value.GetType().Name : "None";

            public EntityComponent() { }

            public EntityComponent(in object value) {
                this.value = value;
            }
        }
    }
}