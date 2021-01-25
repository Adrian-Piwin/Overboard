using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoOverboard : MonoBehaviour
{
    [SerializeField] private int cargoLayer;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer == cargoLayer) 
        {
            Destroy(collision.gameObject, 0.5f);
        }
    }
}
