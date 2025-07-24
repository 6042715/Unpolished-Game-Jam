using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<string> GoalNames = new List<string>();
    public List<Sprite> GoalSprites = new List<Sprite>();
    [SerializeField] private int GoalChosen;
    public string GoalChosenName;
    public Sprite GoalChosenSprite;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GoalChosen = Random.Range(0, GoalNames.Count);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
