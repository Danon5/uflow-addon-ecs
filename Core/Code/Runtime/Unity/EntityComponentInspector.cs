using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UFlow.Odin.Runtime;
using UnityEngine;

[assembly: InternalsVisibleTo("UFlow.Addon.Ecs.Core.Editor")]
namespace UFlow.Addon.Ecs.Core.Runtime {
    [Serializable]
    internal sealed class EntityComponentInspector : ISerializationCallbackReceiver {
        [ColoredFoldoutGroup("Authoring", "$Color", GroupName = "Components")]
        [HideLabel, LabelText("Authoring")] 
        [ListDrawerSettings(ShowFoldout = false)]
        [HideIf(nameof(ShouldDisplayRuntime))]
        [SerializeField]
        private List<EntityComponent> m_authoring = new();

        [ColoredFoldoutGroup("Runtime", "$Color", GroupName = "Components")]
        [ShowInInspector, HideLabel, LabelText("Runtime")] 
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
        [UsedImplicitly] private Color AuthoringColor => new(.25f, .75f, 1f, 1f);
        [UsedImplicitly] private Color RuntimeColor => new(1f, .25f, .25f, 1f);
        [UsedImplicitly] private Color Color => ShouldDisplayRuntime ? RuntimeColor : AuthoringColor;

        [UsedImplicitly]
        private Color DisabledColor {
            get {
                var col = Color;
                var a = col.a;
                col /= 4f;
                col.a = a;
                return col;
            }
        }

        public void OnBeforeSerialize() {
            foreach (var component in m_authoring)
                component.inspector = this;
            foreach (var component in m_runtime)
                component.inspector = this;
        }

        public void OnAfterDeserialize() {
            foreach (var component in m_authoring)
                component.inspector = this;
            foreach (var component in m_runtime)
                component.inspector = this;
        }

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
                m_entity.SetEnabledRaw(type, component.enabled);
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
        internal sealed class EntityComponent {
            [ColoredFoldoutGroup("Default", "$Color", GroupName = "$Name")]
            [ToggleLeft]
            public bool enabled;

            [ColoredFoldoutGroup("Default", "$Color", GroupName = "$Name")]
            [ColoredBoxGroup("Default/Box", "$Color", GroupName = "Data")]
            [InlineProperty, HideLabel]
            [SerializeReference]
            public IEcsComponent value;

            internal EntityComponentInspector inspector;

            [UsedImplicitly] private string Name => value != null ? value.GetType().Name : "None";
            [UsedImplicitly] private Color Color => enabled ? inspector.Color : inspector.DisabledColor;

            public EntityComponent() {
                enabled = true;
            }

            public EntityComponent(in EntityComponentInspector inspector, in IEcsComponent value) {
                this.inspector = inspector;
                this.value = value;
                enabled = true;
            }
        }
    }
}