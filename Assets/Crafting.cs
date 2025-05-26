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

        ShowRecipes();
    }

    private bool CanCraft(List<RecipeItemAndCount> requiredItems, Dictionary<string, int> inventory)
    {
        foreach (var required in requiredItems)
        {
            string itemName = required.item.name;
            if (!inventory.ContainsKey(itemName) || inventory[itemName] < required.count)
            {
                return false;
            }
        }
        return true;
    }

    public void ShowRecipes()
    {
        var collectionBox = FindFirstObjectByType<CollectionBox>();
        Dictionary<string, int> inventory = collectionBox.GetInventory();

        if (weaponsMaterialParent.childCount > 0)
        { 
            for (int i = 0; i < weaponsMaterialParent.childCount; i++)
            {
                Destroy(weaponsMaterialParent.GetChild(i).gameObject);
            }
        }

        foreach (var item in weaponsMaterialsRecipes)
        {
            var recipe = item.Key;
            var recipeItemsLists = item.Value;

            // Find the first craftable recipe group
            RecipeItemGroup selectedGroup = recipeItemsLists
                .Find(group => CanCraft(group.items, inventory));

            // Skip this recipe if it's not craftable
            if (selectedGroup == null)
                continue;

            // Only now, spawn the UI
            GameObject recipeUI = Instantiate(itemCraftingUIElement, weaponsMaterialParent);
            CraftableItemUI itemUI = recipeUI.GetComponent<CraftableItemUI>();

            itemUI.itemCount = selectedGroup.quantityMade;
            itemUI.nameText.text = recipe.recipeName + " x" + itemUI.itemCount.ToString();

            GameObject itemTextPrefab = itemUI.itemText;

            foreach (var recipeItem in selectedGroup.items)
            {
                GameObject spawnedItemText = Instantiate(itemTextPrefab, itemUI.recipeItemParent);
                spawnedItemText.GetComponent<TextMeshProUGUI>().text =
                    recipeItem.item.name + ": " + recipeItem.count;
            }

            itemUI.craftButton.onClick.AddListener(delegate
            {
                CraftItem(selectedGroup.items, itemUI.itemCount, itemUI.nameText.text);
            });
        }
    }

    public void SortRecipes(GameObject collectionBox)
    {

    }

    void CraftItem(List<RecipeItemAndCount> itemAndCount, int quantity, string itemName)
    {
        GetComponent<PlayerInterfaceManager>().AddCraftedItemToCollectionBox(itemAndCount, quantity, itemName);
        ShowRecipes();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
