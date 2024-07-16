using UnityEditor.Callbacks;
using UnityEngine;

public class TopDownCarController : MonoBehaviour
{
    public float accelerationFactor = 30.0f;
    public float turnFactor = 3.5f;
    public float driftFactor = 0.95f;
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

        KillOrthogonalVelocity();

        ApplySteering();
    }

    void KillOrthogonalVelocity(){
        // gives a values for how much forward velocity a car has
        Vector2 forwardVelocity = transform.up * Vector2.Dot(rb.velocity, transform.up);

        // gives a values for how much right velocity a car has
        Vector2 rightVelocity = transform.right * Vector2.Dot(rb.velocity, transform.right);

        rb.velocity = forwardVelocity + rightVelocity * driftFactor;
    }

    void ApplyEngineForce()
    {
        //applying some resistance/drag when the user isnt accelerating to stop the car slowly
        if (accelerationInput == 0){
            rb.drag = Mathf.Lerp(rb.drag, 3.0f, Time.fixedDeltaTime * 3);
        } else {
            rb.drag = 0;
        }

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

