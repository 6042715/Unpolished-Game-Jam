using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class Crafter : MonoBehaviour
{
    private FullInventory inventory;
    private Recipes recipe;
    public List<GameObject> items = new List<GameObject>();
    public List<string> itemNames = new List<string>();
    public List<string> realNames = new List<string>();
    public List<string> nameArr = new List<string>();
    private Movement_player _Player;
    public GameObject recipePRholder;
    public GameObject confirmCraftHolder;
    private Button confirmButton;


    private float bobSpeed = 2f;
    private float bobHeight = 0.5f;
    private float baseY;
    private Vector2 baseScale;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _Player = FindFirstObjectByType<Movement_player>();
        inventory = FindFirstObjectByType<FullInventory>();
        recipe = FindFirstObjectByType<Recipes>();

        confirmButton = confirmCraftHolder.GetComponent<Button>();

        baseY = transform.localPosition.y;
        baseScale = transform.localScale;

        confirmButton.onClick.AddListener(ConfirmCraft);
    }

    // void Update()
    // {
    //     if (inventory.craftMode)
    //     {
    //         float newY = baseY + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
    //         transform.localPosition = Vector2.Lerp(transform.position, new Vector2(transform.localPosition.x, newY), Time.deltaTime * 4f);
    //         transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(newY, newY), Time.deltaTime * 2f);
    //     }
    //     else
    //     {
    //         transform.localPosition = new Vector2(transform.localPosition.x, Mathf.Lerp(transform.localPosition.y, baseY, Time.deltaTime * 4f));
    //         transform.localScale = Vector2.Lerp(transform.localScale, baseScale, Time.deltaTime * 2f);
    //     }

    // }

    public void StrippedAdd(GameObject GOB)
    {

        if (inventory.craftMode == false)
        {
            return;
        }
        // if (GOB.transform.childCount > 0)
        // {
        //     foreach (Transform child in GOB.transform)
        //     {
        //         Destroy(child);
        //     }
        // }
        items.Add(GOB);

        int ID = int.Parse(GOB.name);

        int index = 0;
        int i = 0;
        foreach (int id in _Player.inventoryIDs)
        {
            if (id == ID)
            {
                index = i;
                break;
            }
            else
            {
                i++;
            }
        }

        string name = _Player.inventoryNames[index];
        string[] words = name.Split(' ');

        itemNames.Add(name);

        string[] sizeWords = {
            "Miniscule", "Tiny", "Small", "Regular", "Medium",
            "Big", "Large", "Huge", "Enormous", "Gigantic"
        };

        if (Array.Exists(sizeWords, size => words[0].Contains(size)))
        {
            string newName = string.Join(" ", words, 1, words.Length - 1);

            realNames.Add(newName);
            Debug.Log("Trimmed name: " + newName);

            test1();
        }
        else
        {
            realNames.Add(name);
            test1();
        }



    }
    // public void DefaultRemove(GameObject GOB)
    // {
    //     if (inventory.craftMode == true)
    //     {
    //         int indexID = items.IndexOf(GOB);
    //         items.Remove(GOB);
    //         itemNames.RemoveAt(indexID);
    //         realNames.RemoveAt(indexID);
    //     }
    // }
    public void DefaultRemove(GameObject GOB)
    {
        if (inventory.craftMode == true)
        {
            int indexID = items.IndexOf(GOB);
            if (indexID >= 0)
            {
                items.RemoveAt(indexID);
                itemNames.RemoveAt(indexID);
                realNames.RemoveAt(indexID);
            }
            else
            {
                Debug.LogWarning("DefaultRemove called with GameObject not in items list.");
            }
        }
    }

    public void ResetAll()
    {
        items.Clear();
        itemNames.Clear();
        realNames.Clear();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "CrafterRadius")
        {
            inventory.craftMode = true;
            Debug.ClearDeveloperConsole();
            Debug.Log("craft on");
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "CrafterRadius")
        {
            foreach (GameObject item in items)
            {
                item.transform.GetChild(2).GetComponent<ItemClick>().ExternalReset();
            }

            if (inventory.container.transform.childCount > items.Count)
            {
                inventory.ToggleInventory(true);
            }

            ResetAll();
            inventory.craftMode = false;
            Debug.ClearDeveloperConsole();
            Debug.Log("craft off");

        }
    }

    private void test1()
    {
        foreach (Transform child in recipePRholder.transform)
        {
            Destroy(child.gameObject);
        }

        {
            inventory.SetCrafterPreview(null);
            inventory.SetCrafterVisibility(false);
        }

        GameObject Result = recipe.CheckRecipe();

        if (Result != null)
        {
            SpriteRenderer image = Result.GetComponentInChildren<SpriteRenderer>();
            if (image != null)
            {
                inventory.SetCrafterPreview(image.sprite);
                inventory.SetCrafterVisibility(true);

                Result.transform.SetParent(recipePRholder.transform);
                Debug.Log(Result);
            }
            else
            {
                Debug.LogWarning("Result has no SpriteRenderer component.");
                inventory.SetCrafterPreview(null);
                inventory.SetCrafterVisibility(false);
            }
        }
        else
        {
            Debug.LogWarning("No recipe matched in CheckRecipe(). Result is null.");
            inventory.SetCrafterPreview(null);
            inventory.SetCrafterVisibility(false);
        }
    }

    public void ConfirmCraft()
    {
        if(!recipe.foundRecipe){ return; }

        Debug.Log("trying to craft!");
        List<int> selectedIDs = new List<int>();

        foreach (GameObject IDhol in items)
        {
            selectedIDs.Add(int.Parse(IDhol.name));
        }

        RemAfterCraft(selectedIDs);

        GameObject infoHolder = recipePRholder.transform.GetChild(0).GetChild(0).gameObject;
        string CRname = infoHolder.name;

        float weight;
        if (CRname == "Mega Wood")
        {
            weight = 5f;
        }
        else if (CRname == "Empty Lightbulb")
        {
            weight = 0.5f;
        }
        else if (CRname == "Glass Knife")
        {
            weight = 0.6f;
        }
        else if (CRname == "Refined Iron")
        {
            weight = 1f;
        }
        else
        {
            weight = 1f;
        }
        int craftID = UnityEngine.Random.Range(-100000, 100000);
        _Player.AddToInventory(CRname, weight, craftID, infoHolder.GetComponent<SpriteRenderer>().sprite);

        recipe.CheckRecipe();

        if (recipe.foundRecipe == true)
        {
            _Player.AddToInventory(CRname, weight, craftID, infoHolder.GetComponent<SpriteRenderer>().sprite);
        }

        // recipe.CheckRecipe();
        inventory.ToggleInventory(true);
                    
    }

    // public void RemAfterCraft(List<int> IDs)
    // {
    //     foreach (int ID in IDs)
    //     {
    //         //remove from player inventory
    //         int index = 0;
    //         foreach (int plID in _Player.inventoryIDs)
    //         {
    //             if (ID == plID)
    //             {
    //                 Debug.Log(ID);
    //                 break;
    //             }
    //             else
    //             {
    //                 index++;
    //             }
    //         }
    //         int goodIndex = index;
    //         Debug.Log(goodIndex);

    //         //actually removing
    //         //i am slowly losing my sanity
    //         RemoveFromPlayerINV(goodIndex);

    //         //reload the inventory (hopefully this works)
    //         inventory.ToggleInventory(false);
    //         inventory.ToggleInventory(true);
    //     }

    // }
    public void RemAfterCraft(List<int> IDs)
    {
        if (!recipe.foundRecipe) return;

        List<int> indicesToRemove = new List<int>();

        foreach (int ID in IDs)
        {
            int index = _Player.inventoryIDs.IndexOf(ID);
            if (index >= 0)
            {
                indicesToRemove.Add(index);
            }
        }

        indicesToRemove.Sort((a, b) => b.CompareTo(a));

        foreach (int index in indicesToRemove)
        {
            RemoveFromPlayerINV(index);
        }

        inventory.ToggleInventory(false);
        inventory.ToggleInventory(true);
    }


    public void RemoveFromPlayerINV(int index)
    {
        if (!recipe.foundRecipe) { return; }

        _Player.inventoryWeights.RemoveAt(index);
        _Player.inventoryIDs.RemoveAt(index);
        _Player.inventoryNames.RemoveAt(index);
        _Player.inventorySprites.RemoveAt(index);

    }
    
}
