using UnityEngine;
using UnityEngine.Events;

public class ClickOrTouchEvent_TH : MonoBehaviour
{
    [Header("Constraint (IMPORTANT)")]
    // Drag the specific UI Slide (e.g., Slide19) here.
    public GameObject requiredSlide;

    [Header("Event to Trigger on Click/Touch")]
    public UnityEvent onClicked; // Drag your Animations or Sounds here

    // --- ADDED THIS SECTION TO MATCH YOUR IMAGE ---
    [Header("Unlock Action")]
    public UnityEvent onTaskComplete; // Drag your SlideTaskLock.UnlockNavigation here

    private void Update()
    {
        // For mouse click or touch
        if (Input.GetMouseButtonDown(0))
        {
            // --- GATEKEEPER CHECK ---
            if (requiredSlide != null && !requiredSlide.activeInHierarchy)
            {
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == transform)
                {
                    // 1. Play the Animation
                    onClicked.Invoke();

                    // 2. Unlock the Next Button
                    onTaskComplete.Invoke();
                }
            }
        }
    }
}