using Unity.Mathematics;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenerateMap : MonoBehaviour
{
    public List<AudioClip> breakClips = new List<AudioClip>();
    public int mapHeight;
    public int mapWidth;
    public float xOrg;
    public float yOrg;
    public float scale;
    public float mapScale = 3f;
    public GameObject noisePreview;
    public GameObject mapPiece;
    public GameObject houseSpawner;
    public List<GameObject> types = new List<GameObject>();
    private Texture2D noise;
    private SpriteRenderer previewRenderer;
    private Vector2 pivotPreview;
    private float PPU = 100f;
    private Color[] pix;
    private AudioSource audioSource;
    public bool shouldGenerate = true;
    public bool generated = false;

    void Start()
    {
        xOrg = UnityEngine.Random.Range(0f, 1000f);
        yOrg = UnityEngine.Random.Range(0f, 1000f);

        previewRenderer = noisePreview.GetComponent<SpriteRenderer>();

        audioSource = GetComponent<AudioSource>();

        pivotPreview = previewRenderer.bounds.size / 2;
        noise = new Texture2D(mapWidth, mapHeight);
        pix = new Color[noise.width * noise.height];

        Sprite previewSprite = Sprite.Create(noise, new Rect(0, 0, noise.width, noise.height), pivotPreview, PPU);

        previewRenderer.sprite = previewSprite;

        CalcNoise();
    }

    void CalcNoise()
    {
        GameObject mapHolder = new GameObject("MapHolder");
        mapHolder.transform.SetParent(gameObject.transform);

        float[,] sampleValues = new float[mapWidth, mapHeight];
        for (int y = 0; y < noise.height; y++)
        {
            for (int x = 0; x < noise.width; x++)
            {
                float xCoord = xOrg + x / (float)noise.width * scale;
                float yCoord = yOrg + y / (float)noise.height * scale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                sampleValues[x, y] = sample;

                pix[y * noise.width + x] = new Color(sample > 0.6f ? 1f : 0f, sample > 0.6f ? 1f : 0f, sample > 0.6f ? 1f : 0f);
            }
        }

        noise.SetPixels(pix);
        noise.Apply();

        StartCoroutine(Generate(sampleValues, mapHolder));

        // for (int y = 0; y < noise.height; y++)
        // {
        //     for (int x = 0; x < noise.width; x++)
        //     {
        //         float sample = sampleValues[x, y];

        //         if (sample > 0.4f)
        //         {
        //             if (UnityEngine.Random.Range(0, 3) < 3)
        //             {
        //                 int oreNumber = UnityEngine.Random.Range(0, 100);


        //                 if (oreNumber < 65)
        //                 {
        //                     mapPiece = types[0];
        //                 }
        //                 else if (oreNumber < 80)
        //                 {
        //                     mapPiece = types[1];
        //                 }
        //                 else if (oreNumber < 90)
        //                 {
        //                     mapPiece = types[2];
        //                 }
        //                 else if (oreNumber < 97 && sample > 0.75f)
        //                 {
        //                     mapPiece = types[3];
        //                 }
        //                 else if (oreNumber >= 97 && sample > 0.8f)
        //                 {
        //                     mapPiece = types[4];
        //                 }
        //                 else
        //                 {
        //                     mapPiece = types[0];
        //                 }


        //                 GameObject thisPiece = Instantiate(mapPiece, new Vector2(x, y) * mapScale, Quaternion.Euler(0f, 0f, 0f));
        //                 thisPiece.transform.localScale = new Vector2(mapScale, mapScale);
        //                 thisPiece.transform.SetParent(mapHolder.transform);

        //                 if (sample > 0.8f)
        //                 {
        //                     SpriteRenderer pieceRen = thisPiece.GetComponent<SpriteRenderer>();
        //                     pieceRen.color = new Color(pieceRen.color.r - 0.1f, pieceRen.color.g - 0.1f, pieceRen.color.b - 0.1f);
        //                     pieceRen.sortingOrder = 11;
        //                 }
        //                 else if (sample > 0.7f)
        //                 {
        //                     SpriteRenderer pieceRen = thisPiece.GetComponent<SpriteRenderer>();
        //                     pieceRen.color = new Color(pieceRen.color.r - 0.050f, pieceRen.color.g - 0.050f, pieceRen.color.b - 0.050f);
        //                     pieceRen.sortingOrder = 10;
        //                 }
        //             }
        //         }
        //     }
        // }
    }

    void Update()
    {
    }

    private IEnumerator Generate(float[,] sampleValues, GameObject mapHolder)
    {
        if (!shouldGenerate) { yield break; }
        ;
        for (int y = 0; y < noise.height; y++)
        {
            for (int x = 0; x < noise.width; x++)
            {
                float sample = sampleValues[x, y];

                if (sample > 0.4f)
                {
                    if (UnityEngine.Random.Range(0, 3) < 3)
                    {
                        int oreNumber = UnityEngine.Random.Range(0, 100);


                        if (oreNumber < 65)
                        {
                            mapPiece = types[0];
                        }
                        else if (oreNumber < 80)
                        {
                            mapPiece = types[1];
                        }
                        else if (oreNumber < 90)
                        {
                            mapPiece = types[2];
                        }
                        else if (oreNumber < 97 && sample > 0.75f)
                        {
                            mapPiece = types[3];
                        }
                        else if (oreNumber >= 97 && sample > 0.8f)
                        {
                            mapPiece = types[4];
                        }
                        else
                        {
                            mapPiece = types[0];
                        }


                        GameObject thisPiece = Instantiate(mapPiece, new Vector2(x, y) * mapScale, Quaternion.Euler(0f, 0f, 0f));
                        thisPiece.transform.localScale = new Vector2(mapScale, mapScale);
                        thisPiece.transform.SetParent(mapHolder.transform);

                        if (sample > 0.8f)
                        {
                            SpriteRenderer pieceRen = thisPiece.GetComponent<SpriteRenderer>();
                            pieceRen.color = new Color(pieceRen.color.r - 0.1f, pieceRen.color.g - 0.1f, pieceRen.color.b - 0.1f);
                            pieceRen.sortingOrder = 11;
                        }
                        else if (sample > 0.7f)
                        {
                            SpriteRenderer pieceRen = thisPiece.GetComponent<SpriteRenderer>();
                            pieceRen.color = new Color(pieceRen.color.r - 0.050f, pieceRen.color.g - 0.050f, pieceRen.color.b - 0.050f);
                            pieceRen.sortingOrder = 10;
                        }
                    }
                }

            }

            if (y % 3 == 0)
            {
                Debug.Log("Layer: " + y + " done generating!");

                yield return new WaitForFixedUpdate();
            }

        }
        Vector2 mapSize = new(mapWidth * mapScale, mapHeight * mapScale);
        Vector2 mapCenterOffset = new(mapSize.x / 2f, mapSize.y);
        mapHolder.transform.position = -(Vector3)mapCenterOffset;

        int amountOfHouses = UnityEngine.Random.Range(1, 10);

        for (int i = 0; i < amountOfHouses; i++)
        {
            float halfMapSize = mapSize.x / 2f;

            int posX = Mathf.RoundToInt(UnityEngine.Random.Range(-halfMapSize, halfMapSize));
            int posY = Mathf.RoundToInt(UnityEngine.Random.Range(-mapHeight, 0));

            Vector2 posTS = new Vector2(posX, posY);
            GameObject houseGen = Instantiate(houseSpawner, posTS, Quaternion.Euler(0f, 0f, 0f));
        }

        generated = true;
        
    }

    public void playRockNoise()
    {
        audioSource.pitch = UnityEngine.Random.Range(0.7f, 1.2f);
        if (breakClips.Count <= 1)
        {
            audioSource.PlayOneShot(breakClips[0]);
        }
    }
}
