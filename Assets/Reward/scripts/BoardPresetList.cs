using UnityEngine;

namespace Combat.BoardPreset
{
    [CreateAssetMenu(fileName = "BoardPresetList", menuName = "Scriptable Objects/BoardPreset List")]
    public class BoardPresetList : ScriptableObject
    {
        public BoardPresetObject[] presets;
    }
}