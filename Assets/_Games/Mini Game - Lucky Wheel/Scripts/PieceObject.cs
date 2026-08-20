using UnityEngine;
using UnityEngine.UI;

namespace FortuneWheel
{
    public class PieceObject : MonoBehaviour
    {
        public Image backgroundImage;
        public Image rewardIcon;
        public Text rewardAmount;
        public WheelController.RewardEnum rewardCategory;

        public int index;

        public void SetValues(int pieceNo)
        {
            index = pieceNo;

            if (WheelController.ins.useCustomBackgrounds)
            {
                backgroundImage.color = Color.white;
                backgroundImage.sprite = WheelController.ins.CustomBackgrounds[pieceNo];
            }
            else
            {
                backgroundImage.color = WheelController.ins.PiecesOfWheel[pieceNo].backgroundColor;
                backgroundImage.sprite = WheelController.ins.PiecesOfWheel[pieceNo].backgroundSprite;
            }

            rewardCategory = WheelController.ins.PiecesOfWheel[pieceNo].rewardCategory;
            rewardAmount.text = WheelController.ins.PiecesOfWheel[pieceNo].rewardAmount.ToString();

            for (int i = 0; i < WheelController.ins.categoryIcons.Length; i++)
            {
                if (rewardCategory == WheelController.ins.categoryIcons[i].category)
                {
                    rewardIcon.sprite = WheelController.ins.categoryIcons[i].rewardIcon;
                    WheelController.ins.PiecesOfWheel[pieceNo].rewardIcon = WheelController.ins.categoryIcons[i].rewardIcon;
                }
            }
        }
    }
}