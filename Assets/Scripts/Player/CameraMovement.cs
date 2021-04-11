using Managers;
using UnityEngine;

namespace Player
{
    public class CameraMovement : MonoBehaviour
    {
        [SerializeField] private float panSpeed = 20f;
        [SerializeField] private float panBorderThickness = 10f;

        [SerializeField] private Vector2 panLimit;

        [SerializeField] private float scrollSpeed = 20f;
        [SerializeField] private float minY = 15f;
        [SerializeField] private float maxY = 50f;

        [SerializeField] private GameManager gameManager;

        private void Update()
        {
            var elapsedTime = Time.unscaledDeltaTime;
            var pos = transform.position;

            var horizontalInput = Input.GetAxisRaw("Horizontal");
            var verticalInput = Input.GetAxisRaw("Vertical");
            var scroll = Input.GetAxisRaw("Mouse ScrollWheel");

            if (gameManager.MouseControl)
            {
                if (Input.mousePosition.y >= Screen.height - panBorderThickness) verticalInput = 1;
                if (Input.mousePosition.y <= panBorderThickness) verticalInput = -1;
                if (Input.mousePosition.x >= Screen.width - panBorderThickness) horizontalInput = 1;
                if (Input.mousePosition.x <= panBorderThickness) horizontalInput = -1;
            }

            pos.x += horizontalInput * panSpeed * elapsedTime;
            pos.z += verticalInput * panSpeed * elapsedTime;
            pos.y -= scroll * scrollSpeed * 100f * elapsedTime;

            pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
            pos.z = Mathf.Clamp(pos.z, -panLimit.y, panLimit.y);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);

            transform.position = pos;
        }
    }
}