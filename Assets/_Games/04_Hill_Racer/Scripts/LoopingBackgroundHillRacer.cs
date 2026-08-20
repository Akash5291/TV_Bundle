using System;
using System.Collections;
using UnityEngine;

public class LoopingBackgroundHillRacer : MonoBehaviour
{
    public float speed = 0.3f;

    float offset;
    [SerializeField] Material mat;

    private void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    void Update()
    {
        offset += (Time.deltaTime * speed) / 10f;
        mat.SetTextureOffset("_MainTex", new Vector2(-offset, 0f));
    }
}
