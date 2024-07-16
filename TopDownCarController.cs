using UnityEditor.Callbacks;
using UnityEngine;

public class TopDownCarController : MonoBehaviour
{
    public float accelerationFactor = 30.0f;
    public float turnFactor = 3.5f;
    public float driftFactor = 0.95f;
    public float maxSpeed = 20.0f;
    float rotationAngle = 0;
    float accelerationInput = 0;
    float steeringInput = 0;
    float velocityVsUp = 0;



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
        //calcing how "foreward" we are going in terms of our velocity
        velocityVsUp = Vector2.Dot(transform.up, rb.velocity);

        //limits that caps speed
        if (velocityVsUp > maxSpeed && accelerationInput > 0){
            return; //doesnt applt any more force and exits the function
        }

        //caps revesing speed to 50% of max speed
        if (velocityVsUp < -maxSpeed * 0.5f && accelerationInput < 0){
            return;
        }

        //another case in case we are being pushed/sliding
        if (rb.velocity.sqrMagnitude > maxSpeed * maxSpeed && accelerationInput > 0){
            return;
        }
        //applying some resistance/drag when the user isnt accelerating to stop the car slowly
        if (accelerationInput <= 0.2 && accelerationInput >= -0.2){
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

