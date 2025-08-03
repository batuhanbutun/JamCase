using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts;
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
        if (passenger.ObjColor != currentBus.ObjColor) return false;
        if (currentBus == null || currentBus.IsBusFull)
            return false;

        currentBus.SetSeatForPassenger(passenger);
        if (currentBus.IsBusFull)
            StartCoroutine(HandleBusDeparture());
        
        
        return true;
    }
    
    IEnumerator HandleBusDeparture()
    {
        yield return new WaitForSeconds(0.5f);

        currentBus.PlayDeparture(() =>
        {
            Destroy(currentBus.gameObject);
        });

        yield return new WaitForSeconds(1f);
        currentBus = null;

        // Back öne gelsin
        if (backBus != null)
        {
            currentBus = backBus;
            backBus = null;
            currentBus.MoveTo(frontSlot.position);

            yield return new WaitForSeconds(1f);

            if (currentIndex < busColorSequence.Count)
                backBus = SpawnBus(busColorSequence[currentIndex++], backSlot.position);
            
            WaitingAreaManager.Instance.TryGivePassengersToBus(currentBus);
        }
    }
    
}
