#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// 使字段在检查视图中只读. Editor警告
/// </summary>
public class InspectorReadOnlyAttribute : PropertyAttribute
{

}

[CustomPropertyDrawer(typeof(InspectorReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property,GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position,SerializedProperty property,GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}
#endif