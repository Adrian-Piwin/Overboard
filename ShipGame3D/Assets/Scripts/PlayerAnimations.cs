using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem dustRunningParticles;

    private PlayerMovement playerMovement;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        dustRunningParticles.enableEmission = false;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAnimation();
        UpdateParticleSystems();
    }

    private void UpdateAnimation() 
    {
        if (playerMovement.moveInput.magnitude != 0)
        {
            animator.SetBool("isWalking", true);
        }
        else 
        {
            animator.SetBool("isWalking", false); 
        }

        if (playerMovement.IsFacingHeavyCargo())
        {
            animator.SetBool("isFacingHeavyCargo", true);
        }
        else 
        {
            animator.SetBool("isFacingHeavyCargo", false);
        }


        if (playerMovement.isHolding)
        {
            animator.SetBool("isHolding", true);
        }
        else 
        {
            animator.SetBool("isHolding", false);
        }

        if (playerMovement.isThrowing)
        {
            animator.SetBool("isThrowing", true);
            playerMovement.isThrowing = false;
        }
        else
        {
            animator.SetBool("isThrowing", false);
        }
    }

    private void UpdateParticleSystems() 
    {
        if (playerMovement.IsMoving())
            dustRunningParticles.enableEmission = true;
        else
            dustRunningParticles.enableEmission = false;
    }
}
