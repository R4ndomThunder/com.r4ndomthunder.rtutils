/**
*   MIT License
*
*   Samuele Padalino @R4ndomThunder
*   https://samuelepadalino.dev
*/

using UnityEditor;
using UnityEngine;

namespace RTDK.InspectorPlus.Editor
{
    [CustomPropertyDrawer(typeof(ToggleGroupAttribute))]
    public class ToggleGroupDrawer : PropertyDrawerBase
    {
        private bool isExpanded;
        private bool isToggled;
        private bool hasLoaded;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var toggleGroup = attribute as ToggleGroupAttribute;

            var isToggledSaveKey = $"{property.serializedObject.targetObject}_{property.propertyPath}_IsToggled";
            var isExpandedSaveKey = $"{property.serializedObject.targetObject}_{property.propertyPath}_IsExpanded";

            if (!hasLoaded)
            {
                isExpanded = EditorPrefs.GetBool(isExpandedSaveKey, isExpanded);
                isToggled = EditorPrefs.GetBool(isToggledSaveKey, isToggled);
                hasLoaded = true;
            }

            isExpanded = DrawToggleFoldout(toggleGroup.GroupName, isExpanded, ref isToggled);

            EditorGUIUtility.labelWidth = toggleGroup.LabelWidth;
            EditorGUIUtility.fieldWidth = toggleGroup.FieldWidth;

            if (isExpanded)
            {
                var groupStyle = toggleGroup.DrawInBox ? EditorStyles.helpBox : EditorStyles.inspectorFullWidthMargins;

                using (new EditorGUI.DisabledGroupScope(!isToggled))
                {
                    EditorGUILayout.BeginVertical(groupStyle);

                    foreach (string variableName in toggleGroup.FieldsToGroup)
                    {
                        var variableProperty = FindNestedProperty(property, variableName);

                        // Check for serialized properties since they have a weird naming when serialized and they cannot be found by the normal name
                        variableProperty ??= FindNestedProperty(property, $"<{variableName}>k__BackingField");

                        if (variableProperty != null)
                        {
                            EditorGUILayout.PropertyField(variableProperty, true);
                        }
                        else
                        {
                            EditorGUILayout.HelpBox($"{variableName} is not a valid field", MessageType.Error);
                            break;
                        }
                    }

                    EditorGUILayout.EndVertical();
                }
            }

            if (property.propertyType == SerializedPropertyType.Boolean)
                property.boolValue = isToggled;

            EditorPrefs.SetBool(isToggledSaveKey, isToggled);
            EditorPrefs.SetBool(isExpandedSaveKey, isExpanded);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => -EditorGUIUtility.standardVerticalSpacing; // Remove the space for the holder field

        private bool DrawToggleFoldout(string title, bool isExpanded, ref bool isToggled)
        {
            var backgroundRect = GUILayoutUtility.GetRect(1f, 17f);

            var labelRect = backgroundRect;
            labelRect.xMin += 32f;
            labelRect.xMax -= 20f;

            var foldoutRect = backgroundRect;
            foldoutRect.y += 1f;
            foldoutRect.width = 13f;
            foldoutRect.height = 13f;

            var toggleRect = backgroundRect;
            toggleRect.x += 16f;
            toggleRect.y += 2f;
            toggleRect.width = 13f;
            toggleRect.height = 13f;
            backgroundRect.width += 4f;

            EditorGUI.DrawRect(backgroundRect, new Color(0.1f, 0.1f, 0.1f, 0.2f));

            using (new EditorGUI.DisabledScope(!isToggled))
                EditorGUI.LabelField(labelRect, title, EditorStyles.boldLabel);

            isExpanded = EditorGUI.Toggle(foldoutRect, isExpanded, EditorStyles.foldout);
            isToggled = EditorGUI.Toggle(toggleRect, isToggled, new GUIStyle("ShurikenToggle"));

            var @event = Event.current;

            if (@event.type == EventType.MouseDown && labelRect.Contains(@event.mousePosition) && @event.button == 0)
            {
                isExpanded = !isExpanded;
                @event.Use();
            }

            return isExpanded;
        }
    }
}