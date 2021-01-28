using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSplash : MonoBehaviour
{
    [SerializeField] GameObject splashPrefab;
    [SerializeField] float minSplashParticles;
    [SerializeField] float maxSplashParticles;
    [SerializeField] float heaviestObjMass;
    [SerializeField] float splashDuration;
    [SerializeField] LayerMask splashLayer;

    private Water water;

    // Start is called before the first frame update
    void Start()
    {
        water = GetComponent<Water>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        // If something that can make a splash hit the collider, spawn splash effect
        if (splashLayer == (splashLayer | (1 << collision.gameObject.layer)) && splashLayer == (splashLayer | (1 << collision.gameObject.transform.parent.gameObject.layer)))
        {
            // Splash effect
            StartCoroutine(SpawnSplash(collision.transform, water.WaterLevel(collision.transform.position), collision.transform.GetComponent<Rigidbody>().mass));
        }
    }

    IEnumerator SpawnSplash(Transform obj, float yPos, float objMass) 
    {
        // Wait until object has passed water level
        while (true)
        {
            yield return new WaitForSeconds(0.01f);
            if (obj.position.y < yPos)
                break;
        }

        // Spawn splash
        GameObject splash = Instantiate(splashPrefab, new Vector3(obj.position.x, yPos, obj.position.z), Quaternion.identity, null);
        splash.transform.eulerAngles = new Vector3(
            splash.transform.eulerAngles.x - 90,
            splash.transform.eulerAngles.y,
            splash.transform.eulerAngles.z
        );

        // Splash size
        splash.GetComponent<ParticleSystem>().emissionRate = Mathf.Lerp(minSplashParticles, maxSplashParticles, objMass / heaviestObjMass);

        // Destroy when done
        Destroy(splash, splashDuration);
    }
}
