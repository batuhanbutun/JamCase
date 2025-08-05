using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockClick3D : MonoBehaviour
{
    private int x, z;
    private GridManager3D grid;

    public void Init(int x, int z, GridManager3D grid)
    {
        this.x = x;
        this.z = z;
        this.grid = grid;
    }

    private void OnMouseDown()
    {
        Debug.Log($"Clicked {x}, {z}");
        grid.TrySendToTop(x, z);
    }
}
