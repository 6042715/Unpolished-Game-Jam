using System;
using System.Collections;
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
    public GameObject crafterPreview;

    public bool isOpen = false;
    public List<GameObject> children = new List<GameObject>();
    [SerializeField] private List<GameObject> detailGOBs = new List<GameObject>();
    private Movement_player TSPlayer;
    private Recipes recipes;
    private itemTextureHolder textureHolder;
    private RectTransform rect;
    private RectTransform ENrect;
    private GridLayoutGroup grid;

    private Image previewIMG;
    private TextMeshProUGUI previewName;
    private TextMeshProUGUI previewWeight;
    private TextMeshProUGUI preiewID;
    private TextMeshProUGUI previewMix;
    private Image crafterPreviewIMG;
    private ShowRecipes showRecipes;
    private GameManager game;
    private AudioSource audioSource;
    private ShopStand shopStand;

    public Sprite emptySprite;

    public bool craftMode = false;

    private Color tranColor = new Color(1f, 1f, 1f, 0f);
    private Color defColor = new Color(1f, 1f, 1f, 1f);

    public AudioClip clickSound;
    public AudioClip craftSound;
    public AudioClip craftOn;
    public AudioClip craftOff;
    public AudioClip buyItem;
    public AudioClip woodTap;
    public AudioClip ballTap;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TSPlayer = FindFirstObjectByType<Movement_player>();
        crafter = FindFirstObjectByType<Crafter>();
        recipes = FindFirstObjectByType<Recipes>();
        textureHolder = FindFirstObjectByType<itemTextureHolder>();
        game = FindFirstObjectByType<GameManager>();
        shopStand = FindFirstObjectByType<ShopStand>();

        showRecipes = game.recipeShowHolder.GetComponent<ShowRecipes>();

        rect = container.GetComponent<RectTransform>();
        ENrect = GetComponent<RectTransform>();
        grid = container.GetComponent<GridLayoutGroup>();
        crafterPreviewIMG = crafterPreview.GetComponent<Image>();
        audioSource = GetComponent<AudioSource>();
        

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

        ToggleInventory(false);

        previewIMG.sprite = emptySprite;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {

            if (!isOpen)
            {
                ToggleInventory(true);
                showRecipes.LoadRecipes(false);

                previewIMG.color = tranColor;
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
        // if(shopStand.sellingMode == true || shopStand.isStoreOpen == true){ return;  }
        if (!toggle && (shopStand.sellingMode || shopStand.isStoreOpen)) return;

        crafter.ResetAll();
        recipes.CheckRecipe();

        foreach (GameObject child in children)
        {
            crafter.ResetAll();

            if (child.name != "SellValueHolder")
            {
                child.SetActive(toggle);
            }
        }
        if (toggle)
        {
            foreach (Transform child in container.transform)
            {
                Destroy(child.gameObject);
            }
            StartCoroutine(loadTiles());


        }
    }

    public void ShowDetails(Sprite TSsprite, string TSname, float weight, int ID)
    {
        previewIMG.color = defColor;

        previewIMG.sprite = TSsprite;
        previewName.text = TSname;
        previewWeight.text = ((float)MathF.Round(weight, 2)).ToString() + "KG";
        preiewID.text = ID.ToString();

    }

    public void SetCrafterPreview(Sprite sprite)
    {
        crafterPreviewIMG.sprite = sprite;
    }
    public void SetCrafterVisibility(bool toggle)
    {
        crafterPreviewIMG.enabled = toggle;
    }

    private IEnumerator loadTiles()
    {
        int i = 0;
        foreach (int ID in TSPlayer.inventoryIDs)
        {
            GameObject entry2 = Instantiate(entry);

            entry2.name = ID.ToString();

            entry2.transform.SetParent(container.transform);

            entry2.GetComponentInChildren<Image>().sprite = TSPlayer.inventorySprites[i];
            entry2.GetComponentInChildren<TextMeshProUGUI>().text = TSPlayer.inventoryNames[i];

            i++;

            if (i % 3 == 0)
            {
                yield return new WaitForEndOfFrame();
                game.toggleDebugLight(1);
            }

        }
        int rows = Mathf.CeilToInt(TSPlayer.inventoryIDs.Count / 4f);
        Debug.Log(rows);

        rect.sizeDelta = new Vector2(rect.sizeDelta.x, rows * grid.cellSize.y);
    }

    public void PlayUIsound(AudioClip audioClip, bool randomPitch = false)
    {
        audioSource.pitch = 1f;

        if (randomPitch)
        {
            audioSource.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
        }
        audioSource.PlayOneShot(audioClip);
    }
}
