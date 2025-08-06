using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovable
{
    IEnumerator Move(Vector3 targetPos, Action onComplete = null);
    
}
