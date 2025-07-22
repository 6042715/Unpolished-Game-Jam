using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using System.Collections;
using System;

public class Crafter : MonoBehaviour
{
    public List<GameObject> items = new List<GameObject>();
    public List<string> itemNames = new List<string>();
    public List<string> realNames = new List<string>();
    public List<string> nameArr = new List<string>();
    private Movement_player _Player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _Player = FindFirstObjectByType<Movement_player>();
    }

    public void StrippedAdd(GameObject GOB)
    {
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
        int indexID = items.IndexOf(GOB);
        items.Remove(GOB);
        itemNames.RemoveAt(indexID);
        realNames.RemoveAt(indexID);
    }
    public void ResetAll()
    {
        items.Clear();
        itemNames.Clear();
        realNames.Clear();
    }
}
