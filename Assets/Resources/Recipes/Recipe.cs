using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RecipeItemAndCount
{
    public GameObject item;
    public string itemName;
    public int count;
}

[System.Serializable]
public class RecipeItemGroup
{
    public List<RecipeItemAndCount> items;
    public int quantityMade;
}


[CreateAssetMenu(fileName = "Recipe", menuName = "Scriptable Objects/Recipe")]
public class Recipe : ScriptableObject
{
    public enum RecipeType {CONVERT, CRAFTING};
    public RecipeType type;
    public string recipeName;
    public List<RecipeItemGroup> recipeItems;

}
