using System.Collections;
using System.Collections.Generic;
using _Scripts;
using DG.Tweening;
using UnityEngine;

public class Bus : MonoBehaviour,IColorable
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
        passenger.transform.DOMove(transform.position,1f).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            passengers.Add(passenger);
            PassengerOnBus?.Invoke();
            Destroy(passenger.gameObject);
        });
        //Otobüs koltuğuna oturt
    }

    public void PlayDeparture(System.Action onComplete)
    {
        transform.DOMove(transform.position + Vector3.right * 10f, 1f)
            .SetEase(Ease.InOutSine)
            .OnComplete(() => onComplete?.Invoke());
    }

}
