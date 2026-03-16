using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events; // ✅ added

public class ModelDragSnap3D : MonoBehaviour
{
    public Transform snapPoint;
    public float snapDistance = 2f;

    private Camera cam;
    private bool isDragging;
    private Vector3 offset;
    private float zDistance;
    private bool isSnapped;

    public UnityEvent onSnapEvent; // ✅ added event list only

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (isSnapped) return;

        if (Input.GetMouseButtonDown(0))
            TryStartDrag(Input.mousePosition);

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            CheckSnap();
        }

        if (isDragging)
            DragObject(Input.mousePosition);

        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Began)
                TryStartDrag(t.position);

            if (t.phase == TouchPhase.Moved && isDragging)
                DragObject(t.position);

            if (t.phase == TouchPhase.Ended)
            {
                isDragging = false;
                CheckSnap();
            }
        }
    }

    void TryStartDrag(Vector3 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
        {
            isDragging = true;
            zDistance = cam.WorldToScreenPoint(transform.position).z;

            Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, zDistance));
            offset = transform.position - worldPos;
        }
    }

    void DragObject(Vector3 screenPos)
    {
        Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, zDistance));
        transform.position = worldPos + offset;
    }

    void CheckSnap()
    {
        if (snapPoint == null) return;

        float dist = Vector3.Distance(transform.position, snapPoint.position);
        Debug.Log("Snap Distance = " + dist);

        if (dist < snapDistance)
        {
            isSnapped = true;
            transform.position = snapPoint.position;
            transform.rotation = snapPoint.rotation;
            Debug.Log("✅ SNAPPED SUCCESS!");

            onSnapEvent.Invoke(); // ✅ event call
        }
    }
}
