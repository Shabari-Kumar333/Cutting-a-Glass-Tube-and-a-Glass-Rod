using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class DragDropManager : MonoBehaviour
{
    private Camera cam;
    private Transform draggedObject;
    private Vector3 offset;
    private float objectZ;

    private Vector3 startPos;
    private Quaternion startRot;

    [Header("Tags")]
    public string draggableTag = "Draggable";
    public string targetTag = "DropTarget";

    [Header("Drop Settings")]
    public float snapDistance = 1.5f;

    [Header("Particle Object")]
    [SerializeField] private GameObject particleObject;

    [Header("Camera Move Settings")]
    [SerializeField] private Transform cameraTargetPosition;
    [SerializeField] private float cameraMoveSpeed = 2f;
    [SerializeField] GameObject imagetodisappear;

    [Header("Distance Check (Optional)")]
    public bool calculateDistance = false;
    public float triggerDistance = 2f;
    public UnityEvent OnWithinDistance;
    private bool distanceEventTriggered = false;

    [Header("Events")]
    public UnityEvent OnDroppedCorrectly;

    void Start()
    {
        cam = Camera.main;
        Debug.Log("[MANAGER] Ready");
    }

    void Update()
    {
        HandleDrag();

        if (calculateDistance && draggedObject != null)
        {
            CheckDistanceToTarget();
        }
    }

    void HandleDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.transform.CompareTag(draggableTag))
            {
                if (imagetodisappear != null)
                {
                    imagetodisappear.SetActive(false);
                }

                draggedObject = hit.transform;
                startPos = draggedObject.position;
                startRot = draggedObject.rotation;

                objectZ = cam.WorldToScreenPoint(draggedObject.position).z;
                offset = draggedObject.position - GetMouseWorldPos();

                Debug.Log($"[DRAG START] {draggedObject.name}");
            }
        }

        if (Input.GetMouseButton(0) && draggedObject != null)
        {
            draggedObject.position = GetMouseWorldPos() + offset;
        }

        if (Input.GetMouseButtonUp(0) && draggedObject != null)
        {
            Debug.Log("[DRAG END]");
            TryDrop();
            draggedObject = null;
            distanceEventTriggered = false;
        }
    }

    Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = objectZ;
        return cam.ScreenToWorldPoint(mousePoint);
    }

    void TryDrop()
    {
        Collider[] hits = Physics.OverlapSphere(draggedObject.position, snapDistance);

        foreach (Collider col in hits)
        {
            if (col.CompareTag(targetTag))
            {
                Debug.Log($"[SUCCESS] Dropped on {col.name}");

                draggedObject.position = col.transform.position;
                draggedObject.rotation = col.transform.rotation;

                OnDroppedCorrectly.Invoke();
                StartCoroutine(MoveCameraToTarget());
                return;
            }
        }

        Debug.Log("[FAIL] No valid target — resetting");
        draggedObject.position = startPos;
        draggedObject.rotation = startRot;
    }

    void CheckDistanceToTarget()
    {
        if (cameraTargetPosition == null) return;

        float dist = Vector3.Distance(draggedObject.position, cameraTargetPosition.position);
        Debug.Log("" + dist);
        if (dist <= triggerDistance && !distanceEventTriggered)
        {
            distanceEventTriggered = true;
            Debug.Log("[DISTANCE] Object is close to target");
            OnWithinDistance?.Invoke();
        }

        if (dist > triggerDistance)
        {
            distanceEventTriggered = false;
        }
    }

    // 🎇 PARTICLE
    public void ParticleOn()
    {
        if (particleObject != null)
            particleObject.SetActive(true);
    }

    public void ParticleOff()
    {
        if (particleObject != null)
            particleObject.SetActive(false);
    }

    // 🎥 CAMERA MOVE
    IEnumerator MoveCameraToTarget()
    {
        if (cameraTargetPosition == null) yield break;

        Transform camTransform = cam.transform;
        Vector3 startPos = camTransform.position;
        Quaternion startRot = camTransform.rotation;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * cameraMoveSpeed;
            camTransform.position = Vector3.Lerp(startPos, cameraTargetPosition.position, t);
            camTransform.rotation = Quaternion.Lerp(startRot, cameraTargetPosition.rotation, t);
            yield return null;
        }
    }
}
