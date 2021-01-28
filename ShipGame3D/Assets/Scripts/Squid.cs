using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Squid : MonoBehaviour
{
    [SerializeField] private float heaviestObjectMass;
    [SerializeField] private float throwForce;
    [SerializeField] private float heavyThrowForce;
    [SerializeField] Transform cargoHolder;

    private GameObject cargo;
    [NonSerialized] public Transform cargoManager;

    public void AssignCargo(GameObject cargo) 
    {
        // Assign cargo to squid arm
        cargo.SetActive(true);
        cargo.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Discrete;
        cargo.GetComponent<Rigidbody>().isKinematic = true;
        cargo.transform.position = cargoHolder.position;
        cargo.transform.rotation = cargoHolder.rotation;
        cargo.transform.parent = cargoHolder;
        this.cargo = cargo;
    }

    public void ThrowCargo() 
    {
        // Throw cargo
        if (cargoManager) cargo.transform.parent = cargoManager;
        cargo.GetComponent<Rigidbody>().isKinematic = false;
        cargo.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Update current weight
        cargoManager.GetComponent<CargoManager>().UpdateCurrentWeight();

        // Change force depending on cargo weight
        float force = Mathf.Lerp(throwForce, heavyThrowForce, cargo.GetComponent<Rigidbody>().mass / heaviestObjectMass);
        cargo.GetComponent<Rigidbody>().AddForce(cargo.transform.forward * force, ForceMode.Impulse);
    }

    public void DestroyThis() 
    {
        Destroy(gameObject);
    }
}
