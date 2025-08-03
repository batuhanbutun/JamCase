using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PassengerMovementController : MonoBehaviour
{
    private Passenger passenger;
    
    private void Awake()
    {
        passenger = GetComponent<Passenger>();
    }
    
    public IEnumerator MoveTo(Vector3 targetPos, float duration = 0.3f, Action onComplete = null)
    {
        passenger.SetAnimator("running");
        LookAtSmooth(targetPos);

        yield return transform.DOMove(targetPos, duration)
            .SetEase(Ease.InOutSine)
            .WaitForCompletion();

        passenger.SetAnimator("idle");
        onComplete?.Invoke();
    }
    
    public IEnumerator FollowPath(Vector3[] worldPath, float duration = 1f, Action onComplete = null)
    {
        passenger.SetAnimator("running");

        yield return transform.DOPath(worldPath, duration, PathType.Linear)
            .SetEase(Ease.InOutSine)
            .SetLookAt(0.01f) // Dönüş için, ama gerekirse yorum satırına al
            .WaitForCompletion();

        passenger.SetAnimator("idle");
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
