using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPassengerState
{
    public void Enter();   
    public void Exit();     
    public void Tick();     
}
