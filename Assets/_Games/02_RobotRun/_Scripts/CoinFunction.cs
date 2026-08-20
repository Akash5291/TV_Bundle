using UnityEngine;
using System.Collections;

public class CoinFunction : MonoBehaviour
{
    Camera mainCamera;
    RectTransform coinImage;

    [Header ("Values")]
    [Range (0, 20)]
    [SerializeField] float smoothness;
    [SerializeField] Vector3 offset;

    Vector3 coinPosition;
    WaitForSeconds twoSeconds = new WaitForSeconds(2);


    [SerializeField] bool isCollected = false;

    private void Update()
    {
        if (isCollected)
        {
            coinPosition = mainCamera.ScreenToWorldPoint(coinImage.position);
            transform.position = Vector3.MoveTowards(transform.position, coinPosition + offset, smoothness * Time.deltaTime);
        }
    }

    public void Collected(Camera _mainCamera, RectTransform _coinImage)
    {
        StartCoroutine(nameof(CollectedFunction));
        mainCamera = _mainCamera;
        coinImage = _coinImage;
    }

        IEnumerator CollectedFunction()
    {
        isCollected = true;

        yield return twoSeconds;
        Destroy(gameObject);
    }
}
