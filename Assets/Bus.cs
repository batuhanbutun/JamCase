using System.Collections;
using System.Collections.Generic;
using _Scripts;
using DG.Tweening;
using UnityEngine;

public class Bus : MonoBehaviour,IColorable
{
    [SerializeField] private ObjColor busColor;
    public int busCapacity = 3;
    private List<Passenger> passengers = new();
    
    public bool IsBusFull => passengers.Count >= busCapacity;
    public ObjColor ObjColor => busColor;

    public void ColorSetup(ObjColor color)
    {
        busColor = color;
        GetComponent<Renderer>().material.color = ColorUtils.FromObjColor(color);
    }
    
    public void SetSeatForPassenger(Passenger passenger)
    {
        passengers.Add(passenger);
        passenger.transform.DOMove(transform.position,0.2f).SetEase(Ease.InOutSine);
        //Otobüs koltuğuna oturt
    }
    
    public void MoveTo(Vector3 targetPos)
    {
        transform.DOMove(targetPos, 1f).SetEase(Ease.InOutSine);
    }

    public void PlayDeparture(System.Action onComplete)
    {
        transform.DOMove(transform.position + Vector3.right * 10f, 1f)
            .SetEase(Ease.InOutSine)
            .OnComplete(() => onComplete?.Invoke());
    }

}
