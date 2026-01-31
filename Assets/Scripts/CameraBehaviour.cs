using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    [SerializeField] private Vector3 initialOffset;
    [SerializeField] private float speedFollowTarget = 100;
    [SerializeField] private float sensivityTouch = 10;
    [SerializeField] private Vector2 maxLimited;
    [SerializeField] private Vector2 minLimited;
    [Space]
    [SerializeField] private GameManager gameManager = null;

    private Transform target;

    Vector3 distanceFromCamera;
    Vector3 touchStart;
    Plane plane;

    private delegate void GetInTargetDelegate();
    private GetInTargetDelegate getInTargetEvent;
    private ActionPanel actionPanel;

    private void Awake()
    {
        actionPanel = FindObjectOfType<ActionPanel>();
    }

    private void Start()
    {
        if(target != null)
            initialOffset = transform.position - target.position;
    }

    private void Update()
    {
        if (!gameManager.Pause)
        {
#if UNITY_STANDALONE || UNITY_EDITOR
            DragMouse();
#else
            DragTouch();
#endif

            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, minLimited.x, maxLimited.x),
                transform.position.y,
                Mathf.Clamp(transform.position.z, minLimited.y, maxLimited.y));
        }
    }

    private void DragMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            touchStart = GetWorldPoint(Input.mousePosition);
        }
        if(Input.GetMouseButton(0))
        {
            Vector3 currentPosition = GetWorldPoint(Input.mousePosition);

            if(Vector3.Distance(touchStart, currentPosition) > 1)
            {
                SetTarget(null);

                Vector3 worldDelta = currentPosition - touchStart;
                transform.position -= new Vector3(worldDelta.x * sensivityTouch * Time.deltaTime, 
                    0, worldDelta.z * sensivityTouch * Time.deltaTime);
            }
        }
    }

    private void DragTouch()
    {
        if (Input.touchCount == 1)
        {
            Touch currentTouch = Input.GetTouch(0);

            if (currentTouch.phase == TouchPhase.Began)
            {
                touchStart = GetWorldPoint(currentTouch.position);
            }
            if (currentTouch.phase == TouchPhase.Moved)
            {
                SetTarget(null);

                Vector3 worldDelta = GetWorldPoint(currentTouch.position) - touchStart;
                transform.position -= new Vector3(worldDelta.x * sensivityTouch * Time.deltaTime, 0, worldDelta.z * sensivityTouch * Time.deltaTime);
            }
        }
    }

    private Vector3 GetWorldPoint(Vector3 screenPoint)
    {
        RaycastHit hit;
        Physics.Raycast(Camera.main.ScreenPointToRay(screenPoint), out hit);
        return hit.point;
    }

    
    private void LateUpdate()
    {
        if (!gameManager.Pause && target != null)
        {
            Vector3 targetPosition = new Vector3(target.position.x - initialOffset.x, transform.position.y - initialOffset.y, target.position.z - initialOffset.z);
            Vector3 targetPositionMove = Vector3.MoveTowards(transform.position, targetPosition, speedFollowTarget * Time.deltaTime);
            transform.position = targetPositionMove;
        }
    }
    

    public void SetTarget(Transform newTarget)
    {
        if (!gameManager.Pause && newTarget != target)
        {
            target = newTarget;

            actionPanel.Hide();
        }
    }
}
