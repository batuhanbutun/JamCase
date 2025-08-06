using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts;
using UnityEngine;

public class PassengerFlowController : Singleton<PassengerFlowController>
{
   public List<Passenger> passengerOnMovingGrid;

   private void Start()
   {
      GridEvents.OnPassengerSent += AddPassengerToFlow;
      GridEvents.OnPassengerExitGrid += RemovePassengerToFlow;
      BusManager.Instance.BusReadyToDepart += ClickControl;
      GameManager.Instance.OnGameStart += ClickManager.UnlockClick;
      Bus.PassengerOnBus += ClickControl;
   }

   private void AddPassengerToFlow(Passenger passenger,Vector2Int gridPos)
   {
      passengerOnMovingGrid.Add(passenger);
         ClickControl();
   }
   
   private void RemovePassengerToFlow(Passenger passenger)
   {
      passengerOnMovingGrid.Remove(passenger);
      if(!BusManager.Instance.busDeparting)
         ClickControl();
   }

   private void ClickControl(Bus bus = null)
   {
      if(passengerOnMovingGrid.Count >= WaitingAreaManager.Instance.EmptyBenchCount())
         ClickManager.LockClick();
      else
         ClickManager.UnlockClick();
   }
   
   private void ClickControl()
   {
      if(passengerOnMovingGrid.Count >= WaitingAreaManager.Instance.EmptyBenchCount())
         ClickManager.LockClick();
      else
         ClickManager.UnlockClick();
   }
}
