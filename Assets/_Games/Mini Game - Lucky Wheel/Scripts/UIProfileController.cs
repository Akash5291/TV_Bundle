using UnityEngine;

namespace FortuneWheel
{
    public class UIProfileController : MonoBehaviour
    {
        public enum RewardEnum                  // Your Custom Reward Category
        {
            None,
            Gold,
            Energy,
            Life,
            Gem1,
            Gem2,
            Gem3,
            Gem4,
            Money
        };
        [System.Serializable]
        public class MultiDimensional
        {
            public RewardEnum rewardType;
            public GameObject UIElement;
        }
        public MultiDimensional[] UIProfileElements;

        private void Start()
        {
            SetActiveElements();
        }

        private void SetActiveElements()
        {
            for (int i = 0; i < WheelController.ins.PiecesOfWheel.Length; i++)
            {
                WheelController.RewardEnum collectedRewardType = WheelController.ins.PiecesOfWheel[i].rewardCategory;

                switch (collectedRewardType)
                {
                    case WheelController.RewardEnum.Hundred:
                        {
                            FindThatElement(RewardEnum.Gold);
                        }
                        break;
                    case WheelController.RewardEnum.ThreeHundred:
                        {
                            FindThatElement(RewardEnum.Gold);
                        }
                        break;
                    case WheelController.RewardEnum.TwoHundred:
                        {
                            FindThatElement(RewardEnum.Gold);
                        }
                        break;
                    case WheelController.RewardEnum.Zero:
                        {
                            FindThatElement(RewardEnum.Gold);
                        }
                        break;
                    default:
                        UIProfileElements[i].UIElement.SetActive(false);
                        break;
                }
            }
        }


        private void FindThatElement(RewardEnum type)
        {
            for (int i = 0; i < UIProfileElements.Length; i++)
            {
                if (UIProfileElements[i].rewardType == type)
                    UIProfileElements[i].UIElement.SetActive(true);
            }
        }
    }
}