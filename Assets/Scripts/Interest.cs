using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Interest : MonoBehaviour
{
    public static Interest instance;
    private TextMeshProUGUI emote;
    private Slider slider;
    public Image Fill;
    private Color defaultColor = new Color(1, 0.7f, 0);

    private float offset = 0.5f;  // Offset value (means % of the original color)

    private bool gameOver = false;

    public Slider Slider { get => slider; set => slider = value; }
    public bool GameOver { get => gameOver; }

    private void Awake()
    {
        instance = this;

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public void ResetGame()
    {
        FindObjectOfType<FoodInterest>(true).ResetScore();
        emote = GetComponentInChildren<TextMeshProUGUI>();
        slider = GetComponent<Slider>();
        slider.onValueChanged.RemoveAllListeners();
        slider.onValueChanged.AddListener(OnSliderValueChanged);
        slider.value = 1;
        gameOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameOver)
        {
            if (slider.value > 0)
            {
                slider.value -= Time.deltaTime * 0.02f;
            }
            else
            {
                gameOver = true;
                GameOver gameOverScript = FindObjectOfType<GameOver>(true);
                gameOverScript.gameObject.SetActive(true);
                if(FoodInterest.instance.Score > PlayerPrefs.GetInt(GamePrefs.highScorePrefs))
                {
                    PlayerPrefs.SetInt(GamePrefs.highScorePrefs,FoodInterest.instance.Score);
                }
                gameOverScript.scoreText.text = "Score : " + FoodInterest.instance.Score.ToString();
            }
        }

    }

    // Method to handle the slider value change
    private void OnSliderValueChanged(float value)
    {
        // Additional logic when slider value changes
        Fill.color = new Color(Mathf.Lerp(defaultColor.r * offset, defaultColor.r, slider.value), Mathf.Lerp(defaultColor.g * offset, defaultColor.g, slider.value), 0);

        string emoteValue = "<sprite=4>";
        if (slider.value > 0.6f)
        {
            emoteValue = "<sprite=4>";
        }
        else if (slider.value > 0.3f && slider.value <= 0.6f)
        {
            emoteValue = "<sprite=14>";
        }
        else
        {
            emoteValue = "<sprite=15>";
        }

        emote.text = emoteValue;
    }

}
