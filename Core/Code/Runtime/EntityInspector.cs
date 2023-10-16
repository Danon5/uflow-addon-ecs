using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sirenix.OdinInspector;
using UFlow.Odin.Runtime;
using UnityEngine;
#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
#endif

[assembly: InternalsVisibleTo("UFlow.Addon.Ecs.Core.Editor")]
namespace UFlow.Addon.ECS.Core.Runtime {
    [Serializable]
    internal sealed class EntityInspector
#if UNITY_EDITOR
        : ISerializationCallbackReceiver
#endif

    {
#if UNITY_EDITOR
        [ColoredBoxGroup("Entity", nameof(Color), GroupName = "$" + nameof(m_entity)), ToggleLeft]
#endif
        [SerializeField]
        private bool m_enabled = true;

#if UNITY_EDITOR
        [ColoredFoldoutGroup("ComponentAuthoring", nameof(Color), GroupName = "Components"), HideLabel, 
         LabelText("Authoring"), ListDrawerSettings(ShowFoldout = false), HideIf(nameof(ShouldDisplayRuntime))]
#endif
        [SerializeField]
        private List<InspectorComponent> m_authoring = new();

#if UNITY_EDITOR
        [SerializeField, ColoredFoldoutGroup("ComponentRuntime", nameof(Color), GroupName = "Components"), HideLabel,
         LabelText("Runtime"), ListDrawerSettings(ShowFoldout = false, CustomAddFunction = nameof(Add)), 
         ShowIf(nameof(ShouldDisplayRuntime))]
        private List<InspectorComponent> m_runtime = new();
#endif

        private Entity m_entity;
        private World m_world;
#if UNITY_EDITOR
        private Dictionary<Type, InspectorComponent> m_typeMap;
        private Queue<Type> m_typesToSet;
        private Queue<Type> m_typesToRemove;
#endif

        internal bool EntityEnabled => m_enabled;
#if UNITY_EDITOR
        internal Color AuthoringColor => new(.25f, .75f, 1f, 1f);
        internal Color RuntimeColor => new(1f, .25f, 0f, 1f);
        internal Color Color => ShouldDisplayRuntime ? m_enabled ? RuntimeColor : GetDisabledColor(RuntimeColor) :
            m_enabled ? AuthoringColor : GetDisabledColor(AuthoringColor);
        internal Color DisabledColor => GetDisabledColor(Color);
        private bool ShouldDisplayRuntime => Application.isPlaying && m_entity.IsAlive();

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
#endif

        public void BakeAuthoringComponents(in Entity entity) {
            m_entity = entity;
            m_world = entity.World;
#if UNITY_EDITOR
            m_typeMap = new Dictionary<Type, InspectorComponent>();
            m_typesToSet = new Queue<Type>();
            m_typesToRemove = new Queue<Type>();
#endif
            foreach (var component in m_authoring) {
                if (component.value == null) continue;
                entity.SetRaw(component.value, component.enabled);
            }
        }

#if UNITY_EDITOR
        public void RetrieveRuntimeState() {
            if (!m_entity.IsAlive()) return;
            if (m_world == null) return;

            m_enabled = m_entity.IsEnabled();

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
            foreach (var type in componentTypes)
                m_typesToSet.Enqueue(type);

            // apply sets
            while (m_typesToSet.TryDequeue(out var type)) {
                var componentValue = m_entity.GetRaw(type);
                if (!m_typeMap.ContainsKey(type)) {
                    var component = new InspectorComponent(this, componentValue);
                    m_typeMap.Add(type, component);
                    m_runtime.Add(component);
                    component.enabled = m_entity.IsEnabledRaw(type);
                }
                else {
                    var component = m_typeMap[type];
                    component.value = componentValue;
                    component.enabled = m_entity.IsEnabledRaw(type);
                }
            }
        }

        public void ApplyRuntimeState() {
            if (!m_entity.IsAlive()) return;
            if (m_world == null) return;

            m_entity.SetEnabled(m_enabled);

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
                if (component.enabled == m_entity.IsEnabledRaw(type) && component.value.Equals(m_entity.GetRaw(type))) continue;
                m_typesToSet.Enqueue(type);
            }

            // apply sets
            while (m_typesToSet.TryDequeue(out var type)) {
                var component = m_typeMap[type];
                m_entity.SetRaw(component.value, type, component.enabled);
                m_entity.SetEnabledRaw(type, component.enabled);
            }
        }

        private void Add(InspectorProperty property) {
            var instance = new InspectorComponent(this, default);
            var collectionResolver = (ICollectionResolver)property.ChildResolver;
            var values = new object[] { instance };
            collectionResolver.QueueAdd(values);
            collectionResolver.ApplyChanges();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Color GetDisabledColor(in Color color) {
            var col = color;
            var a = col.a;
            col /= 2f;
            col.a = a;
            return col;
        }
#endif

        [Serializable]
        [HideReferenceObjectPicker]
        internal sealed class InspectorComponent {
#if UNITY_EDITOR
            [ColoredFoldoutGroup("Default", nameof(Color), GroupName = "$" + nameof(Name)), ToggleLeft]
#endif
            public bool enabled;
#if UNITY_EDITOR
            [ColoredFoldoutGroup("Default", nameof(Color), GroupName = "$" + nameof(Name)),
             ColoredBoxGroup("Default/Box", nameof(Color), GroupName = "Data"),
             InlineProperty, HideLabel]
#endif
            [SerializeReference]
            public IEcsComponent value;
#if UNITY_EDITOR
            [NonSerialized] public EntityInspector inspector;
            
            private string Name => value != null ? value.GetType().Name : "None";
            private Color Color => (enabled && inspector.EntityEnabled) || !inspector.EntityEnabled ? 
                inspector.Color : inspector.DisabledColor;
#endif

            public InspectorComponent() => enabled = true;

#if UNITY_EDITOR
            public InspectorComponent(in EntityInspector inspector, in IEcsComponent value) {
                this.inspector = inspector;
                this.value = value;
                enabled = true;
            }
#endif
        }
    }
}