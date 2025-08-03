using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovable
{
    IEnumerator MoveTo(Vector3 targetPos, Action onComplete = null);
    
    IEnumerator FollowPath(Vector3[] worldPath, Action onComplete = null);
}
