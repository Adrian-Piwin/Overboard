using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BoatController : MonoBehaviour
{
    [SerializeField] private bool moveForward;
    [SerializeField] private float speed;

    private Rigidbody body;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (moveForward)
            body.AddForce(transform.forward * speed);
    }
}
