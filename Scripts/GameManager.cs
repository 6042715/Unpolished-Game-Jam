using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public List<string> GoalNames = new List<string>();
    public List<Sprite> GoalSprites = new List<Sprite>();
    public List<float> GoalDifficulties = new List<float>();
    [SerializeField] private int GoalChosen;
    public string GoalChosenName;
    public Sprite GoalChosenSprite;
    public float GoalChosenDiff;
    public bool hasFoundGoal = false;

    public GameObject GoalHolderUIGOB;
    public GameObject GoalNotifHolderGOB;

    public GameObject endScreen;
    public GameObject FinalStatsGOB;
    private TextMeshProUGUI finalStats;

    public GameObject buttonHolder;

    public GameObject recipeShowHolder;

    private TextMeshProUGUI GoalNotifTMP;

    private Movement_player _Player;
    private ShowRecipes showRecipes;

    private AudioSource audioSource;

    public bool shouldPlayMusic = false;
    private float originalVolume;
    public GameObject toggleMusicButton;
    private Button toggleMusic;

    public GameObject lightsHolder;
    private List<Image> lightIMG = new List<Image>();
    [SerializeField] private List<bool> lightStatus = new List<bool>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _Player = FindFirstObjectByType<Movement_player>();
        showRecipes = recipeShowHolder.GetComponent<ShowRecipes>();

        GoalNotifTMP = GoalNotifHolderGOB.GetComponent<TextMeshProUGUI>();

        audioSource = GetComponent<AudioSource>();
        toggleMusic = toggleMusicButton.GetComponent<Button>();

        GenerateGoal();
        StartCoroutine(CheckerLoop(1.25f));

        originalVolume = audioSource.volume;

        GoalNotifTMP.text = "Acquire item ASAP!";

        endScreen.SetActive(false);

        if (!shouldPlayMusic) { audioSource.volume = 0f; }

        toggleMusic.onClick.AddListener(musicToggle);

        foreach (Transform child in lightsHolder.transform)
        {
            lightIMG.Add(child.gameObject.GetComponent<Image>());
            lightStatus.Add(false);
        }

        lightIMG[0].color = Color.blue;
        lightIMG[1].color = Color.red;
        lightIMG[2].color = Color.green;
        lightIMG[3].color = Color.yellow;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            showRecipes.LoadRecipes();
        }

    }

    public void GenerateGoal()
    {
        GoalChosen = UnityEngine.Random.Range(0, GoalNames.Count);

        GoalChosenName = GoalNames[GoalChosen];
        GoalChosenSprite = GoalSprites[GoalChosen];
        GoalChosenDiff = GoalDifficulties[GoalChosen];

        GoalHolderUIGOB.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = GoalChosenSprite;
        GoalHolderUIGOB.transform.GetChild(0).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = GoalChosenName;

        finalStats = FinalStatsGOB.GetComponent<TextMeshProUGUI>();
    }

    private void FoundGoal()
    {
        if (hasFoundGoal)
        {
            _Player.mineSpeed = 4f;

            TextMeshProUGUI uGUI = GoalHolderUIGOB.transform.GetChild(0).transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI uGUI2 = GoalHolderUIGOB.transform.GetChild(0).transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            uGUI.color = Color.yellow;
            uGUI2.color = Color.yellow;
        }
    }

    private IEnumerator CheckerLoop(float delay)
    {
        while (!hasFoundGoal)
        {
            int i = 0;
            foreach (Sprite sprite in _Player.inventorySprites)
            {
                if (sprite == GoalChosenSprite)
                {
                    hasFoundGoal = true;
                    FoundGoal();
                    StartCoroutine(CycleText(2f));
                }
                i++;
                if (i % 4 == 0)
                {
                    yield return new WaitForEndOfFrame();
                    toggleDebugLight(0);
                }
            }
            yield return new WaitForSecondsRealtime(delay);
        }
    }

    private IEnumerator CycleText(float delay)
    {
        while (hasFoundGoal)
        {
            GoalNotifTMP.text = "Mining speed boosted by NotEvilCorp LTD.";
            yield return new WaitForSecondsRealtime(delay);
            GoalNotifTMP.text = "Item acquired, get to bottom ASAP!";
            yield return new WaitForSecondsRealtime(delay);
            GoalNotifTMP.text = "Protect item with your life";
            yield return new WaitForSecondsRealtime(delay);
            GoalNotifTMP.text = "Damage to item will result in immediate death";
            yield return new WaitForSecondsRealtime(delay);
        }
    }

    public void ShowEndScreen()
    {
        StartCoroutine(WaitEndScreen());
    }

    IEnumerator WaitEndScreen()
    {
        yield return new WaitForSecondsRealtime(0.75f);

        float endScore = ((_Player.blocksMined * 2) + (_Player.itemsCrafted * 25) + (_Player.totalAirtime * 0.5f)) * GoalChosenDiff;

        endScreen.SetActive(true);

        finalStats.text =
        "---------- \n" +
        "You took: " + MathF.Round(_Player.timeSpent, 2) + " seconds \n" +
        "You mined: " + _Player.blocksMined + " blocks \n" +
        "You crafted: " + _Player.itemsCrafted + " item(s) \n" +
        "Your total airtime was: " + MathF.Round(_Player.totalAirtime, 2) + " seconds \n" +
        "Goal difficulty: " + GoalChosenDiff + "/10 \n" +
        "\n" +
        "Final Score: " + MathF.Round(endScore, 2) + " points \n" +
        "----------";

        int i = 0;
        foreach (Transform child in buttonHolder.transform)
        {
            Button button = child.gameObject.GetComponent<Button>();
            if (i == 0)
            {
                button.onClick.AddListener(RestartGame);
            }
            else if (i == 1)
            {
                button.onClick.AddListener(ToMenu);

                break;
            }
            i++;
        }

    }

    public void ToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void musicToggle()
    {
        if (shouldPlayMusic)
        {
            audioSource.volume = 0f;
            shouldPlayMusic = false;
        }
        else
        {
            audioSource.volume = originalVolume;
            shouldPlayMusic = true;
        }
    }

    public void toggleDebugLight(int number)
    {
        if (number == 0)
        {
            if (lightStatus[0] == true)
            {
                lightIMG[0].color = Color.darkBlue;
                lightStatus[0] = false;
            }
            else
            {
                lightIMG[0].color = Color.blue;
                lightStatus[0] = true;
            }
        }
        else if (number == 1)
        {
            if (lightStatus[1] == true)
            {
                lightIMG[1].color = Color.darkRed;
                lightStatus[1] = false;
            }
            else
            {
                lightIMG[1].color = Color.red;
                lightStatus[1] = true;
            }
        }
        else if (number == 2)
        {
            if (lightStatus[2] == true)
            {
                lightIMG[2].color = Color.darkGreen;
                lightStatus[2] = false;
            }
            else
            {
                lightIMG[2].color = Color.green;
                lightStatus[2] = true;
            }
        }
        else if (number == 3)
        {
            if (lightStatus[3] == true)
            {
                lightIMG[3].color = Color.yellow;
                lightStatus[3] = false;
            }
            else
            {
                lightIMG[3].color = Color.darkGoldenRod;
                lightStatus[3] = true;
            }
        }
    }
}
