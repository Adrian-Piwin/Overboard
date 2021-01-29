using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject settingsUI;

    [Header("Credits")]
    [SerializeField] private GameObject creditsUI;

    public void ToggleSettings()
    {
        settingsUI.SetActive(!settingsUI.active);
    }

    public void ToggleCredits()
    {
        creditsUI.SetActive(!creditsUI.active);
    }

    public void ExitApp()
    {
        Application.Quit();
    }
}
