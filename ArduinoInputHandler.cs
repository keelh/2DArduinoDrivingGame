using System.IO.Ports;
using UnityEngine;


public class ArduinoInputHandler : MonoBehaviour
{
    SerialPort serialPort = new SerialPort("COM6", 9600);  // Update COM port and baud rate as needed
    TopDownCarController carController;

    bool gameStarted = false;

    void Start()
    {
        carController = GetComponent<TopDownCarController>();
        
        if (carController == null) 
        {
            Debug.LogError("TopDownCarController component not found!");
            return;
        }

        try 
        {
            serialPort.Open();
            serialPort.ReadTimeout = 50;
            Debug.Log("Serial port opened successfully.");
        }
        catch (System.Exception e)
        {
            Debug.Log("Failed to open serial port: " + e.Message);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }

        if (serialPort.IsOpen && gameStarted)
        {
            try
            {
                string data = serialPort.ReadLine();
                string[] values = data.Split(',');

                if (values.Length == 2)
                {
                    if (float.TryParse(values[0], out float xInput) && float.TryParse(values[1], out float yInput))
                    {
                        Vector2 inputVector = new Vector2(xInput, yInput);
                        carController.SetInputVector(inputVector);
                    }
                    else 
                    {
                        Debug.LogWarning("Invalid input data: " + data);
                    }
                    // float xInput = float.Parse(values[0]);
                    // float yInput = float.Parse(values[1]);

                    
                }
            }
            catch (System.TimeoutException)
            {
                // Handle exceptions
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error reading from serial port: " + e.Message);
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

    void StartGame() 
    {
        if (serialPort.IsOpen) 
        {
            try
            {
                serialPort.Write("S");
                gameStarted = true;
                Debug.Log("Start command sent. Game started.");
            }
            catch (System.Exception e) 
            {
                Debug.LogError("Failed to send start command: " + e.Message);
            }
            
        }
    }
}

