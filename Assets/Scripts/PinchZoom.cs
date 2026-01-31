using UnityEngine;

public class PinchZoom : MonoBehaviour
{
    [SerializeField] private float perspectiveZoomSpeed = 0.1f;
    [Space]
    [SerializeField] private float minZoom = 60f;
    [SerializeField] private float maxZoom = 80f;
    [Space]
    [SerializeField] private GameManager gameManager = null;

    void Update()
    {
        if (Input.touchCount == 2 && !gameManager.Pause)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
            GetComponent<Camera>().fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;

            GetComponent<Camera>().fieldOfView = Mathf.Clamp(GetComponent<Camera>().fieldOfView, minZoom, maxZoom);
        }
    }
}