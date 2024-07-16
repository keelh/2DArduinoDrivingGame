using UnityEditor.Callbacks;
using UnityEngine;

public class TopDownCarController : MonoBehaviour
{
    public float accelerationFactor = 30.0f;
    public float turnFactor = 3.5f;
    float rotationAngle = 0;
    float accelerationInput = 0;
    float steeringInput = 0;


    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    
    void FixedUpdate()
    {
        ApplyEngineForce();
        ApplySteering();
    }

    void ApplyEngineForce(){
        Vector2 engineForceVector = transform.up * accelerationInput * accelerationFactor;
        rb.AddForce(engineForceVector, ForceMode2D.Force);
    }

     void ApplySteering(){
        rotationAngle -= steeringInput * turnFactor;
        rb.MoveRotation(rotationAngle);
    }
    
    public void SetInputVector(Vector2 inputVector){
        steeringInput = inputVector.x;
        accelerationInput = inputVector.y;
    }
}

