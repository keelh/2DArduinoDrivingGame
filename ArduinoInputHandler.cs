using System.IO.Ports;
using UnityEngine;

public class ArduinoInputHandler : MonoBehaviour
{
    SerialPort serialPort = new SerialPort("COM6", 9600);  // Update COM port and baud rate as needed
    TopDownCarController carController;

    void Start()
    {
        carController = GetComponent<TopDownCarController>();
        serialPort.Open();
    }

    void Update()
    {
        if (serialPort.IsOpen)
        {
            try
            {
                string data = serialPort.ReadLine();
                string[] values = data.Split(',');

                if (values.Length == 2)
                {
                    float xInput = float.Parse(values[0]);
                    float yInput = float.Parse(values[1]);

                    Vector2 inputVector = new Vector2(xInput, yInput);
                    carController.SetInputVector(inputVector);
                }
            }
            catch (System.Exception)
            {
                // Handle exceptions
            }
        }
    }

    void OnApplicationQuit()
    {
        if (serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}
