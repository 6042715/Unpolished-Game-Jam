using System;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public GameObject container;
    public GameObject inventEntry;
    public GameObject scroll;
    private RectTransform rectTransform;
    private Movement_player playerInfo;
    private ScrollRect SCrect;
    [SerializeField] private int lastInventoryEntries;
    [SerializeField] private List<GameObject> curInvenEntries = new List<GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInfo = FindFirstObjectByType<Movement_player>();

        rectTransform = container.GetComponent<RectTransform>();
        SCrect = scroll.GetComponent<ScrollRect>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RefreshInventory()
    {
        int curEntries = playerInfo.inventoryIDs.Count;

        for (int i = lastInventoryEntries; i < curEntries; i++)
        {
            GameObject entry = Instantiate(inventEntry);
            entry.transform.SetParent(container.transform);

            entry.GetComponentInChildren<TextMeshProUGUI>().text = playerInfo.inventoryNames[i] + "\n (" + ((float)Math.Round(playerInfo.inventoryWeights[i], 2)).ToString() + "KG)";
            entry.GetComponentInChildren<Image>().sprite = playerInfo.inventorySprites[i];

            curInvenEntries.Add(entry);
            ScrollToBottom(SCrect);
        }
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, curInvenEntries.Count * curInvenEntries[0].GetComponent<RectTransform>().sizeDelta.y + curInvenEntries[0].GetComponent<RectTransform>().sizeDelta.y);
        Debug.Log(rectTransform.sizeDelta.y);

        lastInventoryEntries = curEntries;
    }
    public void ScrollToBottom(ScrollRect scrollRect)
    {
        scrollRect.normalizedPosition = new Vector2(0f, 0f);
    }
    
}
