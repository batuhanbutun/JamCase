using System.Collections.Generic;
using _Scripts;
using UnityEngine;

[CreateAssetMenu(menuName = "Level/LevelData")]
public class LevelData : ScriptableObject
{
    public float levelDuration;
    public int gridWidth;
    public int gridHeight;

    public List<PassengerData> passengerList = new();
    public List<ObjColor> busColorSequence = new();
    public List<Vector2Int> lockedGridPositions = new();
}

[System.Serializable]
public class PassengerData
{
    public Vector2Int gridPosition;
    public ObjColor color;
    public PassengerType passengerType;
}
