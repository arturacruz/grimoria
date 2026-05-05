using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Store.Scripts
{
    public class Light : MonoBehaviour
    {
        public Light2D light2D;
        public float min = 0.8f;
        public float max = 1.2f;

        void Update()
        {
            light2D.intensity = Random.Range(min, max);
        }
    }

}