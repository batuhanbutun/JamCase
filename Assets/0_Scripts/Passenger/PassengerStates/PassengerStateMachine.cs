using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerStateMachine : MonoBehaviour
{
    private IPassengerState currentState;

    public void Initialize(IPassengerState startState)
    {
        currentState = startState;
        currentState.Enter();
    }

    public void ChangeState(IPassengerState newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }

    private void Update()
    {
        currentState?.Tick();
    }
}
