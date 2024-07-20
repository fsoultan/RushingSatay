using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class FoodInterest : MonoBehaviour
{
    private int score;

    public static FoodInterest instance;

    private RectTransform orderParent;
    private RectTransform allergicParent;

    [SerializeField] private List<Sprite> foodOrder = new List<Sprite>();
    [SerializeField] private List<Sprite> allergicFoods = new List<Sprite>();

    private bool initialized;

    private GameObject foodInterestPrefab;
    public bool Initialized { get => initialized; }
    public int Score { get => score;}



    private void Awake()
    {
        instance = this;



    }

    private void Start()
    {
        SetFoodInterest();
    }

    private void SetFoodInterest()
    {
        if (orderParent == null) orderParent = transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
        if (allergicParent == null) allergicParent = transform.GetChild(1).GetChild(0).GetComponent<RectTransform>();

        if(foodInterestPrefab == null) foodInterestPrefab = Resources.Load("FoodContainer") as GameObject;

        foreach (Transform child in orderParent)
        {
            Destroy(child.gameObject);
        }

        foreach(Transform child in allergicParent)
        {
            Destroy(child.gameObject);
        }

        List<Sprite> foodSprites = new List<Sprite>(GameManager.instance.sprites);

        foodOrder.Clear();
        allergicFoods.Clear();

        int allergicCount = Random.Range(0, 4);

        for(int i = 0;  i < allergicCount; i++)
        {
            int selectedFood = Random.Range(0, foodSprites.Count);
            allergicFoods.Add(foodSprites[selectedFood]);
            foodSprites.RemoveAt(selectedFood);
        }

        foreach(Sprite sprite in allergicFoods)
        {
            GameObject food = Instantiate(foodInterestPrefab,allergicParent,false);
            food.transform.GetChild(0).GetComponent<Image>().sprite = sprite;
        }

        for(int i = 0; i < Stick.instance.MaxFood; i++)
        {
            GameObject food = Instantiate(foodInterestPrefab, orderParent, false);
            Sprite sprite = foodSprites[Random.Range(0, foodSprites.Count)];
            food.transform.GetChild(0).GetComponent<Image>().sprite = sprite;
            foodOrder.Add(sprite);

        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(orderParent);
        LayoutRebuilder.ForceRebuildLayoutImmediate(allergicParent);

    }

    public void ResetScore()
    {
        score = 0;
        FindObjectOfType<ScoreText>(true).GetComponent<TextMeshProUGUI>().text = "Score : " + score;
        SetFoodInterest();
    }

    public void SubmitFood(List<Sprite> foodList)
    {


        // Count occurrences in each list
        var countA = foodOrder.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());
        var countB = foodList.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());

        // Calculate the total count of elements in `a` that are matched in `b`
        int correct = countA.Sum(pair =>
            Mathf.Min(pair.Value, countB.GetValueOrDefault(pair.Key, 0)));


        int error = foodOrder.Count - correct;

        int critical = foodList.Sum(item => allergicFoods.Count(x => x == item));


        int addedScore = 0;

        string reaction = "";

        if(critical > 0)
        {
            addedScore -= 50 * critical;
            int picker = Random.Range(0, 2);
            if(picker > 0) 
            {
                reaction = "BAD SATAY <sprite=10>";

            }
            else
            {
                reaction = "WHAT??? <sprite=15>";
            }
        }
        else
        {
            if(correct < foodOrder.Count)
            {
                if(correct > 1)
                {
                    reaction = "NOT BAD <sprite=4>";
                }
                else
                {
                    reaction = "OK? <sprite=14>";
                }
                addedScore += 30 * correct;
                addedScore -= 10 * error;
            }else
            {
                reaction = "PERFECT <sprite=2>";
                addedScore = 100;
            }

        }

        Interest.instance.Slider.value += (float)addedScore / (float)100;

        ReactionText.instance.SetText(reaction);
        

        score += addedScore;
        score = Mathf.Clamp(score, 0, int.MaxValue);

        ScoreText.instance.AddScore(addedScore);
        SetFoodInterest();
    }
}
