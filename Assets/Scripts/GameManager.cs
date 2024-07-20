using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public float foodMoveSpeed = 3;

    bool isLeft = false; // Start with the right side

    private GameObject foodPrefab;

    public List<Sprite> sprites = new List<Sprite>();

    private float foodDelay = 2;

    public GameObject mainPanel;
    private MainMenu mainMenu;
    public bool IsLeft { get => isLeft; set => isLeft = value; }

    private void Awake()
    {
        instance = this;

        foodPrefab = Resources.Load("Food") as GameObject;

        GoToMainMenu();
    }

    private void Start()
    {
        StartCoroutine(InitializeFood());
    }

    public void GoToMainMenu()
    {
        mainPanel.SetActive(false);
        if(mainMenu == null) { mainMenu = FindObjectOfType<MainMenu>(true); }
        mainMenu.highScoreText.text = "High Score : " + PlayerPrefs.GetInt(GamePrefs.highScorePrefs);
        mainMenu.gameObject.SetActive(true);
        
    }

    public void PlayGame()
    {
        mainMenu.gameObject.SetActive(false);
        FindObjectOfType<Interest>(true).ResetGame();
        FindObjectOfType<Stick>(true).ResetStick();
        mainPanel.SetActive(true);
    }

    private IEnumerator InitializeFood()
    {
        int foodCount = 6;
        for (int i = 0; i < foodCount; i++)
        {
            GameObject food = Instantiate(foodPrefab);
            yield return new WaitForSeconds(foodDelay);
        }
    }

    public void SpawnFood(int count)
    {
        StartCoroutine(SpawningFood(count));
    }
    private IEnumerator SpawningFood(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject food = Instantiate(foodPrefab);
            yield return new WaitForSeconds(foodDelay);
        }
    }
}
