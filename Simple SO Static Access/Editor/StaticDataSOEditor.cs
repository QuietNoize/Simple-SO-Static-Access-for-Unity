namespace QuietNoize.SimpleSOStaticAccess.Editor
{
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Custom inspector for StaticDataSO.
    /// Provides generation controls and displays code generation settings.
    /// </summary>
    [CustomEditor(typeof(StaticDataSO), true)]
    public class StaticDataSOEditor : Editor
    {
        #region Serialized Properties
        private SerializedProperty m_codegenSettingsProp;
        #endregion

        #region Unity Lifecycle
        protected virtual void OnEnable()
        {
            m_codegenSettingsProp = serializedObject.FindProperty("m_codegenSettings");
        }

        protected virtual void OnDisable()
        {
            AutoSave();
        }
        #endregion

        #region GUI Drawing
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            bool drewAnyAbove = DrawCustomInspectorGUI();

            if (drewAnyAbove)
            {
                EditorGUILayout.Space(3f);
                Rect r = EditorGUILayout.GetControlRect(false, 1);
                EditorGUI.DrawRect(r, new Color(1f, 1f, 1f, 0.08f));
                EditorGUILayout.Space(3f);
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.PropertyField(m_codegenSettingsProp);
                if (GUILayout.Button("Save / Generate"))
                {
                    Save();
                }
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.HelpBox(
                "Only Unity-serialized fields are included in code generation.\n" +
                "Public fields and private fields marked with [SerializeField] are included. Hidden or non-serialized fields are ignored.",
                MessageType.Warning
            );

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Override this method to draw custom inspector content above the code generation settings.
        /// Return true if any GUI elements were drawn — this is used to render a separator line
        /// between the custom fields and the base fields of StaticDataSOEditor,
        /// including the StaticDataCodegenSettings.
        /// </summary>
        protected virtual bool DrawCustomInspectorGUI()
        {
            return DrawSerializedPropertiesExcluding(serializedObject, "m_Script", "m_codegenSettings");
        }

        private bool DrawSerializedPropertiesExcluding(SerializedObject so, params string[] excluded)
        {
            bool drewAny = false;

            SerializedProperty prop = so.GetIterator();
            bool enterChildren = true;

            while (prop.NextVisible(enterChildren))
            {
                enterChildren = false;

                bool isExcluded = false;
                for (int i = 0; i < excluded.Length; i++)
                {
                    if (prop.name == excluded[i])
                    {
                        isExcluded = true;
                        break;
                    }
                }

                if (isExcluded)
                    continue;

                EditorGUILayout.PropertyField(prop, true);
                drewAny = true;
            }

            return drewAny;
        }
        #endregion

        #region Generation
        private void Save()
        {
            var so = (StaticDataSO)target;
            StaticDataGenerator.SyncConstants(so);
        }

        private void AutoSave()
        {
            bool canAutoSave = m_codegenSettingsProp
                .FindPropertyRelative("m_isAutoSaved")
                .boolValue;

            if (canAutoSave)
            {
                Save();
            }
        }
        #endregion
    }
#endif
}
