using System.Reflection;
using Combat.BoardPreset;
using UnityEngine;

public class RandomBoardPresetSelector : MonoBehaviour
{
    public BoardPresetList presetList;

    private void Awake()
    {
        if (presetList == null || presetList.presets == null || presetList.presets.Length == 0)
        {
            Debug.LogError("BoardPresetList vazia.");
            return;
        }

        BoardPresetObject chosen = presetList.presets[Random.Range(0, presetList.presets.Length)];
        Debug.Log("Preset escolhido: " + chosen.name);

        GridComponent grid = GetComponent<GridComponent>();
        if (grid == null)
        {
            Debug.LogError("GridComponent não encontrado.");
            return;
        }

        FieldInfo field = typeof(GridComponent).GetField(
            "boardPreset",
            BindingFlags.NonPublic | BindingFlags.Instance
        );

        if (field == null)
        {
            Debug.LogError("Campo 'boardPreset' não encontrado no GridComponent.");
            return;
        }

        field.SetValue(grid, chosen);
    }
}