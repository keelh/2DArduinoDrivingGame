using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CarLapCounter : MonoBehaviour
{
    private int passedCheckPointNumber = 0;
    private float timeAtLastPassedCheckpoint = 0;
    private int numOfPassedCheckpoints = 0;
    int lapsCompleted = 0;
    public int lapsToComplete = 1;
    bool isRaceCompleted = false;

    public event Action<CarLapCounter> OnPassCheckpoint;

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
                numOfPassedCheckpoints++;
                timeAtLastPassedCheckpoint = Time.time;

                if (checkPoint.isFinishLine)
                {
                    passedCheckPointNumber = 0;
                    lapsCompleted++;

                    if (lapsCompleted >= lapsToComplete)
                    {
                        Debug.Log("The race has finished");
                        isRaceCompleted = true;
                        TopDownCarController carController = GetComponent<TopDownCarController>();
                        if (carController != null)
                        {
                            carController.FinishRace(); // Inform the car controller
                        }
                    }
                }

                OnPassCheckpoint?.Invoke(this);
            }
        }
    }
}
