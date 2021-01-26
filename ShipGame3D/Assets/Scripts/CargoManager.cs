using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoManager : MonoBehaviour
{
    private GameManagement gameManagement;

    private void Start()
    {
        gameManagement = GameObject.Find("Game Management").GetComponent<GameManagement>();
    }

    private void Update()
    {
        if (gameManagement.isPlaying && transform.childCount == 0)
            gameManagement.EmptyCargo();
    }
}
