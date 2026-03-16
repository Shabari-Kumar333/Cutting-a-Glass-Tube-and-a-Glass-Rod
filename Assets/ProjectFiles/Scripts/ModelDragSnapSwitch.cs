using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class ModelDragSnapSwitch : MonoBehaviour
{
    public enum DragAxis { X, Y, Z, Free }

    [Header("Snap Settings")]
    public Transform snapTarget;
    public float snapDistance = 1f;

    [Header("Axis Control")]
    public DragAxis dragAxis = DragAxis.Free;

    [Header("Model References")]
    public GameObject model1;   // Drag model
    public GameObject model2;   // Swap model
    public GameObject model3;   // Only mesh disable

    [Header("Swap Smooth Settings")]
    public float swapSpeed = 5f;

    [Header("Animation")]
    public Animator animator;
    public string animatorTriggerName = "Play";

    [Header("After Animation Event")]
    public UnityEvent onAnimationFinished;

    private Camera cam;
    private bool isDragging = false;
    private bool isSnapped = false;
    private bool isCompleted = false;

    private Vector3 offset;
    private float zDistance;

    void Start()
    {
        cam = Camera.main;
    }

    void OnMouseDown()
    {
        if (isSnapped || isCompleted) return;

        isDragging = true;

        zDistance = cam.WorldToScreenPoint(transform.position).z;
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = zDistance;

        offset = transform.position - cam.ScreenToWorldPoint(mousePos);
    }

    void OnMouseDrag()
    {
        if (!isDragging || isSnapped || isCompleted) return;

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = zDistance;

        Vector3 worldPos = cam.ScreenToWorldPoint(mousePos) + offset;

        Vector3 newPos = transform.position;

        switch (dragAxis)
        {
            case DragAxis.X: newPos.x = worldPos.x; break;
            case DragAxis.Y: newPos.y = worldPos.y; break;
            case DragAxis.Z: newPos.z = worldPos.z; break;
            case DragAxis.Free: newPos = worldPos; break;
        }

        transform.position = newPos;
    }

    void OnMouseUp()
    {
        if (isSnapped || isCompleted) return;

        isDragging = false;

        if (Vector3.Distance(transform.position, snapTarget.position) <= snapDistance)
        {
            StartCoroutine(SnapSequence());
        }
    }

    IEnumerator SnapSequence()
    {
        isSnapped = true;
        transform.position = snapTarget.position;

        // 🔥 Disable ONLY MeshRenderer of Model3
        if (model3 != null)
        {
            MeshRenderer[] renderers = model3.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer r in renderers)
                r.enabled = false;
        }

        // Enable Model2 for smooth swap
        if (model2 != null)
        {
            model2.SetActive(true);
            model2.transform.position = model1.transform.position;
        }

        // Smooth swap movement
        float t = 0f;
        Vector3 startScale = model1.transform.localScale;
        Vector3 targetScale = Vector3.zero;

        while (t < 1f)
        {
            t += Time.deltaTime * swapSpeed;
            model1.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        // 🔥 RUN EVENT AFTER SNAP
        onAnimationFinished?.Invoke();

        // Play Animation
        if (animator != null)
            animator.SetTrigger(animatorTriggerName);
    }


    // 🔥 Called from Animation Event
    public void AnimationFinished()
    {
        isCompleted = true;

        if (model1 != null)
            model1.SetActive(false);

        onAnimationFinished?.Invoke();
    }
}
