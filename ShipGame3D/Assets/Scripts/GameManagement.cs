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
    [SerializeField] private GameObject GameplayUI;

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
        GameplayUI.SetActive(true);

        isPlaying = true;
    }

    private void PlayerLoses()
    {
        if (!isPlaying) return;

        isPlaying = false;

        // Change UI
        MainMenu.SetActive(true);
        GameplayUI.SetActive(false);
    }

    public void Sunk() 
    {
        // Called when sink timer ends
        PlayerLoses();
    }

}
