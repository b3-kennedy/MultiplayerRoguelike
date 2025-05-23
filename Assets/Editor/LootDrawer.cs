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

[CustomPropertyDrawer(typeof(RecipeItemAndCount))]
public class RecipeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty recipeItem = property.FindPropertyRelative("item");
        SerializedProperty itemCount = property.FindPropertyRelative("count");

        // Get the item name or fallback text
        string objectName = recipeItem.objectReferenceValue != null
            ? recipeItem.objectReferenceValue.name
            : "Unassigned";

        // Get the count as string
        int count = itemCount.intValue;

        // Compose label with name and count
        string combinedLabel = $"{objectName} (x{count})";

        // Draw the whole property with the combined label
        EditorGUI.PropertyField(position, property, new GUIContent(combinedLabel), true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, true);
    }
}