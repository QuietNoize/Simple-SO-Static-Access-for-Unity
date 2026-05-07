namespace QuietNoize.SimpleSOStaticAccess.Editor
{
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Custom property drawer for StaticDataCodegenSettings.
    /// Renders code generation settings in the inspector.
    /// </summary>
    [CustomPropertyDrawer(typeof(StaticDataCodegenSettings))]
    public class StaticDataCodegenSettingsDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var isAutoSaved = property.FindPropertyRelative("m_isAutoSaved");
            var wrapperCodePath = property.FindPropertyRelative("m_wrapperCodePath");
            var wrapperClassName = property.FindPropertyRelative("m_wrapperClassName");
            var wrapperNamespace = property.FindPropertyRelative("m_wrapperCodeNamespace");

            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = 2;

            Rect r1 = new Rect(position.x, position.y, position.width, lineHeight);
            Rect r2 = new Rect(position.x, position.y + lineHeight + spacing, position.width, lineHeight);
            Rect r3 = new Rect(position.x, position.y + (lineHeight + spacing) * 2, position.width, lineHeight);
            Rect r4 = new Rect(position.x, position.y + (lineHeight + spacing) * 3, position.width, lineHeight);

            EditorGUI.PropertyField(
                r1,
                isAutoSaved,
                new GUIContent(
                    "Auto Save",
                    "Automatically regenerates the generated static class when the inspector is closed."
                )
            );

            EditorGUI.PropertyField(
                r2,
                wrapperCodePath,
                new GUIContent(
                    "C# Class File",
                    "The output path for the generated C# static wrapper class. If not specified, the script will be created in the same directory as the source asset."
                )
            );

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(
                r3,
                wrapperClassName,
                new GUIContent(
                    "C# Class Name",
                    "The name of the generated static wrapper class."
                )
            );
            if (EditorGUI.EndChangeCheck())
            {
                string rawName = wrapperClassName.stringValue;
                string sanitized = SanitizeClassName(rawName);

                if (sanitized != rawName)
                {
                    wrapperClassName.stringValue = sanitized;
                }
            }

            EditorGUI.PropertyField(
                r4,
                wrapperNamespace,
                new GUIContent(
                    "C# Class Namespace",
                    "The namespace used for the generated static wrapper class. Leave empty for global namespace."
                )
            );

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (EditorGUIUtility.singleLineHeight + 2) * 4;
        }

        private static void DrawStringFieldWithPlaceholder(
            Rect position,
            SerializedProperty property,
            GUIContent label,
            string placeholder)
        {
            EditorGUI.BeginProperty(position, label, property);

            Rect fieldRect = EditorGUI.PrefixLabel(position, label);

            string value = EditorGUI.TextField(fieldRect, property.stringValue);

            if (value != property.stringValue)
            {
                property.stringValue = value;
            }

            if (string.IsNullOrEmpty(property.stringValue))
            {
                var placeholderStyle = new GUIStyle(EditorStyles.textField)
                {
                    normal =
            {
                textColor = new Color(1f, 1f, 1f, 0.35f)
            }
                };

                EditorGUI.LabelField(fieldRect, placeholder, placeholderStyle);
            }

            EditorGUI.EndProperty();
        }

        /// <summary>
        /// Sanitizes a string to be a valid C# class name.
        /// Removes invalid characters and ensures it does not start with a digit.
        /// </summary>
        private static string SanitizeClassName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return string.Empty;

            var chars = new System.Text.StringBuilder(name.Length);
            bool modified = false;

            foreach (char c in name)
            {
                if (char.IsLetterOrDigit(c) || c == '_')
                {
                    chars.Append(c);
                }
                else
                {
                    modified = true;
                }
            }

            string result = chars.ToString();

            if (result.Length > 0 && char.IsDigit(result[0]))
            {
                result = "_" + result;
                modified = true;
            }

            if (modified)
            {
                Debug.LogWarning(
                    $"[StaticDataCodegen] Class name '{name}' was sanitized to '{result}'."
                );
            }

            return result;
        }
    }
}
