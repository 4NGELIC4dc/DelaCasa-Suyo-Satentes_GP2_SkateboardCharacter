using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float sensitivity = 2f;
    public float zoomSpeed = 10f;
    public float minZoom = 5f;
    public float maxZoom = 500f;

    private bool isDragging = false;
    private Vector3 lastMousePosition;

    void Update()
    {
        Vector3 delta = Vector3.zero;

        // Right button click function (rotate camera)
        if (Input.GetMouseButtonDown(1))
        {
            isDragging = true;
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            delta = Input.mousePosition - lastMousePosition;
            float rotationX = -delta.y * sensitivity;
            float rotationY = delta.x * sensitivity;

            transform.eulerAngles += new Vector3(rotationX, rotationY, 0);
        }

        // Middle button click function (move camera)
        if (Input.GetMouseButton(2))
        {
            delta = Input.mousePosition - lastMousePosition;
            Vector3 pan = new Vector3(-delta.x, -delta.y, 0) * sensitivity * 0.01f;
            transform.Translate(pan, Space.Self);
        }

        // Middle button scroll function (zoom in and out)
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            Vector3 zoomDirection = transform.forward * scroll * zoomSpeed;
            Vector3 newPosition = transform.position + zoomDirection;

            float newDistance = (newPosition - Vector3.zero).magnitude;
            if (newDistance > minZoom && newDistance < maxZoom)
            {
                transform.position = newPosition;
            }
        }

        lastMousePosition = Input.mousePosition;
    }
}