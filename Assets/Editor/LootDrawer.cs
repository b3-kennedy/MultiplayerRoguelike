using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Loot))]
public class LootDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty lootObjectProp = property.FindPropertyRelative("lootObject");
        string objectName = lootObjectProp.objectReferenceValue ? lootObjectProp.objectReferenceValue.name : "Unassigned";

        EditorGUI.PropertyField(position, property, new GUIContent(objectName), true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, true);
    }
}