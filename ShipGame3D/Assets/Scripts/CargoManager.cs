using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoManager : MonoBehaviour
{
    [SerializeField] Level level;
    [SerializeField] private LayerMask cargoLayer;

    private void Start()
    {
        int amt = GetCargoMass();
        level.maxWeight = amt;
        level.currentWeight = amt;
    }

    private void OnTriggerEnter(Collider collision)
    {
        // If cargo hit cargo overboard collider, deactivate cargo
        if (cargoLayer == (cargoLayer | (1 << collision.gameObject.layer)) && collision.gameObject.transform.parent == transform)
        {
            collision.gameObject.SetActive(false);
            UpdateCurrentWeight();
        }
    }

    private int GetCargoMass() 
    {
        // Count active objects to get current amount of cargo
        int count = 0;
        foreach (Transform child in transform) 
        {
            if (child.gameObject.active) 
            {
                count += (int)child.gameObject.GetComponent<Rigidbody>().mass;
            }
        }

        return count;
    }

    public void UpdateCurrentWeight()
    {
        level.currentWeight = GetCargoMass();
    }

    public GameObject GetMissingCargo() 
    {
        // Return random cargo that is currently inactive
        List<GameObject> inactiveCargo = new List<GameObject>();

        foreach (Transform child in transform)
        {
            if (!child.gameObject.active)
                inactiveCargo.Add(child.gameObject);
        }

        return inactiveCargo[Random.Range(0, inactiveCargo.Count)];
    }

    public bool IsMissingCargo() 
    {
        // Return true if there is any cargo that is not active  
        foreach (Transform child in transform)
        {
            if (!child.gameObject.active)
            {
                return true;
            }
        }

        return false;
    }

}
