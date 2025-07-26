using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NUnit.Framework;
using System.Collections;

[System.Serializable]
public class Recipe
{
    public List<string> ingredients;
}
public class Recipes : MonoBehaviour
{
    public List<Recipe> recipes = new List<Recipe>();
    public List<string> outputs = new List<string>();
    public bool foundRecipe = false;
    private Crafter crafter;
    private itemTextureHolder textureHolder;
    private FullInventory inventory;
    private Movement_player _Player;
    public GameObject resultHolder;
    private bool ready = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(initComp());
    }

    private IEnumerator initComp()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        crafter = FindFirstObjectByType<Crafter>();
        textureHolder = FindFirstObjectByType<itemTextureHolder>();
        inventory = FindFirstObjectByType<FullInventory>();
        _Player = FindFirstObjectByType<Movement_player>();

        ready = true;

    }

    public GameObject CheckRecipe()
    {
        if(!ready){ return null; }
        // inventory.SetCrafterVisibility(false);

        if (resultHolder.transform.childCount > 0)
        {
            foreach (Transform child in resultHolder.transform)
            {
                Destroy(child.gameObject);
            }
        }
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

                    Result.transform.SetParent(resultHolder.transform);
                    return Result;
                }
                foundRecipe = false;
            }
            foundRecipe = false;
        }

        foundRecipe = false;
        return null;
    }
}
