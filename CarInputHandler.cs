using UnityEditor.Callbacks;
using UnityEngine;

public class CarInputHandler : MonoBehaviour
{
    TopDownCarController rb;
    void Awake()
    {
        rb = GetComponent<TopDownCarController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 inputVector = Vector2.zero;

        inputVector.y = Input.GetAxis("Vertical");
        inputVector.x = Input.GetAxis("Horizontal");

        rb.SetInputVector(inputVector);
    }
}
