using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }
}