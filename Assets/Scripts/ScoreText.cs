using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreText : MonoBehaviour
{
    public static ScoreText instance;
    private TextMeshProUGUI scoreText;
    private void Awake()
    {
        scoreText = GetComponent<TextMeshProUGUI>();
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddScore(int score)
    {
        StartCoroutine(AddingScore(score));
    }

    private IEnumerator AddingScore(int score)
    {
        scoreText.text = "Score : " + FoodInterest.instance.Score.ToString();
        if(score >= 0)
        {
            scoreText.color = Color.green;
        }
        else
        {
            scoreText.color = Color.red;
        }

        yield return StartCoroutine(AnimateText());

        scoreText.color = Color.white;

    }

    private IEnumerator AnimateText()
    {
        float duration = 0.5f;
        float enlargedSize = 1.4f;
        Vector3 originalScale = scoreText.rectTransform.localScale;
        
        // Enlarge
        float elapsedTime = 0f;
        while (elapsedTime < duration / 2)
        {
            scoreText.rectTransform.localScale = Vector3.Lerp(originalScale, originalScale * enlargedSize, (elapsedTime / (duration / 2)));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        scoreText.rectTransform.localScale = originalScale * enlargedSize;

        // Return to normal size
        elapsedTime = 0f;
        while (elapsedTime < duration / 2)
        {
            scoreText.rectTransform.localScale = Vector3.Lerp(originalScale * enlargedSize, originalScale, (elapsedTime / (duration / 2)));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        scoreText.rectTransform.localScale = originalScale;
    }
}
