using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PassengerGridEventHandlerBase : MonoBehaviour
{
    protected Passenger passenger;

    protected virtual void Awake()
    {
        passenger = GetComponent<Passenger>();
    }

    protected virtual void OnEnable()
    {
        GridEvents.OnPassengerSent += OnPassengerSent;
        GridEvents.OnPassengerExitGrid += OnPassengerExitGrid;
    }

    protected virtual void OnDisable()
    {
        GridEvents.OnPassengerSent    -= OnPassengerSent;
        GridEvents.OnPassengerExitGrid -= OnPassengerExitGrid;
    }

    private void OnPassengerSent(Passenger sent, Vector2Int fromGridPos)
    {
        // Kendi grid’imden gönderildiğinde
        if (sent == passenger)
        {
            HandleOwnGridSent(fromGridPos);
            return;
        }

        // Başka grid’den gönderildi, önce neighbor mı kontrol et
        var myPos = passenger.gridPos;
        bool isNeighbor = 
            (Mathf.Abs(fromGridPos.x - myPos.x) == 1 && fromGridPos.y == myPos.y) ||
            (Mathf.Abs(fromGridPos.y - myPos.y) == 1 && fromGridPos.x == myPos.x);

        if (isNeighbor)
            HandleNeighborGridSent(sent, fromGridPos);
        else
            HandleAnyGridSent(sent, fromGridPos);
    }

    private void OnPassengerExitGrid(Passenger exited)
    {
        if (exited == passenger)
            HandleOwnExit();
    }

    /// <summary>Herhangi bir grid’den gönderilen passenger olduğunda.</summary>
    protected virtual void HandleAnyGridSent(Passenger sent, Vector2Int fromGridPos) { }

    /// <summary>Komşu grid’den gönderilen passenger olduğunda.</summary>
    protected virtual void HandleNeighborGridSent(Passenger sent, Vector2Int fromGridPos) { }

    /// <summary>Kendi grid’imden gönderildiğimde.</summary>
    protected virtual void HandleOwnGridSent(Vector2Int fromGridPos) { }

    /// <summary>Kendi grid’imden çıkış tamamlandığında (ister hareket ister başka exit).</summary>
    protected virtual void HandleOwnExit() { }
}
