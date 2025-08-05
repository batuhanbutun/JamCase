using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts;
using DG.Tweening;
using UnityEngine;

public class BusManager : Singleton<BusManager>,IPassengerReceiver
{
    [SerializeField] private Bus busPrefab;
    [SerializeField] private Transform frontSlot;
    [SerializeField] private Transform backSlot;
    
    [SerializeField] private List<ObjColor> busColorSequence; 
    private int currentIndex = 0;
    private Bus currentBus;
    private Bus backBus;

    public System.Action<Bus> BusReadyToDepart;
    public ObjColor CurrentBusColor => currentBus.ObjColor;
    public bool busDeparting;
    private void Awake()
    {
        Bus.PassengerOnBus = null;
        BusReadyToDepart = null;
        Bus.PassengerOnBus += IsCurrentBusFull;
    }
    
    public void SpawnInitialBuses(List<ObjColor> colorSequence)
    {
        busColorSequence = colorSequence;
        
        if (currentIndex < busColorSequence.Count)
            currentBus = SpawnBus(busColorSequence[currentIndex++], frontSlot.position);

        if (currentIndex < busColorSequence.Count)
            backBus = SpawnBus(busColorSequence[currentIndex++], backSlot.position);
    }
    
    private Bus SpawnBus(ObjColor color, Vector3 position)
    {
        Bus spawnedBuss = Instantiate(busPrefab, position, Quaternion.identity);
        spawnedBuss.ColorSetup(color);
        return spawnedBuss;
    }

    public bool TryReceive(Passenger passenger)
    {
        if (currentBus == null) return false;
        if (passenger.ObjColor != currentBus.ObjColor) return false;
        if (currentBus.IsBusReserved) return false;
        
        currentBus.SetSeatForPassenger(passenger);
        return true;
    }
    
    IEnumerator HandleBusDeparture()
    {
        busDeparting = true;
        yield return new WaitForSeconds(0.5f);
        var toMovingBus = currentBus;
        yield return toMovingBus.MoveTo(currentBus.transform.position + Vector3.right * 10f,() => Destroy(toMovingBus.gameObject));
        
        currentBus = null;
        
        if (backBus != null)
        {
            yield return backBus.MoveTo(frontSlot.position, () =>
            {
                currentBus = backBus;
                backBus = null;
                if (currentIndex < busColorSequence.Count)
                    backBus = SpawnBus(busColorSequence[currentIndex++], backSlot.position);
            
                BusReadyToDepart?.Invoke(currentBus);
                busDeparting = false;
            });
        }
        else
            GameManager.Instance.GameWin();
    }

    private void IsCurrentBusFull()
    {
        if (currentBus.IsBusFull)
            StartCoroutine(HandleBusDeparture());
        
    }

    public bool IsCurrentBusAvailable(Passenger p)
    {
        if(busDeparting) return false;
        if(p.ObjColor != currentBus.ObjColor || currentBus.IsBusReserved) return false;
        return true;
    }
    
    
   
    
}
