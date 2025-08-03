using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPassengerReceiver
{
    public bool TryReceive(Passenger passenger);
}
