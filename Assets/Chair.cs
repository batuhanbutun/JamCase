using System.Collections;
using System.Collections.Generic;
using _Scripts;
using DG.Tweening;
using UnityEngine;

public class Chair : MonoBehaviour
{
    [SerializeField] private GameObject seatedPassengerDummy;
    [SerializeField] private Renderer seatedPassengerRenderer;

    public void OpenSeatedPassenger(ObjColor busColor)
    {
        seatedPassengerRenderer.material.color = ColorUtils.FromObjColor(busColor);
        seatedPassengerDummy.SetActive(true);
        seatedPassengerDummy.transform.DOScale(0.64f,0.35f).SetEase(Ease.OutBounce);
    }
    
}
