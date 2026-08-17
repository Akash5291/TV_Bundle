using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TVButtonRow
{
    public List<TVButtonColumn> tvRow = new List<TVButtonColumn>();
}

[Serializable]
public class TVButtonColumn
{
    public List<GameObject> tvCol = new List<GameObject>();
}

public class TVInputManager : MonoBehaviour
{

    [SerializeField] GameObject previewPlayer;
    [SerializeField] TVButtonRow tvButtons;

    bool tvInput = true;

    int row = 1;
    int col = 0;

    private void Update()
    {
        if (!LanguageSelector.Instance.dataReceived) return;

        if (previewPlayer.activeSelf)
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
                previewPlayer.transform.GetComponent<VideoPreviewManager>().onCloseBtn();
        }
        else
        {
            float v = UnityEngine.Input.GetAxis("Vertical");
            float h = UnityEngine.Input.GetAxis("Horizontal");

            if (v != 0 && tvInput)
            {
                if (v > 0 && row > 0)//up arrow
                {
                    tvInput = false; 
                    tvButtons.tvRow[row].tvCol[col].transform.GetChild(0).gameObject.SetActive(false);//unselect current selected lan
                    row -= 1;
                    col = 0; 
                    tvButtons.tvRow[row].tvCol[col].transform.GetChild(0).gameObject.SetActive(true);//select newly selected lan
                }
                else if (v < 0 && (row + 1) < tvButtons.tvRow.Count)//down arrow
                {
                    tvInput = false;
                    tvButtons.tvRow[row].tvCol[col].transform.GetChild(0).gameObject.SetActive(false);//unselect current selected lan
                    row += 1;
                    col = 0;
                    tvButtons.tvRow[row].tvCol[col].transform.GetChild(0).gameObject.SetActive(true);//select newly selected lan
                    LanguageSelector.Instance.updateIntruction(col);
                }
            }
            else if (h != 0 && tvInput)
            {
                if (h > 0)//forward
                {
                    if ((col + 1) < tvButtons.tvRow[row].tvCol.Count)
                    {
                        tvInput = false;
                        tvButtons.tvRow[row].tvCol[col].transform.GetChild(0).gameObject.SetActive(false);//unselect current selected lan
                        col += 1;
                        tvButtons.tvRow[row].tvCol[col].transform.GetChild(0).gameObject.SetActive(true);//select newly selected lan
                        LanguageSelector.Instance.updateIntruction(col);
                    }
                }
                else if (h < 0)//backward
                {
                    if ((col - 1) >= 0)
                    {
                        tvInput = false;
                        tvButtons.tvRow[row].tvCol[col].transform.GetChild(0).gameObject.SetActive(false);//unselect current selected lan
                        col -= 1;
                        tvButtons.tvRow[row].tvCol[col].transform.GetChild(0).gameObject.SetActive(true);//select newly selected lan
                        LanguageSelector.Instance.updateIntruction(col);
                    }
                }
            }
            else if (h == 0)
                tvInput = true;
                        
            // To check preview button click or not
            if(row==0 && col == 0 && 
                (UnityEngine.Input.GetKeyDown(KeyCode.Menu) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Joystick1Button0)))
            {
                previewPlayer.SetActive(true);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                //Application.Quit();Akash
            }
        }
    }
}
