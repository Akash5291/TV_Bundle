using UnityEngine;
using UnityEngine.UI;

public class SelectAnimation : MonoBehaviour
{
    [SerializeField]float max = 1.09f;
    [SerializeField]float min = 1.0f;
    [SerializeField]float speed = 0.1f;
    bool isAnimPlus = true;
    public bool isSelect = false;

    Button button;

    private void Start()
    {
        button = transform.GetComponent<Button>();
    }

    public void onSelect(bool value)
    {
        //Debug.Log("onSelect btn: " + value);
        isSelect = value;
    }

    private void Update()
    {
        if (isSelect)
        {
            if (transform.localScale.x > max)
            {
                isAnimPlus = false;
            }
            else if (transform.localScale.x < min)
            {
                isAnimPlus = true;
            }

            if (isAnimPlus)
            {
                float cnt = 0f;

                if (Time.deltaTime == 0f)
                    cnt = Time.fixedDeltaTime * speed;//Random.Range(0.003f, 0.009f) * speed;
                else
                    cnt = Time.deltaTime * speed;
                transform.localScale = new Vector3(transform.localScale.x + cnt, transform.localScale.y + cnt, transform.localScale.z + cnt);
            }
            else
            {
                float cnt = 0f;

                if (Time.deltaTime == 0f)
                    cnt = Time.fixedDeltaTime * speed;//Random.Range(0.003f, 0.009f) * speed;
                else
                    cnt = Time.deltaTime * speed;
                transform.localScale = new Vector3(transform.localScale.x - cnt, transform.localScale.y - cnt, transform.localScale.z - cnt);
            }
        }
        else
            transform.localScale = new Vector3(min, min, min);
    }
}
