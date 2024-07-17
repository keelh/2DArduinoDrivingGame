using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PositionHandler : MonoBehaviour
{
    public List<CarLapCounter> carLapCounters = new List<CarLapCounter>();

    void Start()
    {
        // Get all car lap counters in the scene
        CarLapCounter[] carLapCounterArray = FindObjectsOfType<CarLapCounter>();

        // Store the lap counters in a list
        carLapCounters = carLapCounterArray.ToList<CarLapCounter>();

        // Hook up the passed checkpoint event
        foreach (CarLapCounter lapCounter in carLapCounters)
            lapCounter.OnPassCheckpoint += OnPassCheckpoint;
        
    }

    void OnPassCheckpoint(CarLapCounter carLapCounter)
    {
        Debug.Log("Event: Car passed a checkpoint");
    }
}
