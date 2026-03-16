using UnityEngine;
using UnityEngine.Events;

public class RaycastAxisDrag : MonoBehaviour
{
    public enum DragAxis { X, Y, Z }

    [Header("Drag Axis")]
    public DragAxis axis = DragAxis.Z;

    [Header("Axis Limits (Relative to Start Position)")]
    public float positiveLimit = 10f;
    public float negativeLimit = -10f;

    [Header("Drag Complete Time (Seconds)")]
    public float dragCompleteTime = 2f;

    [Header("Objects")]
    public GameObject oldTube;
    public GameObject newTube;
    public ParticleSystem completeParticle;

    [Header("Shader Off Object")]
    public GameObject shaderOffObject;

    [Header("Audio")] // ✅ NEW SLOT
    public AudioSource completeAudio;

    [Header("Event")]
    public UnityEvent onComplete;

    private Camera cam;
    private bool isDragging;
    private bool isCompleted;

    private Vector3 startPos;
    private float startAxisValue;
    private Vector3 dragStartMouseWorld;
    private float dragStartAxisValue;

    private float dragTimer = 0f;

    void Start()
    {
        cam = Camera.main;
        startPos = transform.position;
        startAxisValue = GetAxisValue(startPos);

        if (newTube != null)
            newTube.SetActive(false);
    }

    void Update()
    {
        if (isCompleted) return;

        HandleDrag();
    }

    void HandleDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                if (hit.transform == transform || hit.transform.IsChildOf(transform))
                {
                    isDragging = true;
                    dragStartMouseWorld = hit.point;
                    dragStartAxisValue = GetAxisValue(transform.position);
                    dragTimer = 0f;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            dragTimer = 0f;
        }

        if (!isDragging) return;

        Ray dragRay = cam.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(cam.transform.forward * -1f, dragStartMouseWorld);

        if (plane.Raycast(dragRay, out float distance))
        {
            Vector3 currentMouseWorld = dragRay.GetPoint(distance);

            float delta = GetAxisValue(currentMouseWorld) - GetAxisValue(dragStartMouseWorld);
            float newAxisValue = dragStartAxisValue + delta;

            newAxisValue = Mathf.Clamp(
                newAxisValue,
                startAxisValue + negativeLimit,
                startAxisValue + positiveLimit
            );

            Vector3 pos = transform.position;
            SetAxisValue(ref pos, newAxisValue);
            transform.position = pos;
        }

        dragTimer += Time.deltaTime;

        if (dragTimer >= dragCompleteTime)
        {
            Complete();
        }
    }

    float GetAxisValue(Vector3 pos)
    {
        switch (axis)
        {
            case DragAxis.X: return pos.x;
            case DragAxis.Y: return pos.y;
            default: return pos.z;
        }
    }

    void SetAxisValue(ref Vector3 pos, float value)
    {
        switch (axis)
        {
            case DragAxis.X: pos.x = value; break;
            case DragAxis.Y: pos.y = value; break;
            case DragAxis.Z: pos.z = value; break;
        }
    }

    void Complete()
    {
        if (isCompleted) return;

        isCompleted = true;
        isDragging = false;

        if (oldTube != null)
            oldTube.SetActive(false);

        if (newTube != null)
            newTube.SetActive(true);

        if (completeParticle != null)
            completeParticle.Play();

        if (shaderOffObject != null)
            shaderOffObject.SetActive(false);

        if (completeAudio != null)   // ✅ AUDIO PLAY
            completeAudio.Play();

        onComplete?.Invoke();
    }
}
