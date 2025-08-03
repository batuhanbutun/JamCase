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
    
    public IEnumerator MoveTo(Vector3 targetPos, Action onComplete = null)
    {
        
        passenger.SetAnimator("running",true);
        LookAtSmooth(targetPos);

        yield return transform.DOMove(targetPos, movementSpeed)
            .SetEase(Ease.InOutSine)
            .SetSpeedBased(true)
            .WaitForCompletion();

        passenger.SetAnimator("running",false);
        onComplete?.Invoke();
    }
    
    public IEnumerator FollowPath(Vector3[] worldPath, Action onComplete = null)
    {
        passenger.SetAnimator("running",true);

        yield return transform.DOPath(worldPath, movementSpeed, PathType.Linear)
            .SetEase(Ease.InOutSine)
            .SetLookAt(0.01f) // Dönüş için, ama gerekirse yorum satırına al
            .SetSpeedBased(true)
            .WaitForCompletion();

        passenger.SetAnimator("running",false);
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
