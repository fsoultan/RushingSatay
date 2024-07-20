using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public Button playAgain;
    public Button mainMenu;

    private void Awake()
    {
        playAgain.onClick.AddListener(() => { 
            gameObject.SetActive(false);
            GameManager.instance.PlayGame(); });

        mainMenu.onClick.AddListener(() => {  gameObject.SetActive(false);
            GameManager.instance.GoToMainMenu();
        });
    }
}
