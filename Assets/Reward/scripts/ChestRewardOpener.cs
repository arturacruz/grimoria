using UnityEngine;

namespace Reward.scripts
{
    public class ChestRewardOpener : MonoBehaviour
    {
        [SerializeField] private GameObject rewardWindow;

        public void OpenRewardWindow()
        {
            if (rewardWindow != null)
                rewardWindow.SetActive(true);
        }
    }
}