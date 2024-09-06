using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private float driftFactor = 0.95f;
    public float accelerationFactor = 7f;
    public float turnFactor = 5f;
    public float maxSpeed = 5f;



    private float accelerationInput = 0;
    private float steeringInput = 0;
    private float rotateAngle = 0;
    private float velocityVsUp = 0;

    private Rigidbody2D carRigitB2D;

    private void Awake()
    {
        carRigitB2D = GetComponent<Rigidbody2D>();
    }


    private void FixedUpdate()
    {
        ApplyEngineForce();
        KillOrthogonalVelocity();

        ApplySteering();
    }

    void ApplySteering()
    {
        float minSppedBeforeTurn = (carRigitB2D.velocity.magnitude / 8);
        minSppedBeforeTurn = Mathf.Clamp01(minSppedBeforeTurn);
        
        rotateAngle -= steeringInput * turnFactor*minSppedBeforeTurn;
        
        carRigitB2D.MoveRotation(rotateAngle);
    }

    void KillOrthogonalVelocity()
    {
        Vector2 forwardVelocity = transform.up * Vector2.Dot(carRigitB2D.velocity, transform.up);
        Vector2 rightVelocity = transform.right * Vector2.Dot(carRigitB2D.velocity, transform.right);

        carRigitB2D.velocity = forwardVelocity + rightVelocity * driftFactor;

    }

    void ApplyEngineForce()
    {

        velocityVsUp = Vector2.Dot(transform.up, carRigitB2D.velocity);
        if (velocityVsUp > maxSpeed && accelerationInput > 0)
        {
            return;
        }
        if (velocityVsUp < -maxSpeed*0.5f && accelerationInput < 0)
        {
            return;
        }
        if (carRigitB2D.velocity.sqrMagnitude > maxSpeed*maxSpeed&& accelerationInput>0)
        {
            return;
        }

        if (accelerationInput==0)
        {
            carRigitB2D.drag = Mathf.Lerp(carRigitB2D.drag, 2f, Time.fixedTime * 3);
        }
        else
        {
            carRigitB2D.drag = 0;
        }
        Vector2 engineForce = transform.up * accelerationInput * accelerationFactor;
        carRigitB2D.AddForce(engineForce,ForceMode2D.Force);
    }

    public void SetInputVector(Vector2 inputVector)
    {
        steeringInput = inputVector.x;
        accelerationInput = inputVector.y;
    }

}
