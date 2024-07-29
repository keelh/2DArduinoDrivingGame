using System;
using System.IO.Ports;
using UnityEngine;

public class ArduinoInputHandler : MonoBehaviour
{
    SerialPort serialPort = new SerialPort("COM6", 9600);  // Update COM port and baud rate as needed
    TopDownCarController carController;
    public event Action<ArduinoInputHandler> OnPassCheckpoint;

    bool gameStarted = false;

    private int passedCheckPointNumber = 0;
    int lapsCompleted = 0;
    public int lapsToComplete = 1;
    bool isRaceCompleted = false;

    void Start()
    {
        // Initialize carController here
        carController = GetComponent<TopDownCarController>();

        // Open the serial port
        if (!serialPort.IsOpen)
        {
            try
            {
                serialPort.Open();
                serialPort.ReadTimeout = 100; // Set a timeout for reading
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to open serial port: " + e.Message);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.CompareTag("CheckPoint"))
        {
            if (isRaceCompleted)
            {
                return;
            }

            CheckPoint checkPoint = collider2D.GetComponent<CheckPoint>();

            if (passedCheckPointNumber + 1 == checkPoint.checkPointNumber)
            {
                passedCheckPointNumber = checkPoint.checkPointNumber;

                if (checkPoint.isFinishLine)
                {
                    passedCheckPointNumber = 0;
                    lapsCompleted++;

                    if (lapsCompleted >= lapsToComplete)
                    {
                        Debug.Log("The race has finished");
                        isRaceCompleted = true;

                        if (carController != null)
                        {
                            carController.FinishRace();
                        }
                    }

                    // Send a signal to the Arduino
                    SendSignalToArduino();
                }

                OnPassCheckpoint?.Invoke(this);
            }
        }
    }

    void SendSignalToArduino()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            try
            {
                serialPort.WriteLine("L");
                Debug.Log("Signal sent to Arduino");
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to send signal to Arduino: " + e.Message);
            }
        }
        else
        {
            Debug.LogWarning("Serial port is not open");
        }
    }

    void OnDestroy()
    {
        // Ensure the serial port is closed when the object is destroyed
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Write("X");
            gameStarted = false;
            serialPort.Close();
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
                        if (carController != null)
                        {
                            carController.SetInputVector(inputVector);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Invalid input data: " + data);
                    }
                }
            }
            catch (TimeoutException)
            {
                // Handle timeouts or empty reads
            }
            catch (Exception e)
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
            catch (Exception e)
            {
                Debug.LogError("Failed to send start command: " + e.Message);
            }
        }
        else
        {
            Debug.LogWarning("Serial port is not open");
        }
    }
}
