using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BombPassenger : PassengerGridEventHandlerBase
{
    [SerializeField] private int maxLives = 8;
    [SerializeField] private TextMeshProUGUI liveText;
    [SerializeField] private GameObject bombObject;
    private int lives;
    private bool activated;

    protected override void Awake()
    {
        base.Awake();
        lives = maxLives;
        liveText.text = lives.ToString();
        activated = false;
    }
    
    protected override void HandleNeighborGridSent(Passenger sent, Vector2Int fromGridPos)
    {
        if (!activated)
        {
            // İlk komşu gönderimde aktive et, hak düşürme
            activated = true;
            liveText.gameObject.SetActive(true);
        }
        else
        {
            // Sonrasında her gönderimde hak düş
            lives--;
            UpdateBombUI(lives);
            if (lives <= 0)
                GameManager.Instance.GameFail();
        }
    }

    protected override void HandleAnyGridSent(Passenger sent, Vector2Int fromGridPos)
    {
        // Aktif değilken uzak tıklamaları yok say
        if (!activated) return;

        // Aktifken uzak tıklama da hak düşürür
        lives--;
        UpdateBombUI(lives);
        if (lives <= 0)
            GameManager.Instance.GameFail();
    }

    protected override void HandleOwnGridSent(Vector2Int fromGridPos)
    {
        // Kendi gönderimimde disarm et
        Disarm();
    }

    private void Disarm()
    {
        activated = false;
        bombObject.gameObject.SetActive(false);
        // istersen event aboneliğinden de çık:
        // GridEvents.OnPassengerSent -= OnPassengerSent;
    }

    private void UpdateBombUI(int remaining)
    {
        liveText.text = lives.ToString();
    }

    
}
