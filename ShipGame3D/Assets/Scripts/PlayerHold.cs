using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHold : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private int cargoLayer;
    [NonSerialized] public bool canHold;
    [NonSerialized] public Transform currentObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == cargoLayer && !playerMovement.isHolding) {
            currentObject = other.transform;
            canHold = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == cargoLayer && !playerMovement.isHolding)
        {
            currentObject = null;
            canHold = false;
        }
    }
}
