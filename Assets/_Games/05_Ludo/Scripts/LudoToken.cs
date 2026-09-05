using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LudoToken : MonoBehaviour
{
    public LudoTokenController pawnController;
    public int firstWayID;
    public bool inBase;
    public bool inColorWay;
    public bool isProtected;
    public bool isCollected;
    public int currentWayID;
    public int moveCount;
    [SerializeField] GameObject reachHomeParticles;
    Vector2 startScale;
    Vector2 startPosition;

    void Start()
    {
        inBase = true;
        startScale = transform.localScale;
        startPosition = transform.position;
    }

    public void SetScaleToDefault()
    {
        transform.localScale = startScale;
    }

    public void Move(int count)
    {
        if (isCollected) return;
        if (!LudoGameController.Instance.isLocal) return;// && !photonView.IsMine

        if (inBase)
        {
            inBase = false;
            isProtected = true;
            currentWayID = firstWayID; 
            LeanTween.move(gameObject, LudoGameController.Instance.waypointParent.GetChild(firstWayID).position, 0.3f).setOnComplete(() => {
                
                LudoGameController.Instance.CheckGameStatus();
            });
            return;
        }

        StartCoroutine(MoveCoroutine(moveCount + count));
    }

    IEnumerator MoveCoroutine(int totalCount)
    {
        if (LudoGameController.Instance.isLocal)//photonView.IsMine || 
        {
            bool canMove = false;
            while (moveCount != totalCount)
            {
                if (!canMove)
                {
                    canMove = true;
                    currentWayID = (currentWayID + 1) % LudoGameController.Instance.waypointParent.childCount;
                    if (moveCount < 50)
                    {
                        LeanTween.move(gameObject, LudoGameController.Instance.waypointParent.GetChild(currentWayID).position, 0.1f).setDelay(0.05f).setOnComplete(() => {
                            LudoAudioManager.Instance.onTokenMove();
                            moveCount++;
                            canMove = false;
                        });
                    }
                    else
                    {
                        inColorWay = true;

                        if (moveCount == 50)
                            LudoAudioManager.Instance.onEnterColorWay();

                        string[] parseName = gameObject.name.Split("-");
                        Transform colorWay = LudoGameController.Instance.colorWayParent.Find(parseName[0]);
                        LeanTween.move(gameObject, colorWay.GetChild(moveCount - 50).position, 0.1f).setDelay(0.05f).setOnComplete(() => {
                            LudoAudioManager.Instance.onTokenMove();
                            moveCount++;
                            canMove = false;
                            if (moveCount == 56)
                            {
                                isCollected = true;
                                LudoAudioManager.Instance.onTokenReachLimit();
                                reachHomeParticles.SetActive(true);
                                GetComponent<CapsuleCollider2D>().enabled = false;
                                Invoke("hideReachHomeParticles", 2f);
                            }
                        });
                    }
                }

                yield return null;
            }

            if (currentWayID == 2 || currentWayID == 10 || currentWayID == 15 || currentWayID == 23 || currentWayID == 28 || currentWayID == 36 || currentWayID == 41 || currentWayID == 49)
            {
                isProtected = true;
                LudoAudioManager.Instance.onEnterSafeZone();
            }
            else
            {
                isProtected = false;
            }

            LudoGameController.Instance.CheckGameStatus();
        }
    }

    void hideReachHomeParticles()
    {
        reachHomeParticles.SetActive(false);
    }

    public void ReturnToBase()
    {
        StartCoroutine(ReturnToBaseCoroutine());
    }

    IEnumerator ReturnToBaseCoroutine()
    {
        bool canMove = false;
        LudoAudioManager.Instance.onTokenKilled();
        while (!inBase)
        {
            if (!canMove)
            {
                canMove = true;

                if (currentWayID > firstWayID)
                {
                    currentWayID = (currentWayID - 1) % LudoGameController.Instance.waypointParent.childCount;
                    LeanTween.move(gameObject, LudoGameController.Instance.waypointParent.GetChild(currentWayID).position, 0.05f).setDelay(0.025f).setOnComplete(() => {
                        canMove = false;
                    });
                }
                else if (currentWayID < firstWayID)
                {
                    currentWayID = (currentWayID + 1) % LudoGameController.Instance.waypointParent.childCount;
                    LeanTween.move(gameObject, LudoGameController.Instance.waypointParent.GetChild(currentWayID).position, 0.05f).setDelay(0.025f).setOnComplete(() => {
                        canMove = false;
                    });
                }
                else
                {
                    LeanTween.move(gameObject, startPosition, 0.05f).setDelay(0.025f).setOnComplete(() => {
                        canMove = false;
                        inBase = true;
                        moveCount = 0;
                        LudoGameController.Instance.CheckForFinish();
                    });
                }
            }

            yield return null;
        }

    }

    /*public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(inBase);
            stream.SendNext(inColorWay);
            stream.SendNext(isProtected);
            stream.SendNext(isCollected);
            stream.SendNext(currentWayID);
            stream.SendNext(moveCount);
        }
        else
        {
            inBase = (bool)stream.ReceiveNext();
            inColorWay = (bool)stream.ReceiveNext();
            isProtected = (bool)stream.ReceiveNext();
            isCollected = (bool)stream.ReceiveNext();
            currentWayID = (int)stream.ReceiveNext();
            moveCount = (int)stream.ReceiveNext();
        }
    }*/
}