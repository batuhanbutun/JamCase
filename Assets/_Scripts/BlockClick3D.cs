using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockClick3D : MonoBehaviour
{
    private int x, z;
    private GridManager3D grid;
    private Passenger passenger;
    public void Init(int x, int z, GridManager3D grid)
    {
        this.x = x;
        this.z = z;
        this.grid = grid;
        passenger = GetComponent<Passenger>();
    }

    private void OnMouseDown()
    {
        IsPathClear();
    }

    private void IsPathClear()
    {
        Vector2Int start = new Vector2Int(x, z);
        List<Vector2Int> path = GridMovementPathfinder.GetPathToTop(start);
        if (path != null)
        {
            var busAvailable = BusManager.Instance.IsCurrentBusAvailable(passenger);
            var waitingAreaAvailable = WaitingAreaManager.Instance.IsAvailable();
            if(!waitingAreaAvailable && !busAvailable) return;
            
            GridMovementPathfinder.EvaluatePassengerPaths();
            GridManager3D.Instance.gridPassengers[x, z] = null;
            var passengerMovementController = GetComponent<PassengerMovementController>();
            GetComponent<IOutlinable>().OutlineSet(false);
            
            Vector3[] worldPath = GridMovementPathfinder.GetPathWorldPoints(path);
            
            StartCoroutine(passengerMovementController.FollowPath(worldPath, () =>
            {
                PassengerFlowController.Instance.RoutePassenger(passenger);
            }));
            
        }
           
    }
}
