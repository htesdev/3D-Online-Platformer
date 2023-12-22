using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerLook : Photon.MonoBehaviour
{
    [SerializeField] private PhotonView PhotonView;
    
    [SerializeField] private float SensX;
    [SerializeField] private float SensY;

    [SerializeField] private Camera cam;

    [SerializeField] private Transform Orientation;
    
    private float MouseX;
    private float MouseY;

    private float Multiplier = 0.01f;

    private float XRot;
    private float YRot;

    private void Start()
    {
    }

    private void Update()
    {
        HandleInput();
        
        cam.transform.rotation = Quaternion.Euler(XRot, YRot, 0);
        Orientation.transform.rotation = Quaternion.Euler(0, YRot, 0);
    }

    private void HandleInput()
    {
        if (PhotonView.isMine)
        {
            MouseX = Input.GetAxisRaw("Mouse X");
            MouseY = Input.GetAxisRaw("Mouse Y");

            YRot += MouseX * SensX * Multiplier;
            XRot -= MouseY * SensY * Multiplier;

            XRot = Mathf.Clamp(XRot, -90f, 90f);
        }
    }
}
