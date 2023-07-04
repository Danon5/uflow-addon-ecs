using Sirenix.OdinInspector.Editor;
using UFlow.Addon.Ecs.Core.Runtime;
using UnityEditor;
using UnityEngine;

namespace UFlow.Addon.Ecs.Core.Editor.Editors {
    [CustomEditor(typeof(SceneEntity), true)]
    [CanEditMultipleObjects]
    public sealed class SceneEntityEditor : OdinEditor {
        protected override void OnEnable() {
            base.OnEnable();
            EditorApplication.update += Update;
        }

        protected override void OnDisable() {
            base.OnDisable();
            EditorApplication.update -= Update;
        }

        private void Update() {
            var sceneEntity = target as SceneEntity;
            if (Application.isPlaying && sceneEntity != null) sceneEntity.UpdateRuntimeInspector();
            Repaint();
        }
    }
}