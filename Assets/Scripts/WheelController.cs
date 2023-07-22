using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelController : PortalTraveller
{

    [SerializeField] private WheelCollider frontRight;
    [SerializeField] private WheelCollider frontLeft;
    [SerializeField] private WheelCollider backRight;
    [SerializeField] private WheelCollider backLeft;

    [SerializeField] private Transform frontRightTransform;
    [SerializeField] private Transform frontLeftTransform;
    [SerializeField] private Transform backRightTransform;
    [SerializeField] private Transform backLeftTransform;

    public float acceleration = 500f;
    public float breakForce = 300f;

    private float currentAcceleration = 0f;
    private float currentBreakforce = 0f;

    public float maxTurnAngle = 15f;
    private float currentTurnAngle = 0f;


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
            transform.rotation = Quaternion.identity;
    }

    private void FixedUpdate()
    {
        currentAcceleration = acceleration * Input.GetAxis("Vertical");
        
        //apply breakforce if key is held
        if (Input.GetKey(KeyCode.Space))
            currentBreakforce = breakForce;
        else
            currentBreakforce = 0f;

        //2-wheel drive car, apply force to front wheels
        frontLeft.motorTorque = currentAcceleration;
        frontRight.motorTorque = currentAcceleration;
        
        //Apply break to all wheels
        frontLeft.brakeTorque = currentBreakforce;
        frontRight.brakeTorque = currentBreakforce;
        backRight.brakeTorque = currentBreakforce;
        backLeft.brakeTorque = currentBreakforce;

        currentTurnAngle = maxTurnAngle * Input.GetAxis("Horizontal");
        frontLeft.steerAngle = currentTurnAngle;
        frontRight.steerAngle = currentTurnAngle;
        
        UpdateWheel(frontRight, frontRightTransform);
        UpdateWheel(frontLeft, frontLeftTransform);
        UpdateWheel(backRight, backRightTransform);
        UpdateWheel(backLeft, backLeftTransform);
        
    }


    void UpdateWheel(WheelCollider col, Transform trans)
    {
        Vector3 position;
        Quaternion rotation;
        col.GetWorldPose(out position, out rotation);

        trans.position = position;
        trans.rotation = rotation;
    }
    
}
