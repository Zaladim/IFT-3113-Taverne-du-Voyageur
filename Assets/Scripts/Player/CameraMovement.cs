using System;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float panSpeed = 20f;
    [SerializeField] private float panBorderThickness = 10f;

    [SerializeField] private Vector2 panLimit;

    [SerializeField] private float scrollSpeed = 20f;
    [SerializeField] private float minY = 15f;
    [SerializeField] private float maxY = 50f;
    
    private void Update()
    {
        Vector3 pos = transform.position;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        
        if (Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            verticalInput = 1;
        }
        
        if (Input.mousePosition.y <= panBorderThickness)
        {
            verticalInput = -1;
        }
        
        if (Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            horizontalInput = 1;
        }
        
        if (Input.mousePosition.x <= panBorderThickness)
        {
            horizontalInput = -1;
        }

        pos.x += horizontalInput * panSpeed * Time.deltaTime;
        pos.z += verticalInput * panSpeed * Time.deltaTime;
        pos.y -= scroll * scrollSpeed * 100f * Time.deltaTime;
            
        pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
        pos.z = Mathf.Clamp(pos.z, -panLimit.y, panLimit.y);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        
        transform.position = pos;
        
    }
}
