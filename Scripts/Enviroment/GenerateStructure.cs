
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GenerateStructure : MonoBehaviour
{
    private GenerateMap generateMap;

    public GameObject testPos;
    public GameObject wood;
    public GameObject table;
    public GameObject lamp;

    public List<Sprite> tableSprites = new List<Sprite>();
    public List<Sprite> lampSprites = new List<Sprite>();

    private Vector2 testT;

    public int maxHouseHeight = 10;
    public int minHouseHeight = 4;

    public int maxHouseWidth = 12;
    public int minHouseWidth = 5;

    public int doorHouseHeight = 1;

    private bool isHouseAbon = false;

    public Vector2 detectorSize = new Vector2(0.25f, 0.25f);

    private AudioSource houseSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        generateMap = FindFirstObjectByType<GenerateMap>();
        testT = testPos.transform.position;

        if (testPos)
        {
            GenerateQueue("house");
        }
        // testPos.transform.position -= new Vector3(1f, 1f);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void GenerateQueue(string type)
    {
        StartCoroutine(WaitForGen(0.1f, type));
    }

    private void Generate(string type)
    {
        if (type == "house")
        {
            if (Random.Range(0, 15) == 4)
            {
                isHouseAbon = true;
            }

            GameObject House = new GameObject("House");
            houseSource = House.AddComponent<AudioSource>();
            houseSource.volume = 0.5f;

            int height = Random.Range(minHouseHeight, maxHouseHeight);
            int width = Random.Range(minHouseWidth, maxHouseWidth);

            for (int y = 0; y < height; y++)
            {
                if (y == 0 || y == height - 1)
                {
                    for (int x = 0; x < width; x++)
                    {
                        GameObject generatedTile = Instantiate(wood, new Vector2(testT.x + x, testT.y - y), Quaternion.Euler(0f, 0f, 0f));
                        // RemoveStone(new Vector2(testT.x + x, testT.y - y), detectorSize);

                        if (y == 0)
                        {
                            generatedTile.name = "Ceiling";

                            if (Random.Range(0, 6) == 1 && x != 0 && x != width - 1)
                            {
                                GameObject lampINS = Instantiate(lamp, new Vector2(testT.x + x, testT.y - y - 1.12f), Quaternion.Euler(0f, 0f, 0f));

                                int selectedSprite = Random.Range(0, lampSprites.Count);
                                lampINS.GetComponent<SpriteRenderer>().sprite = lampSprites[selectedSprite];

                                lampINS.name = "Lamp-" + selectedSprite;

                                lampINS.transform.SetParent(generatedTile.transform);
                            }
                        }
                        else
                        {
                            generatedTile.name = "Floor";
                            if (Random.Range(0, 4) == 1 && x != 0 && x != width - 1)
                            {
                                GameObject Table = Instantiate(table, new Vector2(testT.x + x, testT.y - y + 1), Quaternion.Euler(0f, 0f, 0f));

                                // RemoveStone(new Vector2(testT.x + x, testT.y - y + 1), detectorSize);
                                int selectedSprite = Random.Range(0, tableSprites.Count);
                                Table.GetComponent<SpriteRenderer>().sprite = tableSprites[selectedSprite];

                                Table.name = "Table-" + selectedSprite;

                                Table.transform.SetParent(generatedTile.transform);
                            }
                        }

                        generatedTile.transform.SetParent(House.transform);
                    }
                }
                if (y != height - 1 - doorHouseHeight)
                {
                    Vector2 pos1 = new Vector2(testT.x + width, testT.y - y);
                    Vector2 pos2 = new Vector2(testT.x + 0, testT.y - y);
                    GameObject wallLeft = Instantiate(wood, pos1, Quaternion.Euler(0f, 0f, 0f));
                    GameObject wallRight = Instantiate(wood, pos2, Quaternion.Euler(0f, 0f, 0f));

                    // RemoveStone(pos1, detectorSize);
                    // RemoveStone(pos2, detectorSize);

                    wallLeft.name = "WallRight";
                    wallRight.name = "WallLeft";

                    wallLeft.transform.SetParent(House.transform);
                    wallRight.transform.SetParent(House.transform);
                }
            }

            House.transform.position += new Vector3(0.5f, -0.5f);

            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    Vector2 destroyPos = new Vector2(testT.x + x, testT.y - y);
                    RemoveStone(destroyPos, detectorSize);
                }
            }

            if (isHouseAbon)
            {
                int totalAmount = House.transform.childCount;
                int amountToDestroy = Random.Range(0, totalAmount - 4);

                for (int i = 0; i < amountToDestroy; i++)
                {
                    Destroy(House.transform.GetChild(Random.Range(0, House.transform.childCount)).gameObject);
                }

                foreach (Transform child in House.transform)
                {
                    SpriteRenderer SPR = child.GetComponent<SpriteRenderer>();

                    float brightness = Random.Range(-0.1f, 0f);
                    SPR.color = new Color(SPR.color.r + brightness, SPR.color.g + brightness, SPR.color.b + brightness);

                    if (Random.Range(0, 3) == 2)
                    {
                        Vector3 euler = child.transform.rotation.eulerAngles;
                        euler.z += Random.Range(-10f, 10f);
                        child.transform.rotation = Quaternion.Euler(euler);
                    }

                    child.gameObject.GetComponent<BlockMinable>().isWoodBroken = true;
                    
                }
            }

        }
    }
    private void RemoveStone(Vector2 pos, Vector2 scale)
    {
        Collider2D[] hitStoneCols = Physics2D.OverlapBoxAll(pos, scale, 0f);
        if (hitStoneCols.Length > 0)
        {
            foreach (Collider2D col in hitStoneCols)
            {
                if (col.gameObject.GetComponent<BlockMinable>() != null)
                {
                    Destroy(col.gameObject);
                }
            }
        }
    }
    private IEnumerator WaitForGen(float delay, string type)
    {
        while (true)
        {
            if (generateMap.generated == true)
            {
                Generate(type);
                yield break;
            }
            yield return new WaitForSecondsRealtime(delay);
        }
    }
}
