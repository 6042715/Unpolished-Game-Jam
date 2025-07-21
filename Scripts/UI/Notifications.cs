using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Notifications : MonoBehaviour
{
    public GameObject textHolder;
    public GameObject imageHolder;
    public Color activeColor;
    public Color passiveColor;

    public AudioClip notifySound;

    public List<String> facts = new List<string>();
    public List<Sprite> dollarDude = new List<Sprite>();


    private TextMeshProUGUI uGUI;
    private Image image;
    private AudioSource audioSource;

    [SerializeField] private bool idle = false;
    [SerializeField] private float idleTimer = 0f;

    private bool isShowingFacts = false;
    public float factCooldown = 2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        uGUI = textHolder.GetComponent<TextMeshProUGUI>();
        image = imageHolder.GetComponent<Image>();
        audioSource = GetComponent<AudioSource>();

        uGUI.color = passiveColor;

        StartCoroutine(timer());
        StartCoroutine(ShowFactsIdle());
    }

    // Update is called once per frame
    void Update()
    {
        if (idleTimer > 5.0f)
        {
            idle = true;
        }
        else
        {
            idle = false;
        }

    }

    public void ShowNotification(string name, float weight, int ID, Sprite sprite)
    {
        uGUI.text = "Added: " + name + " ( " + weight.ToString() + " KG)";
        image.sprite = sprite;

        StartCoroutine(NotifEffect());

        idle = false;
        idleTimer = 0f;
    }
    private IEnumerator NotifEffect()
    {
        uGUI.color = activeColor;

        audioSource.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
        audioSource.PlayOneShot(notifySound);

        yield return new WaitForSecondsRealtime(0.5f);
        uGUI.color = passiveColor;
    }
    private IEnumerator timer()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(0.1f);

            idleTimer += 0.1f;
        }
    }

    private IEnumerator ShowFactsIdle()
    {
        while (true)
        {
            if (idle)
            {
                uGUI.text = facts[UnityEngine.Random.Range(0, facts.Count)];
                image.sprite = dollarDude[UnityEngine.Random.Range(0, dollarDude.Count)];

                StartCoroutine(NotifEffect());
            }
            yield return new WaitForSecondsRealtime(factCooldown);
            
        }

    }
}
