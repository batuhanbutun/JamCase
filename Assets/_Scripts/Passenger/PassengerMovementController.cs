using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PassengerMovementController : MonoBehaviour,IMovable
{
    private Passenger passenger;
    [SerializeField] private float movementSpeed = 2f;
    
    private void Awake()
    {
        passenger = GetComponent<Passenger>();
    }

    
    public bool TryToSendTop(int x, int z)
    {
        Vector2Int start = new Vector2Int(x, z);
        List<Vector2Int> path = GridMovementPathfinder.GetPathToTop(start);
        if (path == null) return false;
        
        var busAvailable = BusManager.Instance.IsCurrentBusAvailable(passenger);
        var waitingAreaAvailable = WaitingAreaManager.Instance.IsAvailable();
        if(!waitingAreaAvailable && !busAvailable) return false;
        
        GridMovementPathfinder.EvaluatePassengerPaths();
        GridManager3D.Instance.gridPassengers[x, z] = null;
        GetComponent<IOutlinable>().OutlineSet(false);
            
        Vector3[] worldPath = GridMovementPathfinder.GetPathWorldPoints(path);
            
        StartCoroutine(Move(worldPath, Route));
        return true;
    }

    private void Route()
    {
        PassengerFlowController.Instance.RoutePassenger(passenger);
    }

    public IEnumerator Move(Vector3 targetPos,Action onComplete = null)
    {
        passenger.SetAnimator("running",true);
        LookAtSmooth(targetPos);

        yield return transform.DOMove(targetPos, movementSpeed)
            .SetEase(Ease.Linear)
            .SetSpeedBased(true)
            .WaitForCompletion();

        passenger.SetAnimator("running",false);
        if (GameManager.Instance.gameState == GameState.PLAY)
            onComplete?.Invoke();
    }
    
    public IEnumerator Move(Vector3[] worldPath,Action onComplete = null)
    {
        if (worldPath == null || worldPath.Length < 2)
        {
            onComplete?.Invoke();
            yield break;
        }
        passenger.SetAnimator("running",true);

        yield return transform.DOPath(worldPath, movementSpeed, PathType.Linear)
            .SetEase(Ease.Linear)
            .SetLookAt(0.01f) // Dönüş için, ama gerekirse yorum satırına al
            .SetSpeedBased(true)
            .WaitForCompletion();

        passenger.SetAnimator("running",false);
        if (GameManager.Instance.gameState == GameState.PLAY)
            onComplete?.Invoke(); 
    }
    
    private void LookAtSmooth(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(direction);
            transform.DORotate(lookRot.eulerAngles, 0.2f).SetEase(Ease.InOutSine);
        }
    }
    
    
    
}
