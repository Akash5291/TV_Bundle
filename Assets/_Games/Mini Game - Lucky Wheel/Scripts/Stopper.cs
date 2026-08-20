using UnityEngine;

namespace FortuneWheel
{
    public class Stopper : MonoBehaviour
    {
        private Animator anim;

        private void Start()
        {
            anim = transform.GetComponent<Animator>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.tag == "PinPoint")
            {
                anim.SetBool("isPlay", true);
                WheelController.ins.rewardImageHeader.sprite = WheelController.ins.PiecesOfWheel[collision.transform.parent.GetComponent<PieceObject>().index].rewardIcon;
                WheelController.ins.rewardTextHeader.text = WheelController.ins.PiecesOfWheel[collision.transform.parent.GetComponent<PieceObject>().index].rewardAmount.ToString();
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.transform.tag == "PinPoint")
            {
                anim.SetBool("isPlay", false);
            }
        }
    }
}
