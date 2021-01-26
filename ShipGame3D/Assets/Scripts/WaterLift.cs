using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterLift : MonoBehaviour
{
    [SerializeField] private float waterHeight;
    [SerializeField] private float waterPower;

    // Update is called once per frame
    void Update()
    {
        PushUp();
    }

    private void PushUp() 
    {
        // Raycast to find object over water
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.up, out hit, waterHeight)) 
        {
            // If hit rigid body, push it upwards
            if (hit.transform.GetComponent<Rigidbody>())
            {
                hit.transform.GetComponent<Rigidbody>().AddForce(Vector3.up * waterPower, ForceMode.Impulse);
            }
        }
    }
}
