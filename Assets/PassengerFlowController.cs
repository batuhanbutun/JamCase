using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerFlowController : Singleton<PassengerFlowController>
{
    public void RoutePassenger(Passenger passenger)
    {
        if (BusManager.Instance.TryReceive(passenger))
            return;

        if (WaitingAreaManager.Instance.TryReceive(passenger))
            return;

        Debug.Log("Game Over!");
        // GameOverController.Instance.TriggerGameOver();
    }
}
