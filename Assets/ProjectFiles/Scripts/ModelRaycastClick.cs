using UnityEngine;
using UnityEngine.Events;

public class ModelRaycastClick : MonoBehaviour
{
    public Camera mainCamera;
    public Animator modelAnimator;
    public string parameterName = "Click";

    public UnityEvent onModelClicked;

    private bool playedOnce = false; // ✅ NEW (lock animation after first play)

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    void Update()
    {
        if (playedOnce) return; // ✅ Stop replay

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    Debug.Log("✅ Model Clicked!");

                    if (modelAnimator != null)
                    {
                        modelAnimator.SetTrigger(parameterName);
                        playedOnce = true; // ✅ Lock animation
                        Debug.Log("🎬 Animation Played Once & Locked");
                    }

                    onModelClicked?.Invoke();
                }
            }
        }
    }
}
