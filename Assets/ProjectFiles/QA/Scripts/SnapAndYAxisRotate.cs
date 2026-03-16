using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class DragSnapXY : MonoBehaviour
{
    [Header("Drag (XY Only)")]
    [SerializeField] private float minX = -5f;
    [SerializeField] private float maxX = 5f;
    [SerializeField] private float minY = -3f;
    [SerializeField] private float maxY = 3f;

    [Header("Snap")]
    [SerializeField] private Transform snapTarget;
    [SerializeField] private float snapDistance = 0.25f;

    private Camera mainCam;
    private bool isDragging;
    public bool isSnapped;

    private float fixedZ;
    private float screenDepth;
    public GameObject[] slides;

    private void Awake()
    {
        mainCam = Camera.main;
        fixedZ = transform.position.z;
    }

    private void Update()
    {
        // Check slide state first
        if (IsAnySlideActive())
        {
            isDragging = false;
            return;
        }

        if (!isDragging || isSnapped)
            return;

        DragXYOnlyClamped();
        CheckForSnap();
    }

    private bool IsAnySlideActive()
    {
        foreach (GameObject slide in slides)
        {
            if (slide != null && slide.activeSelf)
                return true;
        }
        return false;
    }

    private void OnMouseDown()
    {
        // Prevent drag if snapped or slide is active
        if (isSnapped || IsAnySlideActive())
            return;

        screenDepth = mainCam.WorldToScreenPoint(transform.position).z;
        isDragging = true;
    }

    private void OnMouseUp()
    {
        isDragging = false;
    }

    private void DragXYOnlyClamped()
    {
        Vector3 mouseScreenPos = new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            screenDepth
        );

        Vector3 worldPos = mainCam.ScreenToWorldPoint(mouseScreenPos);

        float clampedX = Mathf.Clamp(worldPos.x, minX, maxX);
        float clampedY = Mathf.Clamp(worldPos.y, minY, maxY);

        transform.position = new Vector3(clampedX, clampedY, fixedZ);
    }

    private void CheckForSnap()
    {
        if (!snapTarget)
            return;

        if (Vector3.Distance(transform.position, snapTarget.position) <= snapDistance)
        {
            Snap();
        }
    }

    private void Snap()
    {
        transform.position = snapTarget.position;
        transform.rotation = snapTarget.rotation;

        isSnapped = true;
        isDragging = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (!snapTarget)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(snapTarget.position, snapDistance);
    }
}
