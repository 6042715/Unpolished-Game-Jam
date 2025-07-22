using System;
using System.Collections;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class LoadScreen : MonoBehaviour
{
    public GameObject loadTextHolder;
    public GameObject loadTimeHolder;
    public GameObject generatingInfoHolder;
    private GenerateMap generateMap;
    private TextMeshProUGUI uGUI;
    private TextMeshProUGUI uGUI2;
    private TextMeshProUGUI uGUI3;
    private Image image;
    private int totalLayers;
    private int curLayer;
    private DateTime startTime;
    private bool generationComplete = false;
    private bool updateTime = true;
    private bool shouldFade = false;
    private float fadeTimer = 0f;
    private float fadeDuration = 1f;

    void Start()
    {
        generateMap = FindFirstObjectByType<GenerateMap>();

        uGUI = loadTextHolder.GetComponent<TextMeshProUGUI>();
        uGUI2 = loadTimeHolder.GetComponent<TextMeshProUGUI>();
        uGUI3 = generatingInfoHolder.GetComponent<TextMeshProUGUI>();
        image = GetComponent<Image>();

        totalLayers = generateMap.mapHeight;
        startTime = DateTime.Now;

        if (generateMap.generated == true)
        {
            shouldFade = true;
        }
    }

    void Update()
    {
        generationComplete = generateMap.generated;

        curLayer = generateMap.layersGenerated;
        uGUI.text = curLayer + " / " + totalLayers;

        float fps = 1f / Time.smoothDeltaTime;

        if (fps > 5)
        {
            uGUI.color = Color.green;
        }
        else if (fps < 4)
        {
            uGUI.color = Color.limeGreen;
        }
        else if (fps < 2)
        {
            uGUI.color = Color.yellow;
        }
        else if (fps < 1)
        {
            uGUI.color = Color.orange;
        }
        else if (fps < 0.3f)
        {
            uGUI.color = Color.red;
        }
        else
        {
            uGUI.color = Color.purple;
        }

        Debug.Log("FPS: " + fps);

        if (!generationComplete)
        {
            uGUI3.text = "Generating terrain";
        }
        else
        {
            uGUI3.text = " ";
        }

        if (generationComplete && updateTime)
        {
            TimeSpan duration = DateTime.Now - startTime;
            uGUI2.text = "Took: " + string.Format("{0:D2}m:{1:D2}s", duration.Minutes, duration.Seconds);

            generationComplete = false;
            updateTime = false;

            shouldFade = true;
        }

        if (shouldFade)
        {
            fadeTimer += Time.deltaTime;
            float t = Mathf.Clamp01(fadeTimer / fadeDuration); // 0 to 1 over time

            float opacity = math.lerp(1f, 0f, t);
            image.color = new Color(image.color.r, image.color.g, image.color.b, opacity);

            if (image.color.a == 0f)
            {
                Debug.Log("Destroying load screen!");
                Destroy(gameObject);

                shouldFade = false;
            }
        }
    }

}
