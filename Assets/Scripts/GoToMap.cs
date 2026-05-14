using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToMap: MonoBehaviour
{
    public void go_to_map()
    {
        SceneManager.LoadScene("MapScene");
    }
}
