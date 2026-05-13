using UnityEngine;

namespace Combat.BoardPreset
{
    [CreateAssetMenu(fileName = "BoardPresetObject", menuName = "Scriptable Objects/BoardPreset")]
    public class BoardPresetObject : ScriptableObject
    {
        [System.Serializable]
        public struct Line
        {
            public Creature[] creatures;
        }

        public Line[] preset;
    }
}