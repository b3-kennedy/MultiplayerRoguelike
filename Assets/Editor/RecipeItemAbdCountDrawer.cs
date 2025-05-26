using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(RecipeItemAndCount))]
public class RecipeItemAndCountDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 3 + 6;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Get parent Recipe object
        var recipeProp = property.serializedObject.FindProperty("type");
        var recipeType = (Recipe.RecipeType)recipeProp.enumValueIndex;

        EditorGUI.BeginProperty(position, label, property);

        // Layout rectangles
        Rect itemLabelRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        Rect itemRect = new Rect(position.x, position.y + 18, position.width, EditorGUIUtility.singleLineHeight);
        Rect itemNameRect = new Rect(position.x, position.y + 36, position.width, EditorGUIUtility.singleLineHeight);

        EditorGUI.LabelField(itemLabelRect, label);

        SerializedProperty item = property.FindPropertyRelative("item");
        SerializedProperty itemName = property.FindPropertyRelative("itemName");
        SerializedProperty count = property.FindPropertyRelative("count");

        if (recipeType == Recipe.RecipeType.CONVERT)
        {
            EditorGUI.PropertyField(itemRect, item);
        }
        else
        {
            EditorGUI.PropertyField(itemRect, itemName);
        }

        EditorGUI.PropertyField(itemNameRect, count);

        EditorGUI.EndProperty();
    }
}