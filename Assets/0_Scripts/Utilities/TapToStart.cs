using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TapToStart : MonoBehaviour
{
    private bool _started = false;

    private void Update()
    {
        if (!_started && (Input.GetMouseButtonDown(0) || Input.touchCount > 0))
        {
            _started = true;
            GameManager.Instance.StartGame();
            enabled = false;
            gameObject.SetActive(false);
        }
    }
}
