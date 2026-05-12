using UnityEngine;

namespace Store.Scripts
{

    public class Inventory : MonoBehaviour
    {
        [SerializeField] private Vector3 closedPos;
        [SerializeField] private Vector3 openPos;
        [SerializeField] private float speed = 8f;

        private bool open;

        private void Update()
        {
            Vector3 target = open ? openPos : closedPos;
            transform.localPosition = Vector3.MoveTowards(
                transform.localPosition,
                target,
                speed * Time.deltaTime
            );
        }

        public void Toggle()
        {
            open = !open;
        }
    }
}