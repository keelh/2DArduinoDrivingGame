using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PositionHandler : MonoBehaviour
{
    public List<ArduinoInputHandler> carLapCounters = new List<ArduinoInputHandler>();   
    void Start()
    {
        // Get all car lap counters in the scene
        ArduinoInputHandler[] carLapCounterArray = FindObjectsOfType<ArduinoInputHandler>();

        // Store the lap counters in a list
        carLapCounters = carLapCounterArray.ToList<ArduinoInputHandler>();

        // Hook up the passed checkpoint event
        foreach (ArduinoInputHandler lapCounter in carLapCounters)
            lapCounter.OnPassCheckpoint += OnPassCheckpoint;
        
    }

    void OnPassCheckpoint(ArduinoInputHandler carLapCounter)
    {
        Debug.Log("Event: Car passed a checkpoint");
    }
}
