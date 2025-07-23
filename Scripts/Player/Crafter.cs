using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using System.Collections;
using System;

public class Crafter : MonoBehaviour
{
    private FullInventory inventory;
    public List<GameObject> items = new List<GameObject>();
    public List<string> itemNames = new List<string>();
    public List<string> realNames = new List<string>();
    public List<string> nameArr = new List<string>();
    private Movement_player _Player;


    private float bobSpeed = 2f;
    private float bobHeight = 0.5f; 
    private float baseY;
    private Vector2 baseScale;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _Player = FindFirstObjectByType<Movement_player>();
        inventory = FindFirstObjectByType<FullInventory>();

        baseY = transform.localPosition.y;
        baseScale = transform.localScale;
    }

    void Update()
    {
        if (inventory.craftMode)
        {
            float newY = baseY + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.localPosition = Vector2.Lerp(transform.position, new Vector2(transform.localPosition.x, newY), Time.deltaTime * 4f);
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(newY, newY), Time.deltaTime * 2f);
        }
        else
        {
            transform.localPosition = new Vector2(transform.localPosition.x, Mathf.Lerp(transform.localPosition.y, baseY, Time.deltaTime * 4f));
            transform.localScale = Vector2.Lerp(transform.localScale, baseScale, Time.deltaTime * 2f);
        }
    
    }

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
        }



    }
    public void DefaultRemove(GameObject GOB)
    {
        if (inventory.craftMode == true)
        {
            int indexID = items.IndexOf(GOB);
            items.Remove(GOB);
            itemNames.RemoveAt(indexID);
            realNames.RemoveAt(indexID);
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
    
}
