using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Crafting : MonoBehaviour
{
    public Dictionary<Recipe, List<RecipeItemGroup>> weaponsMaterialsRecipes = new Dictionary<Recipe, List<RecipeItemGroup>>();
    PlayerInterfaceManager playerInterfaceManager;

    [SerializeField] Transform weaponsMaterialParent;

    [SerializeField] GameObject itemCraftingUIElement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInterfaceManager = GetComponent<PlayerInterfaceManager>();

        Recipe[] recipes = Resources.LoadAll<Recipe>("Recipes/Convert/Weapons");

        foreach (var recipe in recipes)
        {
            if (!weaponsMaterialsRecipes.ContainsKey(recipe))
            {
                weaponsMaterialsRecipes.Add(recipe, recipe.recipeItems);
            }
            else
            {
                Debug.LogWarning($"Duplicate recipe detected: {recipe}. Skipping.");
            }
        }

        foreach (var item in weaponsMaterialsRecipes)
        {
            var recipe = item.Key;
            var recipeItemsLists = item.Value;
            GameObject recipeUI = Instantiate(itemCraftingUIElement, weaponsMaterialParent);
            CraftableItemUI itemUI = recipeUI.GetComponent<CraftableItemUI>();
            itemUI.nameText.text = recipe.recipeName;
            //itemUI.image =
            List<RecipeItemAndCount> firstRecipe = recipeItemsLists[1].items;
            GameObject itemTextPrefab = itemUI.itemText;
            foreach (var recipeItem in firstRecipe)
            {
                GameObject spawnedItemText = Instantiate(itemTextPrefab, itemUI.recipeItemParent);
                spawnedItemText.GetComponent<TextMeshProUGUI>().text = recipeItem.item.gameObject.name + ": " + recipeItem.count.ToString();
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
