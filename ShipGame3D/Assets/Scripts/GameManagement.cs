using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering.UI;
using TMPro;

public class GameManagement : MonoBehaviour
{

    [Header("Prefabs")]
    [SerializeField] private GameObject mainMenuLevel;
    [SerializeField] private List<GameObject> levelList;

    [Header("UI References")]
    [SerializeField] private GameObject MainMenu;
    [SerializeField] private GameObject GameplayUI;
    [SerializeField] private GameObject EndgameUI;

    [SerializeField] private TextMeshProUGUI scoreUI;
    [SerializeField] private TextMeshProUGUI highscoreUI;
    [SerializeField] private GameObject newScoreUI;

    [SerializeField] public bool isPlaying;

    private void Start()
    {
        MainMenu.SetActive(true);
    }

    public void StartGame()
    {
        if (isPlaying) return;

        // Destroy any existing levels
        if (transform.childCount != 0)
        {
            foreach (Transform level in transform)
            {
                Destroy(level.gameObject);
            }
        }

        // Instantiate new level
        Instantiate(levelList[UnityEngine.Random.Range(0, levelList.Count)], Vector3.zero, Quaternion.identity, transform);

        // Change UI
        MainMenu.SetActive(false);
        EndgameUI.SetActive(false);
        newScoreUI.SetActive(false);
        GameplayUI.SetActive(true);

        isPlaying = true;
    }

    private void PlayerLoses()
    {
        if (!isPlaying) return;

        isPlaying = false;

        // Change UI
        EndgameUI.SetActive(true);
        GameplayUI.SetActive(false);
    }

    public void ResetToMenu() 
    {
        // Destroy any existing levels
        if (transform.childCount != 0)
        {
            foreach (Transform level in transform)
            {
                Destroy(level.gameObject);
            }
        }

        // Instantiate main menu level
        Instantiate(mainMenuLevel, Vector3.zero, Quaternion.identity, transform);

        // Change UI
        EndgameUI.SetActive(false);
        newScoreUI.SetActive(false);
        MainMenu.SetActive(true);
    }

    public void Sunk(float score) 
    {
        // Called when sink timer ends

        int highScore = PlayerPrefs.GetInt("highscore", 0);

        scoreUI.text = (int)score + "";
        if (highScore < score)
        {
            newScoreUI.SetActive(true);
            highScore = (int)score;
        }

        PlayerPrefs.SetInt("highscore", highScore);
        highscoreUI.text = highScore + "";

        PlayerLoses();
    }

}
