using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.XR;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float drag;
    [SerializeField] private float gravityMod;
    [SerializeField] private float slopeGravityMod;
    [SerializeField] private float cargoGravityMod;
    [SerializeField] private float distanceToSlope;

    [Header("Hold Settings")]
    [SerializeField] private float throwForce;
    [SerializeField] private float throwForceUp;
    [SerializeField] private float holdMagnitude, holdDistance, holdFollowSpeed, rotationSpeed, holdMoveOffset;

    [Header("References")]
    [SerializeField] private Transform forwardDir;
    [SerializeField] private Transform playerHoldTransform;
    [SerializeField] private PlayerHold playerHoldScript;
    [SerializeField] private int holdLayer;
    [SerializeField] private int cargoLayer;

    private GameManagement gameManagement;
    private Rigidbody body;
    private Vector3 playerDir;
    private bool leftClickInput;
    private bool rightClickInput;
    [NonSerialized] public Vector2 moveInput;
    [NonSerialized] public bool isHolding;
    private GameObject holdPlaceholderObj;

    void Start()
    {
        gameManagement = GameObject.Find("Game Management").GetComponent<GameManagement>();
        playerDir = transform.forward.normalized * -1;
        body = GetComponent<Rigidbody>();
    }
    
    void Update()
    {
        GetInput();
        Rotate();
        Hold();
    }

    void FixedUpdate()
    {
        Move();
        HoldMovement();
        ExtraGravity();
    }

    private void GetInput() 
    {
        if (!gameManagement.isPlaying) return;

        // Player input
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        leftClickInput = Input.GetButtonUp("Fire1");
        rightClickInput = Input.GetButtonUp("Fire2");
    }

    private void Move() 
    {
        // Fix speed on diagonal input
        if (moveInput.magnitude > 1)
            moveInput = moveInput.normalized;

        // Check for input
        if (moveInput.magnitude != 0)
        {
            // Desired direction to move relative to camera 
            playerDir = (forwardDir.right * moveInput.x) + (forwardDir.forward * moveInput.y);
            // Movement towards direction
            body.AddForce(playerDir * speed);
        }

        // Drag
        body.velocity = new Vector3(body.velocity.x * drag, body.velocity.y, body.velocity.z * drag);
        
    }

    private void ExtraGravity() 
    {
        // Apply extra gravity more realistic movement 
        if (IsOnSlope())
            body.AddForce(Vector3.down * slopeGravityMod);
        else
            body.AddForce(Vector3.down * slopeGravityMod);
    }

    private void Rotate() 
    {
        // Rotate towards direction player is moving towards
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerDir * -1), rotateSpeed * Time.deltaTime);
    }

    private void Hold()
    {
        // Toggle holding
        if (playerHoldScript.canHold && leftClickInput && !isHolding)
        {
            StartHold();
        }
        else if (leftClickInput && isHolding)
        {
            EndHold();
        }

        // If holding and pressing right click, throw towards player dir
        if (isHolding && rightClickInput)
        {
            EndHold();
            playerHoldScript.currentObject.GetComponent<Rigidbody>().AddForce((playerDir * throwForce) + (Vector3.up * throwForceUp), ForceMode.Impulse);
        }
    }

    private void StartHold() 
    {
        // Do once on start of holding
        isHolding = true;

        // Create placeholder obj in cargo object
        holdPlaceholderObj = Instantiate(new GameObject(), GameObject.Find("Cargo").transform);

        playerHoldScript.currentObject.GetComponent<ConstantForce>().force = Vector3.zero;
        playerHoldScript.currentObject.GetComponent<Rigidbody>().freezeRotation = true;
        playerHoldScript.currentObject.parent = playerHoldTransform;

        playerHoldScript.currentObject.gameObject.layer = holdLayer;
    }

    private void EndHold()
    {
        // Do once on end of holding
        isHolding = false;

        // Destroy placeholder obj in cargo object
        if (holdPlaceholderObj != null)
            Destroy(holdPlaceholderObj);

        playerHoldScript.currentObject.GetComponent<Rigidbody>().freezeRotation = false;
        playerHoldScript.currentObject.GetComponent<ConstantForce>().force = new Vector3(0, -cargoGravityMod, 0);
        playerHoldScript.currentObject.parent = GameObject.Find("Cargo").transform;

        playerHoldScript.currentObject.gameObject.layer = cargoLayer;
    }

    void HoldMovement()
    {
        if (!isHolding) return;

        // HANDLE POSITION:
        Vector3 targetPosition = playerHoldTransform.position;
        if (IsMoving())
            targetPosition = playerHoldTransform.position + (playerHoldTransform.forward * holdMoveOffset);

        Rigidbody objectHeld = playerHoldScript.currentObject.GetComponent<Rigidbody>();
        Vector3 motionVector = targetPosition - objectHeld.position;

        Vector3 targetVelocity = Vector3.MoveTowards(objectHeld.velocity, motionVector * holdMagnitude, (motionVector.magnitude / holdDistance) * holdFollowSpeed);
        if ((targetVelocity - objectHeld.velocity).magnitude < 20.0f)
        {
            objectHeld.velocity = targetVelocity;
        }
        else
        {
            EndHold();
            return;
        }

        // HANDLE ROTATION:
        if (objectHeld.angularVelocity.magnitude < 20f)
        {
            objectHeld.MoveRotation(Quaternion.Slerp(objectHeld.rotation, Quaternion.LookRotation(transform.forward), Time.deltaTime * rotationSpeed));
        }
    }

    private bool IsOnSlope()
    {
        Debug.DrawRay(transform.position + (transform.forward * 0.5f), Vector3.down * distanceToSlope);
        Debug.DrawRay(transform.position - (transform.forward * 0.5f), Vector3.down * distanceToSlope);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, distanceToSlope))
            if (hit.normal != Vector3.up)
                return true;

        return false;
    }

    public bool IsMoving() 
    {
        if (body.velocity.magnitude > 3f)
            return true;
        else
            return false;
    }

    
}
