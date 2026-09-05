using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

    [System.Serializable]
    public struct BotMovements {
        public LudoToken pawn;
        public int moveCount;

        public BotMovements(LudoToken target, int count) {
            pawn = target;
            moveCount = count;
        }
    }

public class LudoTokenController : MonoBehaviour
{
    public LudoToken[] token;
    public GameObject tokenParent;
    public GameObject profileParent;
    public Image profileDiceImg;
    public Image profileTimeImg;
    public Sprite[] diceSprites;
    public string tokenColor;
    public bool isBot;
    public bool canPlayAgain;
    public List<BotMovements> botMovements;
    public float time;
    public TMP_Text usernameText;
    public Image avatarImg;


    void Start()
    {
        botMovements = new List<BotMovements>();
    }
    
    void Update()
    {
        if (!LudoGameController.Instance.isLocal) return;//&& !LudoGameController.Instance.photonView.IsMine

        if (LudoGameController.Instance.currentPawnController == this)
        {
            if (LudoGameController.Instance.gameState != LudoGameController.GameState.FINISHED && LudoGameController.Instance.gameState != LudoGameController.GameState.MOVING && LudoGameController.Instance.gameState != LudoGameController.GameState.WAIT)
            {
                if (time > 0)
                {
                    time -= 1 * Time.deltaTime;
                    profileTimeImg.fillAmount = time / 10;
                }
                else
                {
                    canPlayAgain = false;
                    profileTimeImg.fillAmount = 0;
                    HighlightDices(false);
                    LudoGameController.Instance.CheckGameStatus();
                }
            }
            else
            {
                profileTimeImg.fillAmount = 0;
            }
        }
    }

    /*public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(time);
        }
        else
        {
            profileTimeImg.fillAmount = (float)stream.ReceiveNext() / 10;
        }
    }*/

    public void SetUserInfo(string name, int avatarID)
    {
        usernameText.text = name;
        avatarImg.sprite = LudoGameController.Instance.avatars[avatarID];
    }

    public void SetUserInfo()
    {
        if (LudoGameController.Instance.myPawnController != this)
        {
            if (PlayerPrefs.GetString("isOnline").Equals("true"))
                usernameText.text = "Guest" + Random.Range(0, 99999); 
            else
                usernameText.text = "AI";

            avatarImg.sprite = LudoGameController.Instance.avatars[Random.Range(0, LudoGameController.Instance.avatars.Length)];
        }
        else
        {
            usernameText.text = "You";
            avatarImg.sprite = LudoGameController.Instance.avatars[PlayerPrefs.GetInt("avatar")];
        }
    }

    public void DisableColliders()
    {
        for (int i = 0; i < token.Length; i++)
        {
            token[i].GetComponent<CapsuleCollider2D>().enabled = false;
        }
    }

    public void HighlightDices(bool active)
    {
        for (int i = 0; i < token.Length; i++)
        {
            if (active)
            {
                LeanTween.scale(token[i].gameObject, token[i].transform.localScale * 1.1f, 0.2f).setLoopPingPong();
            }
            else
            {
                LeanTween.cancel(token[i].gameObject);
                token[i].SetScaleToDefault();
            }
        }
    }

    public void CheckAvailableMovements(bool playAgain)
    {
        botMovements.Clear();
        canPlayAgain = playAgain;

        LudoToken[] basePawns = token.Where(x => x.inBase).ToArray();
        int availablePawnCount = 0;

        if (basePawns.Length == 4 && LudoGameController.Instance.currentDice != 5)
        {
            LudoGameController.Instance.ChangePlayer();
            return;
        }

        for (int i = 0; i < token.Length; i++)
        {
            LudoToken pawn = token[i];

            if (!pawn.isCollected)
            {
                if (pawn.inBase)
                {
                    if (LudoGameController.Instance.currentDice == 5)
                    {
                        availablePawnCount++;
                        botMovements.Add(new BotMovements(pawn, LudoGameController.Instance.currentDice + 1));
                        LeanTween.scale(pawn.gameObject, pawn.transform.localScale * 1.1f, 0.2f).setLoopPingPong();
                    }
                }
                else
                {
                    if (pawn.moveCount < 56 && pawn.moveCount + LudoGameController.Instance.currentDice + 1 <= 56)
                    {
                        availablePawnCount++;
                        botMovements.Add(new BotMovements(pawn, LudoGameController.Instance.currentDice + 1));
                        LeanTween.scale(pawn.gameObject, pawn.transform.localScale * 1.1f, 0.2f).setLoopPingPong();
                    }
                }
            }
        }

        if (availablePawnCount > 0)
        {
            LudoGameController.Instance.ChangeGameState(LudoGameController.GameState.MOVE);
            if (isBot)
            {
                LudoGameController.Instance.ChangeGameState(LudoGameController.GameState.MOVING);
                HighlightDices(false);
                BotMovements rand = botMovements[Random.Range(0, botMovements.Count)];
                //rand.pawn.Move(rand.moveCount);
                StartCoroutine(delayAITokenMovement(rand));
                botMovements.Clear();
            }
        }
        else
        {
            LudoGameController.Instance.ChangePlayer();
        }
    }

    IEnumerator delayAITokenMovement(BotMovements rand)
    {
        yield return new WaitForSeconds(Random.Range(0.5f, 1f));
        rand.pawn.Move(rand.moveCount);
    }

    public void DisablePawn()
    {
        profileParent.SetActive(false);
        tokenParent.SetActive(false);
        gameObject.SetActive(false);
    }

    public void StartTimer(bool animation = false)
    {
        if (animation)
        {
            profileParent.LeanScale(Vector3.one * 1.05f, 0.6f).setLoopPingPong();
        }
        time = 10;
    }

    public void StopAnimation()
    {
        if (profileParent.LeanIsTweening())
        {
            profileParent.LeanCancel();
        }
        profileParent.transform.localScale = Vector3.one;
    }

    public void Play()
    {
        //LudoGameController.Instance.GameDiceBtn(tokenColor);
        Invoke("delayDiceRollForAI", Random.Range(0.5f, 3f));
    }

    void delayDiceRollForAI()
    {
        CancelInvoke("delayDiceRollForAI");
        LudoGameController.Instance.GameDiceBtn(tokenColor);
    }

    public void PlayDiceAnimation()
    {
        StartCoroutine(DiceAnimation());
    }

    IEnumerator DiceAnimation()
    {
        int rand = Random.Range(0, diceSprites.Length);
        int oldRand = 0;
        float t = 0;

        while (t < 0.45f)
        {
            t += Time.deltaTime;

            if (rand == oldRand)
            {
                rand = Random.Range(0, diceSprites.Length);
            }

            profileDiceImg.sprite = diceSprites[rand];

            oldRand = rand;
            yield return null;
        }

        profileDiceImg.sprite = diceSprites[LudoGameController.Instance.currentDice];

    }
}