using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameplayUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreUI;
    [SerializeField] private Transform sinkBarUI;
    [SerializeField] private float startingWidth;

    public float currentSinkPercentage;
    public float score;

    private void OnEnable()
    {
        sinkBarUI.localScale = new Vector3(startingWidth, sinkBarUI.localScale.y, sinkBarUI.localScale.z);
    }

    // Update is called once per frame
    void Update()
    {
        sinkBarUI.localScale = new Vector3(startingWidth * (1 - currentSinkPercentage), sinkBarUI.localScale.y, sinkBarUI.localScale.z);
        scoreUI.text = "Time Survived: " + (int)score;
    }
}