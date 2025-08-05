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
    
    [Header("SeatSettings")]
    [SerializeField] private List<Chair> chairs;
    public void ColorSetup(ObjColor color)
    {
        busColor = color;
        visualRenderer.material.color = ColorUtils.FromObjColor(color);
    }
    
    public void SetSeatForPassenger(Passenger passenger)
    {
        reservedSeats++;
        StartCoroutine(passenger.GetComponent<IMovable>().Move(transform.position, () =>
        {
            passengers.Add(passenger);
            PassengerOnBus?.Invoke();
            chairs[passengers.Count-1].OpenSeatedPassenger(busColor);
            Destroy(passenger.gameObject);
        }));
        //Otobüs koltuğuna oturt
    }

    public IEnumerator Move(Vector3 targetPos, Action onComplete = null)
    {
        yield return transform.DOMove(targetPos, 1f)
            .SetEase(Ease.InOutSine)
            .OnComplete(() => onComplete?.Invoke());
    }
    
}
