using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonoBehaviour = Photon.MonoBehaviour;

public class PlayerController : Photon.MonoBehaviour
{
    [SerializeField] private Transform Orientation;
    
    [SerializeField] private PhotonView PhotonView;
    
    [SerializeField] private Rigidbody RB;
    [SerializeField] private GameObject PlayerCamera;

    [Header("Movement")] 
    [SerializeField] private float MoveSpeed = 6f;
    [SerializeField] private float MovementMultiplier = 10f;
    [SerializeField] private float AirMovementMultiplier = 0.4f;

    [Header("Jumping")] [SerializeField] private float JumpForce = 5f;

    [Header("Drag")] 
    [SerializeField] private float GroundDrag = 6f;

    [SerializeField] private float AirDrag = 2f;

    private bool IsGrounded;
    private float GroundDistance = 0.4f;
    [SerializeField] private LayerMask GroundMask;
    [SerializeField] private Transform GroundCheck;
    
    private float PlayerHeight = 2f;
    
    private float HorizontalMovement;
    private float VerticalMovement;

    private Vector3 MovementDirection;
    private Vector3 SlopeMovementDirection;

    private RaycastHit SlopeHit;

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out SlopeHit, PlayerHeight / 2f + 0.5f))
        {
            if (SlopeHit.normal != Vector3.up)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        return false;
    }
    
    private void Awake()
    {
        if (PhotonView.isMine)
        {
            PlayerCamera.SetActive(true);
        }
    }

    private void Start()
    {
        RB.freezeRotation = true;
    }

    private void Update()
    {
        IsGrounded = Physics.CheckSphere(GroundCheck.position, GroundDistance, GroundMask);
        
        HandleInput();
        ControlDrag();

        if (Input.GetAxisRaw("Jump") > 0 && IsGrounded)
        {
            Jump();
        }
        
        SlopeMovementDirection = Vector3.ProjectOnPlane(MovementDirection, SlopeHit.normal);
    }

    private void HandleInput()
    {
        HorizontalMovement = Input.GetAxisRaw("Horizontal");
        VerticalMovement = Input.GetAxisRaw("Vertical");

        MovementDirection = Orientation.forward * VerticalMovement + Orientation.right * HorizontalMovement;
    }

    private void Jump()
    {
        if (IsGrounded)
        {
            RB.velocity = new Vector3(RB.velocity.x, 0, RB.velocity.z);
            RB.AddForce(transform.up * JumpForce, ForceMode.Impulse);
        }
    }

    private void ControlDrag()
    {
        RB.drag = IsGrounded ? GroundDrag : AirDrag;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        if (IsGrounded && !OnSlope())
        {
            RB.AddForce(MovementDirection.normalized * (MoveSpeed * MovementMultiplier), ForceMode.Acceleration);
        }
        else if (IsGrounded && OnSlope())
        {
            RB.AddForce(SlopeMovementDirection.normalized * (MoveSpeed * MovementMultiplier), ForceMode.Acceleration);
        }
        else
        {
            RB.AddForce(MovementDirection.normalized * (MoveSpeed * MovementMultiplier * AirMovementMultiplier), ForceMode.Acceleration);
        }
    }
}
