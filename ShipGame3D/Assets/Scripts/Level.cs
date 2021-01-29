using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cinemachine;

public class Level : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField] private float maxSinkDepth;

    [Header("Level Start Effect Settings")]
    [SerializeField] private float screenShakeAmt;
    [SerializeField] private float startEffectDuration;
    private bool isSinking;

    [Header("Difficulty Settings")]
    [SerializeField] private float difficultyIncreaseRate;
    [SerializeField] private float weightPercentToRise;

    [SerializeField] private float sinkSpeedMod;
    [SerializeField] private float sinkSpeedModAdd;
    [SerializeField] private float sinkSpeedModMax;

    [SerializeField] private float squidSpawnRate;
    [SerializeField] private float squidSpawnRateAdd;
    [SerializeField] private float squidSpawnRateMin;

    private float difficulty;

    [Header("References")]
    [SerializeField] private Transform boat;
    [SerializeField] private Transform leaks;
    [SerializeField] private GameObject playerBoundry;
    [SerializeField] private GameObject cargoBoundry;
    [SerializeField] private CargoManager cargoManager;
    [SerializeField] private SquidManagement squidManagement;
    [SerializeField] private Animator boatAnimator;
    [SerializeField] private CinemachineVirtualCamera camera;

    private List<float> startingLeakSize;
    private GameManagement gameManagement;
    private GameplayUI gameplayUI;

    // Weight
    [NonSerialized] public float maxWeight;
    [NonSerialized] public float currentWeight;

    // Sinking
    private float sinkSpeed;
    [NonSerialized] public float currentSinkPercentage;

    // Score
    [NonSerialized] public float score;
    private float startingTime;

    // Start is called before the first frame update
    void Start()
    {
        gameManagement = GameObject.Find("Game Management").GetComponent<GameManagement>();
        gameplayUI = GameObject.Find("Gameplay UI").GetComponent<GameplayUI>();

        startingLeakSize = new List<float>();
        foreach (Transform leak in leaks)
        {
            startingLeakSize.Add(leak.GetComponent<ParticleSystem>().startSpeed);
        }

        // Setup squids
        squidManagement.spawnRate = squidSpawnRate;

        StartCoroutine(StartSinkingEffects()) ;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isSinking) return;

        IncreaseDifficulty();
        Sinking();
        SinkBoatEffect();
        DynamicLeakSize();
        score = Time.time - startingTime;
        gameplayUI.currentSinkPercentage = currentSinkPercentage;
        gameplayUI.score = score;
    }

    private void IncreaseDifficulty() 
    {
        // Increase difficulty when score reaches threshold
        if (score > difficulty + difficultyIncreaseRate) 
        {
            difficulty += difficultyIncreaseRate;

            // Increase sink speed
            if (sinkSpeedMod < sinkSpeedModMax)
                sinkSpeedMod += sinkSpeedModAdd;

            // Increase squid spawn rate
            if (squidSpawnRate > squidSpawnRateMin) 
            {
                squidSpawnRate += squidSpawnRateAdd;
                squidManagement.spawnRate = squidSpawnRate;
            }
        }
    }

    private void Sinking() 
    {
        if (!gameManagement.isPlaying || maxWeight == 0) return;

        // Set speed of sinking depending on weight percentage
        sinkSpeed = (currentWeight / maxWeight);
        // Rise if weight under certain percentage
        sinkSpeed = (sinkSpeed < weightPercentToRise) ? -(1 - sinkSpeed) : sinkSpeed;

        if (currentSinkPercentage >= 0 && currentSinkPercentage < 1) 
        {
            currentSinkPercentage += sinkSpeed * sinkSpeedMod * Time.deltaTime;
        }

        // Sink percentage is 100%
        if (currentSinkPercentage >= 1)
            SinkBoat();
        else if (currentSinkPercentage <= 0)
            currentSinkPercentage = 0;
    }

    private void SinkBoatEffect() 
    {
        // Move boat transform down as boat sinks    change
        boat.position = new Vector3(boat.position.x, -2.5f - (maxSinkDepth * currentSinkPercentage), boat.position.z);
    }

    private void DynamicLeakSize() 
    {
        // Change leak size depending on weight
        int i = 0;
        foreach (Transform leak in leaks)
        {
            leak.GetComponent<ParticleSystem>().startSpeed = startingLeakSize[i] * Mathf.Clamp((currentWeight / maxWeight), 0.3f, 1f);
            i++;
        }
    }

    private void SinkBoat() 
    {
        // Sunk, no longer sinking
        isSinking = false;

        // Disable boundies
        playerBoundry.SetActive(false);
        cargoBoundry.SetActive(false);

        // Turn off leaks
        foreach (Transform leak in leaks)
        {
            leak.GetComponent<ParticleSystem>().enableEmission = false;
        }

        // Make all kinematic objects no longer kinematic
        foreach (Transform cargo in cargoManager.transform) 
        {
            if (cargo.GetComponent<Rigidbody>().isKinematic)
                cargo.GetComponent<Rigidbody>().isKinematic = false;
        }

        // Stop spawning squids
        squidManagement.isSpawning = false;
        // Stop following the player
        camera.Follow = null;
        // Play sink animation
        boatAnimator.Play("BoatSink");
        // Tell game management the player sunk
        gameManagement.Sunk(score);
    }

    IEnumerator StartSinkingEffects() 
    {
        // Start screen shake
        camera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = screenShakeAmt;

        yield return new WaitForSeconds(startEffectDuration);

        // Stop screen shake
        camera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0f;

        // Start leaks on end of screen shake
        foreach (Transform leak in leaks)
        {
            leak.GetComponent<ParticleSystem>().enableEmission = true;
        }

        startingTime = Time.time;
        isSinking = true;
    }

}
