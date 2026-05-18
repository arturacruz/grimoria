using UnityEngine;

public class color : MonoBehaviour
{
    SpriteRenderer sprite;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();

        sprite.color *= 1.37f;
    }
}
