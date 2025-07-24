using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
    public GameObject stButton;
    public GameObject opButtton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stButton.GetComponent<Button>().onClick.AddListener(startGame);
    }

    private void startGame() {
        SceneManager.LoadScene(1);
    }
}
