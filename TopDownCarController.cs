using UnityEditor.Callbacks;
using UnityEngine;

public class TopDownCarController : MonoBehaviour
{
    public float accelerationFactor = 30.0f;
    public float turnFactor = 3.5f;
    public float driftFactor = 0.8f;
    public float maxSpeed = 20.0f;
    float rotationAngle = 0;
    float accelerationInput = 0;
    float steeringInput = 0;
    float velocityVsUp = 0;

    Rigidbody2D rb;
    bool isRaceCompleted = false; // Add this line

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (isRaceCompleted)
        {
            rb.velocity = Vector2.zero; // Stop the car
            rb.angularVelocity = 0; // Stop any rotation
            return; // Skip further processing
        }

        ApplyEngineForce();
        KillOrthogonalVelocity();
        ApplySteering();
    }

    void KillOrthogonalVelocity()
    {
        Vector2 forwardVelocity = transform.up * Vector2.Dot(rb.velocity, transform.up);
        Vector2 rightVelocity = transform.right * Vector2.Dot(rb.velocity, transform.right);

        rb.velocity = forwardVelocity + rightVelocity * driftFactor;
    }

    void ApplyEngineForce()
    {
        velocityVsUp = Vector2.Dot(transform.up, rb.velocity);

        if (velocityVsUp > maxSpeed && accelerationInput > 0)
        {
            return;
        }

        if (velocityVsUp < -maxSpeed * 0.5f && accelerationInput < 0)
        {
            return;
        }

        if (rb.velocity.sqrMagnitude > maxSpeed * maxSpeed && accelerationInput > 0)
        {
            return;
        }

        if (accelerationInput <= 0.5 && accelerationInput >= -0.5)
        {
            rb.drag = Mathf.Lerp(rb.drag, 3.0f, Time.fixedDeltaTime * 3);
        }
        else
        {
            rb.drag = 0;
        }

        Vector2 engineForceVector = transform.up * accelerationInput * accelerationFactor;
        rb.AddForce(engineForceVector, ForceMode2D.Force);
    }

    void ApplySteering()
    {
        if (steeringInput < 0.3 && steeringInput > -0.3){
            return;
        } else {
            rotationAngle -= steeringInput * turnFactor;
            rb.MoveRotation(rotationAngle);
        }
    }

    public void SetInputVector(Vector2 inputVector)
    {
        steeringInput = inputVector.x;
        accelerationInput = inputVector.y;
    }

    public void FinishRace() // Add this method
    {
        isRaceCompleted = true;
    }
}
