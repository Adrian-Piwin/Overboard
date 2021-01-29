using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManagement : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resolutionText;
    [SerializeField] private Toggle fullscreenToggle;

    private List<Vector2Int> resolutionList = new List<Vector2Int> {
        new Vector2Int(2560, 1440),
        new Vector2Int(1920, 1080),
        new Vector2Int(1280, 720),
        new Vector2Int(852, 480)
    };

    private int currentResoultion;
    private bool isFullscreen;

    // Start is called before the first frame update
    void Start()
    {
        currentResoultion = PlayerPrefs.GetInt("Current Resolution", 1);
        isFullscreen = PlayerPrefs.GetInt("Is Fullscreen", 1) == 1 ? true : false;

        Screen.SetResolution(resolutionList[currentResoultion].x, resolutionList[currentResoultion].y, isFullscreen);
        resolutionText.text = resolutionList[currentResoultion].x + "x" + resolutionList[currentResoultion].y;
        fullscreenToggle.isOn = isFullscreen;
    }

    public void OnResolutionClick() {
        currentResoultion++;

        if (currentResoultion > resolutionList.Count - 1)
            currentResoultion = 0;

        Screen.SetResolution(resolutionList[currentResoultion].x, resolutionList[currentResoultion].y, isFullscreen);
        resolutionText.text = resolutionList[currentResoultion].x + "x" + resolutionList[currentResoultion].y;
        PlayerPrefs.SetInt("Current Resolution", currentResoultion);
    }

    public void OnFullScreenToggle(bool toggle) {
        isFullscreen = toggle;
        Screen.SetResolution(resolutionList[currentResoultion].x, resolutionList[currentResoultion].y, isFullscreen);
        PlayerPrefs.SetInt("Is Fullscreen", toggle ? 1 : 0);
    }
}
