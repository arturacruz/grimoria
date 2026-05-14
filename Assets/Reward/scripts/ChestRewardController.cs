using UnityEngine;

namespace Reward.scripts
{
    public class ChestRewardController : MonoBehaviour
    {
        [Header("Reward Window")] [SerializeField]
        private GameObject rewardWindow;

        [Header("Reward Slots")] [SerializeField]
        private SpriteRenderer[] slotRenderers;

        [Header("Reward Sprites")] [SerializeField]
        private Sprite[] rewardSprites;

        public void OnChestOpened()
        {
            rewardWindow.SetActive(true);
            FillRewards();
        }

        private void FillRewards()
        {
            for (int i = 0; i < slotRenderers.Length; i++)
            {
                if (i < rewardSprites.Length && rewardSprites[i] != null)
                {
                    slotRenderers[i].sprite = rewardSprites[i];
                    slotRenderers[i].enabled = true;
                }
                else
                {
                    slotRenderers[i].enabled = false;
                }
            }
        }
    }
}