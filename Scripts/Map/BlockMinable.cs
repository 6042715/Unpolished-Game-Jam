using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Variants
{
    stone,
    copper,
    iron,
    gold,
    diamond,
    wood
}
public class BlockMinable : MonoBehaviour
{
    public Variants variant;
    public List<Sprite> oreImages = new List<Sprite>();
    [SerializeField] private GameObject player;
    private Movement_player playerSCR;
    private Color color;
    private SpriteRenderer SPrend;
    private bool CanBeMined = false;
    private Color canBeMinedCol;
    private Color defaultCol;
    private Collider2D col2;
    private IEnumerator isBeingMined;
    private float timeToMine = 1;
    private string TSname;
    private float minWeight;
    private float maxWeight;
    private Sprite oreImage;
    private GenerateMap genMap;
    [SerializeField] private float weight;
    [SerializeField] private int thisID;
    [SerializeField] private string weightName;
    public bool mining = false;
    public GameObject thisBlock;
    public bool aboutToBreak = false;
    private bool isPlayerNear;
    private bool shouldRender = false;
    private itemTextureHolder textureHolder;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        SPrend = GetComponent<SpriteRenderer>();
        if (variant == Variants.stone)
        {
            SPrend.color = new Color(0.5f, 0.5f, 0.5f);
            TSname = "Cobblestone";

            minWeight = 0.50f;
            maxWeight = 3.00f;

            oreImage = oreImages[0];
        }
        else if (variant == Variants.copper)
        {
            SPrend.color = new Color(0.831f, 0.478f, 0f);
            timeToMine = 1.5f;
            TSname = "Copper Ore";

            minWeight = 0.10f;
            maxWeight = 2.50f;

            oreImage = oreImages[1];
        }
        else if (variant == Variants.iron)
        {
            SPrend.color = new Color(1f, 1f, 1f);
            TSname = "Iron Ore";

            minWeight = 0.10f;
            maxWeight = 2.00f;

            oreImage = oreImages[2];
        }
        else if (variant == Variants.diamond)
        {
            SPrend.color = new Color(0.11f, 0.992f, 1f);
            TSname = "Rough Diamond";

            minWeight = 0.05f;
            maxWeight = 0.4f;

            oreImage = oreImages[4];
        }
        else if (variant == Variants.gold)
        {
            SPrend.color = new Color(0.961f, 1f, 0.118f);
            TSname = "Gold Ore";

            minWeight = 0.25f;
            maxWeight = 1.50f;

            oreImage = oreImages[3];
        }
        else if (variant == Variants.wood)
        {
            timeToMine = 0.6f;
            TSname = "Wood";

            minWeight = 0.25f;
            maxWeight = 1f;

            oreImage = oreImages[5];
        }
    }
    void Start()
    {
        thisID = UnityEngine.Random.Range(1000, 1000000);


        gameObject.name = thisID.ToString();
        player = GameObject.FindGameObjectWithTag("Player");

        genMap = FindFirstObjectByType<GenerateMap>();

        playerSCR = FindFirstObjectByType<Movement_player>();

        thisBlock = gameObject;

        weight = UnityEngine.Random.Range(minWeight, maxWeight);

        textureHolder = FindFirstObjectByType<itemTextureHolder>();

        if (weight < 0.1f)
        {
            weightName = "Miniscule";
        }
        else if (weight < 0.2f)
        {
            weightName = "Tiny";
        }
        else if (weight < 0.3f)
        {
            weightName = "Small";
        }
        else if (weight < 0.6f)
        {
            weightName = "Regular";
        }
        else if (weight < 1.3f)
        {
            weightName = "Medium";
        }
        else if (weight < 1.4f)
        {
            weightName = "Big";
        }
        else if (weight < 1.8f)
        {
            weightName = "Large";
        }
        else if (weight < 2f)
        {
            weightName = "Huge";
        }
        else if (weight < 2.5f)
        {
            weightName = "Enormous";
        }
        else if (weight < 3f)
        {
            weightName = "Gigantic";
        }

        col2 = GetComponent<Collider2D>();
        canBeMinedCol = new Color(SPrend.color.r, SPrend.color.g, SPrend.color.b, 0.5f);
        defaultCol = SPrend.color;

        // isBeingMined = IsBeingMined();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isPlayerNear)
        {
            if (!SPrend.enabled)
            {
                SPrend.enabled = true;
                Debug.Log("Renderer of block: " + gameObject.name + " enabled!");
            }
            float distance = Vector2.Distance(player.transform.position, transform.position);
            if (distance <= 3f)
            {
                CanBeMined = true;
            }
            else
            {
                CanBeMined = false;
            }

            if (CanBeMined && SPrend.color != canBeMinedCol && !mining)
            {
                SPrend.color = canBeMinedCol;
            }
            else if (!CanBeMined && SPrend.color != defaultCol)
            {
                SPrend.color = defaultCol;
            }

            if (CanBeMined)
            {
                if (IsTouchingMouse() && Input.GetMouseButton(0))
                {
                    if (!mining)
                    {
                        StartCoroutine(IsBeingMined());
                    }
                }
                else
                {
                    mining = false;
                }
            }
        }
        else if (!shouldRender)
        {
            SPrend.enabled = false;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        isPlayerNear = true;
        Debug.Log("Collided with collider: " + collision.gameObject.name);

        if (collision.gameObject.name == "RenderColliderCheck")
        {
            shouldRender = true;
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        isPlayerNear = false;

        if (collision.gameObject.name == "RenderColliderCheck")
        {
            shouldRender = false;
        }

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            CanBeMined = true;
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            CanBeMined = false;
        }
    }
    private bool IsTouchingMouse()
    {
        Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return gameObject.GetComponent<Collider2D>().OverlapPoint(point);
    }

    IEnumerator IsBeingMined()
    {
        mining = true;
        float i = 0;
        int j = 0;

        float timesI = timeToMine / 0.1f;
        float percentDone;
        Debug.Log(timesI);

        while (mining)
        {
            if (Input.GetMouseButton(0))
            {
                if (i > timeToMine)
                {

                    if (TSname == "Wood")
                    {
                        if (gameObject.transform.childCount > 0)
                        {
                            Transform childTR = gameObject.transform.GetChild(0);
                            GameObject child = childTR.gameObject;

                            string childName = child.name;

                            if (childName.Contains("Table"))
                            {
                                string lastLetter = childName.Substring(childName.Length - 1);
                                Debug.Log("Table type: " + lastLetter);

                                int itemID = UnityEngine.Random.Range(-100000, -100);

                                AudioSource houseSource = child.transform.parent.transform.parent.GetComponent<AudioSource>();
                                houseSource.pitch = UnityEngine.Random.Range(0.8f, 1.2f);

                                houseSource.PlayOneShot(textureHolder.audioClips[0]);

                                if (lastLetter == "1")
                                {
                                    playerSCR.AddToInventory("" + "Red wine", 0.33f, itemID, textureHolder.itemTextures[0]);
                                }
                                else if (lastLetter == "2")
                                {
                                    playerSCR.AddToInventory("" + "Peas", 0.2f, itemID, textureHolder.itemTextures[1]);
                                }
                                else if (lastLetter == "3")
                                {
                                    playerSCR.AddToInventory("" + "Cat", 2f, itemID, textureHolder.itemTextures[2]);
                                }
                                else if (lastLetter == "4")
                                {
                                    playerSCR.AddToInventory("" + "RAM", 0.1f, itemID, textureHolder.itemTextures[3]);
                                }
                            }
                            else if (childName.Contains("Lamp"))
                            {
                                child.transform.SetParent(gameObject.transform.parent);

                                string lastLetter = childName.Substring(childName.Length - 1);

                                int itemID = UnityEngine.Random.Range(-100000, -1000);

                                int RandomNumber = UnityEngine.Random.Range(0, 50);

                                if (lastLetter == "0" && RandomNumber < 25)
                                {
                                    playerSCR.AddToInventory("Metal Scrap", RandomWeight(0.1f, 0.5f), itemID, textureHolder.itemTextures[4]);
                                }
                                else if (lastLetter == "1" && RandomNumber < 12)
                                {
                                    playerSCR.AddToInventory("* Copper Wire", RandomWeight(0.2f, 0.4f), itemID, textureHolder.itemTextures[5]);
                                }
                                else if (lastLetter == "2" && RandomNumber < 12)
                                {
                                    playerSCR.AddToInventory("* Black Ink", RandomWeight(0.1f, 0.4f), itemID, textureHolder.itemTextures[6]);
                                }
                                else if (lastLetter == "3" && RandomNumber == 21)
                                {
                                    playerSCR.AddToInventory("** White Ink", RandomWeight(0.1f, 0.4f), itemID, textureHolder.itemTextures[7]);
                                }
                            }
                        }
                    }

                    playerSCR.AddToInventory(weightName + " " + TSname, weight, thisID, oreImage);

                    if (TSname == "Copper Ore" || TSname == "Iron Ore" || TSname == "Gold Ore")
                    {
                        if (UnityEngine.Random.Range(0, 50) == 3)
                        {
                            int itemID = UnityEngine.Random.Range(1, 1000);
                            playerSCR.AddToInventory("* Asteroid", RandomWeight(1f, 3.5f), itemID, textureHolder.itemTextures[8]);
                        }
                    }
                    if (TSname == "Gold Ore" || TSname == "Rough Diamond")
                    {
                        int treasureChance = UnityEngine.Random.Range(0, 100);

                        if (treasureChance == 2)
                        {
                            int itemID = UnityEngine.Random.Range(1, 1000);
                            playerSCR.AddToInventory("*** Emerald Skull", RandomWeight(1f, 2f), itemID, textureHolder.itemTextures[9]);
                        }
                        else if (treasureChance == 3)
                        {
                            int itemID = UnityEngine.Random.Range(1, 1000);
                            playerSCR.AddToInventory("*** Ruby Heart", RandomWeight(1f, 2f), itemID, textureHolder.itemTextures[10]); 
                        }
                    }

                    genMap.playRockNoise();
                    mining = false;

                    Destroy(gameObject);
                    yield break;
                }

                i += 0.1f * playerSCR.mineSpeed;
                j += 1;

                percentDone = 100 - (-((j - timesI) / timesI) * 100);
                Debug.Log(percentDone);

                SPrend.color = new Color(SPrend.color.r, SPrend.color.g, SPrend.color.b, 1f - (percentDone / 100f));
                Debug.Log(percentDone / 100);
                yield return new WaitForSecondsRealtime(0.1f);
            }
            else
            {
                mining = false;
                yield break;
            }
        }
    }

    public void OtherAddInventory()
    {
        genMap.playRockNoise();

        playerSCR.AddToInventory(weightName + " " + TSname, weight, thisID, oreImage);
        Destroy(gameObject);
    }
    public float RandomWeight(float min, float max) {
        return UnityEngine.Random.Range(min, max);
    }
}
