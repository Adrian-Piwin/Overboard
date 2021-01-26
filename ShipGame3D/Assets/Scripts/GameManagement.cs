using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class GameManagement : MonoBehaviour
{

    [Header("Prefabs")]
    [SerializeField] private GameObject mainMenuLevel;
    [SerializeField] private List<GameObject> levelList;

    [Header("UI References")]
    [SerializeField] private GameObject MainMenu;

    private int currentLevel;
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
        Instantiate(levelList[currentLevel], Vector3.zero, Quaternion.identity, transform);

        // Change UI
        MainMenu.SetActive(false);

        isPlaying = true;
    }

    private void PlayerWins()
    {
        if (!isPlaying) return;

        isPlaying = false;
    }

    private void PlayerLoses()
    {
        if (!isPlaying) return;

        isPlaying = false;

    }

    public void EmptyCargo() 
    {
        // Called when there is no more cargo
        PlayerWins();
    }


}
