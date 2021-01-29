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
    [SerializeField] private float distanceToHeavyCargo;

    [Header("Hold Settings")]
    [SerializeField] private float throwForce;
    [SerializeField] private float throwForceUp;
    [SerializeField] private float holdMagnitude, holdDistance, holdFollowSpeed, rotationSpeed, holdMoveOffset;

    [Header("References")]
    [SerializeField] private Transform forwardDir;
    [SerializeField] private Transform playerHoldTransform;
    [SerializeField] private PlayerHold playerHoldScript;
    [SerializeField] private GameObject holdPlaceholderPrefab;
    [SerializeField] private int holdLayer;
    [SerializeField] private int cargoLayer;
    [SerializeField] private LayerMask heavyCargoLayer;

    private GameManagement gameManagement;
    private Rigidbody body;
    private Vector3 playerDir;
    private bool leftClickInput;
    private bool rightClickInput;
    [NonSerialized] public Vector2 moveInput;
    [NonSerialized] public bool isHolding;
    [NonSerialized] public bool isThrowing;
    private Transform currentHeldObj;

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
        if (!gameManagement.isPlaying) {
            moveInput = Vector2.zero;
            return;
        }


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
            EndHold(false);
        }

        // If holding and pressing right click, throw towards player dir
        if (isHolding && rightClickInput)
        {
            EndHold(true);
        }
    }

    private void StartHold() 
    {
        // Do once on start of holding
        isHolding = true;
        currentHeldObj = playerHoldScript.currentObject;

        if (currentHeldObj.GetComponent<Rigidbody>().isKinematic)
            currentHeldObj.GetComponent<Rigidbody>().isKinematic = false;

        currentHeldObj.GetComponent<ConstantForce>().force = Vector3.zero;
        currentHeldObj.GetComponent<Rigidbody>().freezeRotation = true;
        currentHeldObj.parent = playerHoldTransform;
        currentHeldObj.gameObject.layer = holdLayer;
    }

    private void EndHold(bool doesThrow)
    {
        // Do once on end of holding

        currentHeldObj.GetComponent<ConstantForce>().force = new Vector3(0, -cargoGravityMod, 0);
        currentHeldObj.GetComponent<Rigidbody>().freezeRotation = false;
        currentHeldObj.parent = GameObject.Find("Cargo").transform;
        currentHeldObj.gameObject.layer = cargoLayer;

        // Throw object
        if (doesThrow)
        {
            isThrowing = true;
            currentHeldObj.GetComponent<Rigidbody>().AddForce((playerDir * throwForce) + (Vector3.up * throwForceUp), ForceMode.Impulse);
        }

        currentHeldObj = null;
        isHolding = false;
    }

    void HoldMovement()
    {
        if (!isHolding) return;

        // HANDLE POSITION:
        Vector3 targetPosition = playerHoldTransform.position;
        if (IsMoving())
            targetPosition = playerHoldTransform.position + (playerHoldTransform.forward * holdMoveOffset);

        Rigidbody objectHeld = currentHeldObj.GetComponent<Rigidbody>();
        Vector3 motionVector = targetPosition - objectHeld.position;

        Vector3 targetVelocity = Vector3.MoveTowards(objectHeld.velocity, motionVector * holdMagnitude, (motionVector.magnitude / holdDistance) * holdFollowSpeed);
        if ((targetVelocity - objectHeld.velocity).magnitude < 20.0f)
        {
            objectHeld.velocity = targetVelocity;
        }
        else
        {
            EndHold(false);
            return;
        }
    }

    public bool IsFacingHeavyCargo() 
    {

        if (Physics.Raycast(transform.position + new Vector3(0, 1.5f, 0), transform.forward*-1, distanceToHeavyCargo, heavyCargoLayer))
        {
            return true;
        }

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
