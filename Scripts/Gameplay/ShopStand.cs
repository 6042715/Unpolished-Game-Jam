using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopStand : MonoBehaviour
{
    public bool isStoreOpen = false;
    public bool sellingMode = false;
    public bool hasChosenMode = false;
    public GameObject storeHolder;
    public GameObject valueUIholder;
    private FullInventory fullInventory;
    private Movement_player _Player;
    private TextMeshProUGUI valueUITMP;
    private TextMeshProUGUI buttonText;
    private Button confirmButtom;
    private Button returnButton;
    private float lastMoney;
    private Crafter crafter;

    public GameObject itemGOb;
    public GameObject itemHolder;
    public GameObject buyScreenHolder;
    public GameObject itemInfoHolder;

    private TextMeshProUGUI itemNameGUI;
    private TextMeshProUGUI itemStatsGUI;
    private Image itemSpriteGUI;

    [SerializeField] private List<Transform> defaultUI = new List<Transform>();
    [SerializeField] private List<int> selectedIDs = new List<int>();

    public List<Sprite> sellValueSprites = new List<Sprite>();
    public List<Sprite> oreSprites;
    public List<float> sellValues = new List<float>();
    public float totalSellValue = 0f;
    public GameObject selectedShopItem;
    public GameObject buttonShopHolder;
    private Button buyButton;
    private Button returnButtonShop;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fullInventory = FindFirstObjectByType<FullInventory>();
        _Player = FindFirstObjectByType<Movement_player>();
        crafter = FindFirstObjectByType<Crafter>();

        valueUITMP = valueUIholder.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        buttonText = valueUIholder.transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        confirmButtom = valueUIholder.transform.GetChild(1).GetComponent<Button>();
        returnButton = valueUIholder.transform.GetChild(2).GetComponent<Button>();

        buyButton = buttonShopHolder.transform.GetChild(0).GetComponent<Button>();
        returnButtonShop = buttonShopHolder.transform.GetChild(1).GetComponent<Button>();

        itemNameGUI = itemInfoHolder.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        itemStatsGUI = itemInfoHolder.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        itemSpriteGUI = itemInfoHolder.transform.GetChild(2).GetComponent<Image>();

        buyButton.onClick.AddListener(BuyShopItem);

        lastMoney = _Player.playerMoney;

        if (storeHolder != null)
        {
            int i = 0;
            foreach (Transform child in storeHolder.transform)
            {
                defaultUI.Add(child);

                if (i == 1)
                {
                    child.gameObject.name = "BuyButton";
                    child.gameObject.GetComponent<Button>().onClick.AddListener(SetBuyMode);
                }
                else if (i == 2)
                {
                    child.gameObject.name = "SellButton";
                    child.gameObject.GetComponent<Button>().onClick.AddListener(SetSellMode);
                }

                i++;
            }

            storeHolder.SetActive(false);
            valueUIholder.SetActive(false);
            buyScreenHolder.SetActive(false);
        }

        confirmButtom.onClick.AddListener(ConfirmSell);
        returnButton.onClick.AddListener(ResetSellMode);
        returnButtonShop.onClick.AddListener(CloseBuyScreen);

        ShowStatsOnScreen("", 6969, null, null);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "CrafterRadius")
        {
            isStoreOpen = true;
            storeHolder.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "CrafterRadius")
        {
            isStoreOpen = false;
            sellingMode = false;

            fullInventory.ToggleInventory(false);

            storeHolder.SetActive(false);
            valueUIholder.SetActive(false);
            buyScreenHolder.SetActive(false);
        }
    }


    private void SetBuyMode()
    {
        SetStoreMode(1);
        fullInventory.PlayUIsound(fullInventory.woodTap);
    }
    private void SetSellMode()
    {
        SetStoreMode(2);
        fullInventory.PlayUIsound(fullInventory.woodTap);
    }
    private void CloseBuyScreen()
    {
        SetStoreMode(0);
        fullInventory.PlayUIsound(fullInventory.woodTap);
    }
    private void ResetSellMode()
    {
        sellingMode = false;
        fullInventory.ToggleInventory(false);
        ResetSellValues();
        SetStoreMode(0);
    }



    public void SetStoreMode(int type = 0)
    {
        if (type == 0)
        {
            storeHolder.SetActive(true);
            valueUIholder.SetActive(false);
            buyScreenHolder.SetActive(false);

            isStoreOpen = false;
            fullInventory.ToggleInventory(false);
            isStoreOpen = true;

            ResetSellValues();
        }
        else if (type == 1)
        {
            storeHolder.SetActive(false);
            valueUIholder.SetActive(false);
            buyScreenHolder.SetActive(true);

            fullInventory.ToggleInventory(false);

            ResetSellValues();
        }
        else if (type == 2)
        {
            lastMoney = _Player.playerMoney;
            storeHolder.SetActive(false);
            buyScreenHolder.SetActive(false);

            isStoreOpen = false;
            fullInventory.ToggleInventory(true);
            isStoreOpen = true;

            valueUIholder.SetActive(true);

            sellingMode = true;
        }
    }

    public void CalculateValue(Sprite sprite, float weight, bool mode, int ID)
    {

        int index = 0;
        foreach (Sprite TSsprite in sellValueSprites)
        {
            if (TSsprite == sprite)
            {
                float TSvalue = sellValues[index];
                float TStotalValue;

                if (oreSprites.Contains(TSsprite))
                {
                    Debug.Log("item is ore");
                    TStotalValue = TSvalue * weight;
                }
                else
                {
                    TStotalValue = TSvalue;
                }


                Debug.Log("This item cost: " + TStotalValue);

                if (mode == true)
                {
                    ChangeTotalValue(TStotalValue);
                    selectedIDs.Add(ID);
                }
                else
                {
                    ChangeTotalValue(-TStotalValue);
                    selectedIDs.Remove(ID);
                }
                break;
            }

            index++;
        }
    }

    public void ChangeTotalValue(float value)
    {
        if (sellingMode)
        {
            totalSellValue += value;

            valueUITMP.text = MathF.Round(totalSellValue, 2).ToString();
            buttonText.text = MathF.Round(lastMoney, 2).ToString() + "-->" + MathF.Round(totalSellValue, 2).ToString();
            Debug.Log("Total sell value: " + MathF.Round(totalSellValue, 2));
        }
    }

    private void ConfirmSell()
    {
        Debug.Log("trying to confirm sell!");
        if (MathF.Round(totalSellValue, 2) > 0)
        {
            Debug.Log("Confirmed sell!");

            crafter.RemAfterCraft(selectedIDs, true);
            _Player.playerMoney += totalSellValue;
            _Player.UpdateMoneyDisplay(totalSellValue);

            ResetSellValues();

            sellingMode = false;
            fullInventory.ToggleInventory(true);

            SetStoreMode(0);
        }
        else
        {
            ResetSellValues();
            SetStoreMode(0);
        }
    }

    private void ResetSellValues()
    {
        selectedIDs.Clear();
        totalSellValue = 0f;
        valueUITMP.text = "0";
    }

    public void GenerateItemsInShop(List<Sprite> sprites, List<string> names)
    {
        int index = 0;

        foreach (Sprite TSsprite in sprites)
        {
            GameObject TSitem = Instantiate(itemGOb);
            TSitem.transform.SetParent(itemHolder.transform);

            float price = UnityEngine.Random.Range(150, 800);

            TSitem.name = names[index];
            TSitem.transform.GetChild(0).GetComponent<Image>().sprite = TSsprite;
            TSitem.transform.GetChild(0).name = price.ToString();

            TSitem.transform.GetChild(0).transform.rotation = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(-360f, 360f));

            index++;
        }
    }

    public void ShowStatsOnScreen(string name, float cost, Sprite sprite, GameObject gameObject)
    {
        
        itemNameGUI.text = "-" + name + "-";
        itemStatsGUI.text = "$" + cost.ToString();
        itemSpriteGUI.sprite = sprite;
        itemSpriteGUI.color = Color.white;

        if (cost == 6969f)
        {
            itemStatsGUI.text = "";
            itemNameGUI.text = "";
            itemSpriteGUI.sprite = null;
            itemSpriteGUI.color = Color.clear;
        }


        selectedShopItem = gameObject;
    }

    public void BuyShopItem()
    {
        if(selectedShopItem == null){ return; }

        float thisPrice = float.Parse(selectedShopItem.transform.GetChild(0).name);

        if (thisPrice < _Player.playerMoney)
        {
            int id = UnityEngine.Random.Range(-100000, 1000000);
            _Player.AddToInventory(selectedShopItem.name, 1f, id, selectedShopItem.transform.GetChild(0).GetComponent<Image>().sprite);

            Destroy(selectedShopItem);
            _Player.playerMoney -= thisPrice;
            _Player.UpdateMoneyDisplay(-thisPrice);

            fullInventory.PlayUIsound(fullInventory.buyItem);
            ShowStatsOnScreen("", 6969, null, null);
        }
    }
}
