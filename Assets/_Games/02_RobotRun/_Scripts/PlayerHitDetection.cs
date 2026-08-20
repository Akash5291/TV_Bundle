using System;
using UnityEngine;

public class PlayerHitDetection : MonoBehaviour
{
    public static Action CoinCollected;
    public static Action HitObstacles;

    [SerializeField] Camera mainCamera;
    [SerializeField] RectTransform coinImage;

    [Header("Tags")]
    [SerializeField] string coinTag;
    [SerializeField] string obstacles;

    public bool hacks = false;
    bool checkHit = true;

    private void OnTriggerEnter2D(Collider2D info)
    {
        if (!checkHit)
            return;

        if (info.CompareTag (coinTag))
        {
            CoinCollected?.Invoke();
            info.GetComponent<CoinFunction>().Collected(mainCamera, coinImage);
        }

        else if (info.CompareTag (obstacles))
        {
            if (hacks)
                return;

            HitObstacles?.Invoke();
            checkHit = false;
        }
    }
}
