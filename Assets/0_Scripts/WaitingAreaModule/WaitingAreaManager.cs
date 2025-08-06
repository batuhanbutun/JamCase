using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class WaitingAreaManager : Singleton<WaitingAreaManager>, IPassengerReceiver
{
    [SerializeField] private List<Transform> waitingAreaGrids;
    private Passenger[] waitingPassengers;
    private int areaCapacity = 0;
    private int waitingPassengerCount = 0;
    private Coroutine passengerMoveCoroutine;
    private void Awake()
    {
        areaCapacity = waitingAreaGrids.Count;
        waitingPassengers = new Passenger[areaCapacity];
    }

    public void SetPassengerToBench()
    {
        waitingPassengerCount++;
        DOVirtual.DelayedCall(1f, () =>
        {
            if (IsFull() && !BusManager.Instance.busDeparting)
                GameManager.Instance.GameFail();
        });
    }

    public bool TryReceive(Passenger passenger)
    {
        for (int i = 0; i < areaCapacity; i++)
            if (waitingPassengers[i] == null)
                return true;
        
        return false;
    }

    public Vector3 ReserveBenchForPassenger(Passenger passenger,out int benchIndex)
    {
        Vector3 pos = Vector3.zero;
        for (int i = 0; i < areaCapacity; i++)
        {
            if (waitingPassengers[i] == null)
            {
                waitingPassengers[i] = passenger;
                benchIndex = i;
                pos = waitingAreaGrids[i].transform.position;
                return pos;
            }
        }

        benchIndex = -1;
        return pos;
    }

    public void ReleaseBench(int benchIndex)
    {
        waitingPassengers[benchIndex] = null;
        waitingPassengerCount--;
    }
    
    private bool IsFull()
    {
        if(waitingPassengerCount >= areaCapacity)
            return true;
        return false;
    }

    public bool IsAvailable()
    {
        foreach (var passenger in waitingPassengers)
        {
            if (passenger == null)
                return true;
        }
        return false;
    }

    public int EmptyBenchCount()
    {
        int emptyCount = 0;
        foreach (var p in waitingPassengers)
        {
            if(p == null)
                emptyCount++;
        }

        return emptyCount;
    }
    
    
}
