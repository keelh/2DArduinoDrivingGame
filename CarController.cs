using UnityEngine;

public class CarController : MonoBehaviour
{
    public float speed = 10f;
    public float turnSpeed = 50f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Get input from keyboard
        float moveInput = Input.GetAxis("Vertical");
        float turnInput = Input.GetAxis("Horizontal");

        // Apply forward and backward movement
        rb.velocity = transform.up * moveInput * speed;

        // Apply rotation
        if (moveInput != 0 || turnInput != 0)
        {
            float rotation = -turnInput * turnSpeed * Time.deltaTime * Mathf.Sign(moveInput);
            rb.MoveRotation(rb.rotation + rotation);
        }
    }
}
