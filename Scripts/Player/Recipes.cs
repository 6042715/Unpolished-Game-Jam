using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NUnit.Framework;

[System.Serializable]
public class Recipe
{
    public List<string> ingredients;
}
public class Recipes : MonoBehaviour
{
    public List<Recipe> recipes = new List<Recipe>();
    public List<string> outputs = new List<string>();
    [SerializeField] private bool foundRecipe = false;
    private Crafter crafter;
    private itemTextureHolder textureHolder;
    private FullInventory inventory;
    private Movement_player _Player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        crafter = FindFirstObjectByType<Crafter>();
        textureHolder = FindFirstObjectByType<itemTextureHolder>();
        inventory = FindFirstObjectByType<FullInventory>();
        _Player = FindFirstObjectByType<Movement_player>();
    }

    public GameObject CheckRecipe()
    {
        inventory.SetCrafterVisibility(false);

        for (int recipeIndex = 0; recipeIndex < recipes.Count; recipeIndex++)
        {
            Recipe recipe = recipes[recipeIndex];

            if (recipe.ingredients.Count == crafter.realNames.Count)
            {
                if (recipe.ingredients.OrderBy(x => x).SequenceEqual(crafter.realNames.OrderBy(x => x)))
                {
                    foundRecipe = true;

                    int previewID = UnityEngine.Random.Range(100, 1000);
                    GameObject Result = new GameObject(previewID.ToString());

                    GameObject spriteHolder = new GameObject(outputs[recipeIndex]);
                    SpriteRenderer ResultSP = spriteHolder.AddComponent<SpriteRenderer>();

                    ResultSP.sprite = textureHolder.craftingSprites[recipeIndex];
                    ResultSP.enabled = false;

                    spriteHolder.transform.SetParent(Result.transform);

                    inventory.SetCrafterVisibility(true);
                    return Result;
                }
            }
        }

        foundRecipe = false;
        return null;
    }
}
