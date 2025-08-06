using System.Collections;
using System.Collections.Generic;
using _Scripts;
using UnityEngine;

public class PassengerClick : MonoBehaviour
{
    private int x, z;
    private GridManager3D grid;
    private Passenger passenger;
    private PassengerStateMachine stateMachine;
    private bool clicked = false;
    public void Init(int x, int z, GridManager3D grid)
    {
        this.x = x;
        this.z = z;
        this.grid = grid;
        passenger = GetComponent<Passenger>();
        passenger.gridPos = new Vector2Int(x, z);
        stateMachine = GetComponent<PassengerStateMachine>();
    }

    private void OnMouseDown()
    {
        if(!ClickManager.ClickLock && !clicked)
            IsPathClear();
    }

    private bool IsPathClear()
    {
        List<Vector2Int> path = GridMovementPathfinder.GetPathToTop(new Vector2Int(x, z));
        var busAvailable = BusManager.Instance.IsCurrentBusAvailable(passenger);
        var waitingAreaAvailable = WaitingAreaManager.Instance.IsAvailable();
        if(!waitingAreaAvailable && !busAvailable) return false;
        if (path == null) return false;
        
        var moveOutState = new MovingOutGridState(passenger, x, z,path);
        stateMachine.ChangeState(moveOutState);
        clicked = true;
        return true;
    }
}
