using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public float foodMoveSpeed = 3;

    bool isLeft = false; // Start with the right side

    private GameObject foodPrefab;

    public List<Sprite> sprites = new List<Sprite>();

    public int amountToPool;


    public List<GameObject> pooledObjects;

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
        for (int i = 0; i < amountToPool; i++)
        {
            GameObject food = Instantiate(foodPrefab);
            pooledObjects.Add(food);
            yield return new WaitForSeconds(foodDelay);
        }
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }
        return null;
    }

    public void RespawnFood()
    {
        StartCoroutine(RespawningFood());
    }

    private IEnumerator RespawningFood()
    {
        while(GetPooledObject() != null) {
            GetPooledObject().GetComponent<Food>().SetRandomFood();
            yield return null;
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
            GameObject food = GetPooledObject();
            if (food != null)
            {
                food.GetComponent<Food>().SetRandomFood();
            }
            yield return new WaitForSeconds(foodDelay);
        }
    }
}
