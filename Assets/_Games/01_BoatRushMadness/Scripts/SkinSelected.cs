using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinSelected : MonoBehaviour
{
    [SerializeField] private SkinManager skinManager;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Image>().sprite = skinManager.GetSelectedSkin().sprite;

        
    }

    
}
