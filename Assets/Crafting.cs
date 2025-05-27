using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Crafting : NetworkBehaviour
{
    public Dictionary<Recipe, List<RecipeItemGroup>> weaponsMaterialsRecipes = new Dictionary<Recipe, List<RecipeItemGroup>>();
    PlayerInterfaceManager playerInterfaceManager;

    [SerializeField] Transform weaponsMaterialParent;

    [SerializeField] Transform craftingMaterialParent;

    [SerializeField] GameObject itemCraftingUIElement;

    CollectionBox collectionBox;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        collectionBox = FindFirstObjectByType<CollectionBox>();

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
            string itemName;
            if (!string.IsNullOrEmpty(required.itemName))
            {
                itemName = required.itemName;
            }
            else
            {
                if (required.item != null)
                {
                    itemName = required.item.name;
                }
                else
                {
                    itemName = null;
                }
            }

            if (string.IsNullOrEmpty(itemName))
            {
                Debug.LogWarning("RecipeItemAndCount has neither itemName nor item GameObject set!");
                return false;
            }

            if (!inventory.ContainsKey(itemName) || inventory[itemName] < required.count)
            {
                return false;
            }
        }
        return true;
    }

    public void ShowRecipes()
    {
        
        Dictionary<string, int> inventory = collectionBox.GetInventory();

        // Clear UI elements
        foreach (Transform child in weaponsMaterialParent)
            Destroy(child.gameObject);

        foreach (Transform child in craftingMaterialParent)
            Destroy(child.gameObject);

        foreach (var item in weaponsMaterialsRecipes)
        {
            var recipe = item.Key;
            var recipeItemsLists = item.Value;

            RecipeItemGroup selectedGroup = recipeItemsLists.Find(group => CanCraft(group.items, inventory));
            if (selectedGroup == null)
                continue;

            if (recipe.type == Recipe.RecipeType.CONVERT)
            {
                CreateRecipeUI(recipe, selectedGroup, weaponsMaterialParent, recipe.type);
            }
            else if (recipe.type == Recipe.RecipeType.CRAFTING)
            {
                CreateRecipeUI(recipe, selectedGroup, craftingMaterialParent, recipe.type);
            }
        }
    }

    private void CreateRecipeUI(Recipe recipe, RecipeItemGroup selectedGroup, Transform parent, Recipe.RecipeType recipeType)
    {

        GameObject recipeUI = Instantiate(itemCraftingUIElement, parent);
        CraftableItemUI itemUI = recipeUI.GetComponent<CraftableItemUI>();

        itemUI.itemCount = selectedGroup.quantityMade;
        itemUI.nameText.text = recipe.recipeName + " x" + itemUI.itemCount.ToString();

        GameObject itemTextPrefab = itemUI.itemText;

        foreach (var recipeItem in selectedGroup.items)
        {
            GameObject spawnedItemText = Instantiate(itemTextPrefab, itemUI.recipeItemParent);
            if (recipeItem.item)
            {
                spawnedItemText.GetComponent<TextMeshProUGUI>().text = recipeItem.item.name + ": " + recipeItem.count;
            }
            else
            {
                spawnedItemText.GetComponent<TextMeshProUGUI>().text = recipeItem.itemName + ": " + recipeItem.count;
            }

        }

        itemUI.craftButton.onClick.AddListener(delegate
        {
            CraftItem(selectedGroup.items, itemUI.itemCount, recipe.recipeName, recipeType, recipe);
        });

    }



    void CraftItem(List<RecipeItemAndCount> itemAndCount, int quantity, string itemName, Recipe.RecipeType recipeType, Recipe recipe)
    {
        if (recipe.craftedItem)
        {
            //gun layer
            if (recipe.craftedItem.layer == 6)
            {
                Transform gunParent = GetComponent<PlayerData>().GetGunParent();
                if (gunParent.childCount == 2)
                {
                    Destroy(gunParent.GetChild(0));
                    ServerInteractManager.Instance.PickUpWeaponServerRpc(recipe.craftedItem.name, NetworkManager.Singleton.LocalClientId);
                }
                else
                {
                    ServerInteractManager.Instance.PickUpWeaponServerRpc(recipe.craftedItem.name, NetworkManager.Singleton.LocalClientId);
                }
            }
        }
        else if (recipeType == Recipe.RecipeType.CONVERT)
        {
            GetComponent<PlayerInterfaceManager>().AddCraftedItemToCollectionBox(itemAndCount, quantity, itemName);
        }
        else
        {
            // Add the crafted item to the player's inventory
            GetComponent<LootHolder>().AddItemServerRpc(itemName, NetworkManager.Singleton.LocalClientId, quantity);

            // Remove required items from the collection box
            var collectionBox = FindFirstObjectByType<CollectionBox>();
            foreach (var item in itemAndCount)
            {
                string name = !string.IsNullOrEmpty(item.itemName) ? item.itemName : (item.item != null ? item.item.name : null);
                if (!string.IsNullOrEmpty(name))
                {
                    collectionBox.RemoveItemServerRpc(name, item.count);
                }
            }
        }
        
        ShowRecipes();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
