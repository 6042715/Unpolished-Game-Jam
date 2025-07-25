using TMPro;
using UnityEngine;

public class ShowRecipes : MonoBehaviour
{
    public GameObject textHolderGOB;
    public GameObject scrollRectGOB;
    private TextMeshProUGUI proUGUI;
    private RectTransform rectTransform;

    private Recipes recipes;
    private FullInventory fullInventory;
    private bool active = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        proUGUI = textHolderGOB.GetComponent<TextMeshProUGUI>();
        recipes = FindFirstObjectByType<Recipes>();
        fullInventory = FindFirstObjectByType<FullInventory>();

        rectTransform = scrollRectGOB.GetComponent<RectTransform>();

        gameObject.SetActive(false);
    }

    public void LoadRecipes(bool toggle = true)
    {
        if (active || toggle == false)
        {
            proUGUI.text = "";
            active = false;
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);
        fullInventory.ToggleInventory(false);

        int index = 0;
        proUGUI.text = "";

        foreach (Recipe recipe in recipes.recipes)
        {
            proUGUI.text += recipes.outputs[index] + ": \n";
            foreach (string ingredient in recipe.ingredients)
            {
                proUGUI.text += "--" + ingredient + "\n";
            }
            proUGUI.text += "\n";
            index++;
        }

        active = true;
    }



}
