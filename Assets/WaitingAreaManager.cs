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
    
    private Coroutine passengerMoveCoroutine;
    private void Awake()
    {
        areaCapacity = waitingAreaGrids.Count;
        waitingPassengers = new Passenger[areaCapacity];
    }

    private void Start()
    {
        BusManager.Instance.BusReadyToDepart += TryGivePassengersToBus;
    }

    public bool TryReceive(Passenger passenger)
    {
        for (int i = 0; i < areaCapacity; i++)
        {
            if (waitingPassengers[i] == null)
            {
                waitingPassengers[i] = passenger;
                passengerMoveCoroutine = StartCoroutine(passenger.GetComponent<IMovable>().MoveTo(waitingAreaGrids[i].position));
                return true;
            }
        }

        // Hepsi doluysa game over
        //OnGameOver?.Invoke();
        return false;
    }
    
    private void TryGivePassengersToBus(Bus bus)
    {
        for (int i = 0; i < waitingPassengers.Length; i++)
        {
            if (waitingPassengers[i] != null && waitingPassengers[i].ObjColor == bus.ObjColor)
            {
                if (bus.IsBusFull) break;

                Passenger p = waitingPassengers[i];
                waitingPassengers[i] = null;

                bool isBusAvailable = BusManager.Instance.TryReceive(p);
                if (isBusAvailable)
                    StopCoroutine(passengerMoveCoroutine);
                
            }
        }
    }
    
    public void ResetArea()
    {
        for (int i = 0; i < waitingPassengers.Length; i++)
        {
            if (waitingPassengers[i] != null)
            {
                Destroy(waitingPassengers[i].gameObject);
                waitingPassengers[i] = null;
            }
        }
    }
    
}
