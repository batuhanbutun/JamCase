using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts;
using DG.Tweening;
using UnityEngine;

public class Bus : MonoBehaviour,IColorable,IMovable
{
    [SerializeField] private ObjColor busColor;
    [SerializeField] private Renderer visualRenderer;
    private int busCapacity = 3;
    private int reservedSeats = 0;
    private List<Passenger> passengers = new();
    
    public bool IsBusFull => passengers.Count >= busCapacity;
    public bool IsBusReserved => reservedSeats >= busCapacity;
    public ObjColor ObjColor => busColor;

    public static System.Action PassengerOnBus;
    public void ColorSetup(ObjColor color)
    {
        busColor = color;
        visualRenderer.material.color = ColorUtils.FromObjColor(color);
    }
    
    public void SetSeatForPassenger(Passenger passenger)
    {
        reservedSeats++;
        StartCoroutine(passenger.GetComponent<IMovable>().MoveTo(transform.position, () =>
        {
            passengers.Add(passenger);
            PassengerOnBus?.Invoke();
            Destroy(passenger.gameObject);
        }));
        //Otobüs koltuğuna oturt
    }

    public IEnumerator MoveTo(Vector3 targetPos, Action onComplete = null)
    {
        yield return transform.DOMove(targetPos, 1f)
            .SetEase(Ease.InOutSine)
            .OnComplete(() => onComplete?.Invoke());
    }

    public IEnumerator FollowPath(Vector3[] worldPath, Action onComplete = null)
    {
        //Şu an ihtiyaç yok. Ama ilerde oyuna otobüs pathleri eklersek kullanılabilir. 20-30 level oynadım ama öyle bir şey yok sanırım :)
        yield break;
    }
}
