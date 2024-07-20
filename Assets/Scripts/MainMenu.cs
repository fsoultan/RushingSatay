using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public TextMeshProUGUI highScoreText;
    public Button playButton;


    private void Start()
    {
        playButton.onClick.AddListener(() => { GameManager.instance.PlayGame(); });
    }
}
