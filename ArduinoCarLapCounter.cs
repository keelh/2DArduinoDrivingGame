// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using System.IO.Ports;


// public class ArduinoCarLapCounter : MonoBehaviour
// {
//     private int passedCheckPointNumber = 0;
//     private float timeAtLastPassedCheckpoint = 0;
//     private int numOfPassedCheckpoints = 0;
//     int lapsCompleted = 0;
//     public int lapsToComplete = 1;
//     bool isRaceCompleted = false;

//     public event Action<ArduinoCarLapCounter> OnPassCheckpoint;

//     // Declare and initialize the serial port
//     private SerialPort serialPort;

//     void Start()
//     {
//         // Initialize the serial port
//         serialPort = new SerialPort("COM3", 9600); // Use the correct port and baud rate
//         try
//         {
//             serialPort.Open();
//             Debug.Log("Serial port opened");
//         }
//         catch (Exception e)
//         {
//             Debug.LogError("Failed to open serial port: " + e.Message);
//         }
//     }

//     void OnTriggerEnter2D(Collider2D collider2D)
//     {
//         if (collider2D.CompareTag("CheckPoint"))
//         {
//             if (isRaceCompleted)
//             {
//                 return;
//             }

//             CheckPoint checkPoint = collider2D.GetComponent<CheckPoint>();

//             if (passedCheckPointNumber + 1 == checkPoint.checkPointNumber)
//             {
//                 passedCheckPointNumber = checkPoint.checkPointNumber;
//                 numOfPassedCheckpoints++;
//                 timeAtLastPassedCheckpoint = Time.time;

//                 if (checkPoint.isFinishLine)
//                 {
//                     passedCheckPointNumber = 0;
//                     lapsCompleted++;

//                     if (lapsCompleted >= lapsToComplete)
//                     {
//                         Debug.Log("The race has finished");
//                         isRaceCompleted = true;
//                         TopDownCarController carController = GetComponent<TopDownCarController>();
//                         if (carController != null)
//                         {
//                             carController.FinishRace();
//                         }
//                     }

//                     // Send a signal to the Arduino
//                     SendSignalToArduino();
//                 }

//                 OnPassCheckpoint?.Invoke(this);
//             }
//         }
//     }

//     void SendSignalToArduino()
//     {
//         if (serialPort != null && serialPort.IsOpen)
//         {
//             try
//             {
//                 serialPort.WriteLine("LapCompleted");
//                 Debug.Log("Signal sent to Arduino");
//             }
//             catch (Exception e)
//             {
//                 Debug.LogError("Failed to send signal to Arduino: " + e.Message);
//             }
//         }
//         else
//         {
//             Debug.LogWarning("Serial port is not open");
//         }
//     }

//     void OnDestroy()
//     {
//         // Ensure the serial port is closed when the object is destroyed
//         if (serialPort != null && serialPort.IsOpen)
//         {
//             serialPort.Close();
//         }
//     }
// }
