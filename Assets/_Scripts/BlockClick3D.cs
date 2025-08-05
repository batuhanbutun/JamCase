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
        GetComponent<PassengerMovementController>().TryToSendTop(x, z);
    }
}
