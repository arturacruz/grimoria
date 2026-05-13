using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;

    private void LateUpdate()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        Vector3 cameraPosition = targetCamera.transform.position;

        transform.position = new Vector3(
            cameraPosition.x,
            cameraPosition.y,
            transform.position.z
        );
    }
}