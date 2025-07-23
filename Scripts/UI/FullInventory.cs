using System;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class FullInventory : MonoBehaviour
{
    private Crafter crafter;

    public GameObject entry;
    public GameObject container;
    public GameObject details;

    public bool isOpen = false;
    public List<GameObject> children = new List<GameObject>();
    [SerializeField] private List<GameObject> detailGOBs = new List<GameObject>();
    private Movement_player TSPlayer;
    private RectTransform rect;
    private RectTransform ENrect;
    private GridLayoutGroup grid;

    private Image previewIMG;
    private TextMeshProUGUI previewName;
    private TextMeshProUGUI previewWeight;
    private TextMeshProUGUI preiewID;

    public bool craftMode = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TSPlayer = FindFirstObjectByType<Movement_player>();
        crafter = FindFirstObjectByType<Crafter>();

        rect = container.GetComponent<RectTransform>();
        ENrect = GetComponent<RectTransform>();
        grid = container.GetComponent<GridLayoutGroup>();

        foreach (Transform child in transform)
        {
            children.Add(child.gameObject);
        }

        foreach (Transform child in details.transform)
        {
            detailGOBs.Add(child.gameObject);
        }

        previewIMG = detailGOBs[0].GetComponent<Image>();
        previewName = detailGOBs[1].GetComponent<TextMeshProUGUI>();
        previewWeight = detailGOBs[2].GetComponent<TextMeshProUGUI>();
        preiewID = detailGOBs[3].GetComponent<TextMeshProUGUI>();

        if(!isOpen){ ToggleInventory(false); }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isOpen)
            {
                ToggleInventory(true);
                isOpen = true;
            }
            else
            {
                ToggleInventory(false);
                isOpen = false;
            }
        }

        if (craftMode)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (crafter.items.Count > 0)
                {
                    foreach (GameObject item in crafter.items)
                    {
                        item.transform.GetChild(2).GetComponent<ItemClick>().ExternalReset();
                    }
                    crafter.ResetAll();
                }
            }
        }
    }

    public void ToggleInventory(bool toggle)
    {
        foreach (GameObject child in children)
        {
            crafter.ResetAll();
            child.SetActive(toggle);
        }
        if (toggle)
        {
            foreach (Transform child in container.transform)
            {
                Destroy(child.gameObject);
            }

            int i = 0;
            foreach (int ID in TSPlayer.inventoryIDs)
            {
                GameObject entry2 = Instantiate(entry);

                entry2.name = ID.ToString();

                entry2.transform.SetParent(container.transform);

                entry2.GetComponentInChildren<Image>().sprite = TSPlayer.inventorySprites[i];
                entry2.GetComponentInChildren<TextMeshProUGUI>().text = TSPlayer.inventoryNames[i];

                i++;

            }
            int rows = Mathf.CeilToInt(TSPlayer.inventoryIDs.Count / 4f);
            Debug.Log(rows);

            rect.sizeDelta = new Vector2(rect.sizeDelta.x, rows * grid.cellSize.y);

        }
    }

    public void ShowDetails(Sprite TSsprite, string TSname, float weight, int ID)
    {

        previewIMG.sprite = TSsprite;
        previewName.text = TSname;
        previewWeight.text = ((float)MathF.Round(weight, 2)).ToString() + "KG";
        preiewID.text = ID.ToString();

    }
}
