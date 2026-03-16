using UnityEngine;
using UnityEngine.Events; // Added this for the Unlock Event

public class ObjectTrigger : MonoBehaviour
{
    [Header("Constraint (IMPORTANT)")]
    // Drag the specific UI Slide (e.g., Slide16) here. 
    // If this slide is NOT active, the click will be ignored.
    public GameObject requiredSlide;

    [Header("Objects to show when clicked")]
    public GameObject[] objectsToEnable;

    [Header("Unlock Action")]
    // Drag your 'Next' button here in the Inspector -> Select "Button.interactable = bool" -> Check the box
    public UnityEvent onTaskComplete;

    private void OnMouseDown()
    {
        // --- GATEKEEPER CHECK ---
        // If we assigned a slide, and that slide is currently hidden (inactive), STOP here.
        if (requiredSlide != null && !requiredSlide.activeInHierarchy)
        {
            return;
        }

        Debug.Log($"Clicked on: {gameObject.name}");

        // 1. Enable the objects (Turn on flame, etc.)
        foreach (GameObject obj in objectsToEnable)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }

        // 2. Unlock the Next Button (or trigger any other success logic)
        onTaskComplete.Invoke();
    }
}