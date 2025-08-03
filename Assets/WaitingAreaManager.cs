using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class WaitingAreaManager : Singleton<WaitingAreaManager>, IPassengerReceiver
{
    [SerializeField] private List<Transform> waitingAreaGrids;
    private Passenger[] waitingPassengers;
    private int areaCapacity = 0;
    private void Awake()
    {
        areaCapacity = waitingAreaGrids.Count;
        waitingPassengers = new Passenger[areaCapacity];
    }

    public bool TryReceive(Passenger passenger)
    {
        for (int i = 0; i < areaCapacity; i++)
        {
            if (waitingPassengers[i] == null)
            {
                waitingPassengers[i] = passenger;
                passenger.transform.DOMove(waitingAreaGrids[i].transform.position,0.3f).SetEase(Ease.InOutSine);
                return true;
            }
        }

        // Hepsi doluysa game over
        //OnGameOver?.Invoke();
        return false;
    }
    
    public void TryGivePassengersToBus(Bus bus)
    {
        for (int i = 0; i < waitingPassengers.Length; i++)
        {
            if (waitingPassengers[i] != null &&
                waitingPassengers[i].ObjColor == bus.ObjColor)
            {
                if (bus.IsBusFull) break;

                Passenger p = waitingPassengers[i];
                waitingPassengers[i] = null;

                BusManager.Instance.TryReceive(p);
            }
        }
    }
}
