using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemClick : MonoBehaviour
{
    private Button button;
    public FullInventory inventory;
    private Inventory inventory2;
    public Crafter crafter;
    public Recipes recipes;
    public ShopStand shopStand;
    public Movement_player _Player;
    private TextMeshProUGUI uGUI;
    private Image image;
    private bool craftSelected = false;
    private bool sellSelected = false;
    private Color defaultColor;

    private Sprite thisSprite;
    private float thisWeight;
    private int thisID;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventory = FindFirstObjectByType<FullInventory>();
        _Player = FindFirstObjectByType<Movement_player>();
        crafter = FindFirstObjectByType<Crafter>();
        recipes = FindAnyObjectByType<Recipes>();
        shopStand = FindFirstObjectByType<ShopStand>();

        button = GetComponent<Button>();
        uGUI = transform.parent.GetComponentInChildren<TextMeshProUGUI>();
        image = transform.parent.GetComponentInChildren<Image>();

        button.onClick.AddListener(ShowInInventory);
        button.onClick.AddListener(AddToCrafter);
        button.onClick.AddListener(AddToSeller);

        defaultColor = uGUI.color;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void ShowInInventory()
    {
        if (!(_Player.inventoryIDs.Count == _Player.inventoryNames.Count) || !(_Player.inventorySprites.Count == _Player.inventoryWeights.Count)) { return; }

        List<int> IDS = _Player.inventoryIDs;

        thisID = int.Parse(transform.parent.gameObject.name);

        // int i = 0;
        // int thisIndex = 0;
        // foreach (int ID in IDS)
        // {
        //     if (ID == ThisID)
        //     {
        //         thisIndex = i;

        //         Debug.Log(thisIndex);
        //         break;
        //     }
        //     else
        //     {
        //         i++;
        //     }

        // }
        int thisIndex = _Player.inventoryIDs.IndexOf(thisID);
        if (thisIndex < 0) return;

        thisSprite = _Player.inventorySprites[thisIndex];
        string name = _Player.inventoryNames[thisIndex];
        thisWeight = _Player.inventoryWeights[thisIndex];

        Debug.Log(thisSprite.name + " " + name + " " + thisWeight);
        inventory.ShowDetails(thisSprite, name, thisWeight, thisID);

        inventory.PlayUIsound(inventory.clickSound);

    }

    private void AddToCrafter()
    {
        if (inventory.craftMode == false)
        {
            return;
        }

        inventory.SetCrafterVisibility(false);
        if (!craftSelected)
        {
            uGUI.color = Color.coral;
            image.color = Color.coral;

            crafter.StrippedAdd(gameObject.transform.parent.gameObject);

            craftSelected = true;

            recipes.CheckRecipe();
        }
        else
        {
            uGUI.color = defaultColor;
            image.color = Color.white;

            crafter.DefaultRemove(gameObject.transform.parent.gameObject);

            craftSelected = false;

            recipes.CheckRecipe();
        }
    }

    private void AddToSeller()
    {
        if (shopStand.sellingMode == false)
        {
            return;
        }

        if (!sellSelected)
        {
            uGUI.color = Color.lightGreen;
            image.color = Color.green;

            shopStand.CalculateValue(thisSprite, thisWeight, true, thisID);
            sellSelected = true;
        }
        else
        {
            uGUI.color = defaultColor;
            image.color = Color.white;

            shopStand.CalculateValue(thisSprite, thisWeight, false, thisID);
            sellSelected = false;
        }
        

    }

    public void ExternalReset()
    {
        uGUI.color = defaultColor;
        image.color = Color.white;

        craftSelected = false;

        //insert cool ass emoji with sunglasses smiling with his thumbs up
    }
}
