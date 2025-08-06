using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PassengerClickType
{
    GridClick,
    NeighborClick,
    SelfClick
}

public struct PassengerClickContext
{
    public Vector2Int ClickedGridPos;     
    public PassengerClickType ClickType;
}
