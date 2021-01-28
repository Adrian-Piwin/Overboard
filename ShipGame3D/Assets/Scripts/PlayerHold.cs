﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHold : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private LayerMask cargoLayer;

    [NonSerialized] public bool canHold;
    [SerializeField] public Transform currentObject;

    private void OnTriggerEnter(Collider other)
    {
        if (playerMovement.isHolding) return;

        if (cargoLayer == (cargoLayer | (1 << other.gameObject.layer)) && currentObject == null) {
            currentObject = other.transform;
            canHold = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform == currentObject)
        {
            currentObject = null;
            canHold = false;
        }
    }
}
