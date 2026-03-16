using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SlideTaskLock : MonoBehaviour
{
    [Header("Navigation")]
    public Button nextButton;

    // MEMORY: This variable remembers if the task is done.
    // "private" means only this script knows, but it keeps the value 
    // as long as the game is running.
    private bool isTaskCompleted = false;

    private void OnEnable()
    {
        // CHECK MEMORY: Did we already do this?
        if (isTaskCompleted)
        {
            // YES: Just make sure the button is ON and exit.
            if (nextButton != null) nextButton.interactable = true;
        }
        else
        {
            // NO: We haven't done it yet. Lock the button.
            StartCoroutine(ForceLockRoutine());
        }
    }

    private IEnumerator ForceLockRoutine()
    {
        yield return null; // Wait for SlideManager to finish

        // Double check: If we completed the task inside this single frame, don't lock it!
        if (nextButton != null && !isTaskCompleted)
        {
            nextButton.interactable = false;
        }
    }

    // --- YOUR UNLOCK FUNCTIONS ---

    public void UnlockNavigation()
    {
        // 1. Mark as Done forever
        isTaskCompleted = true;

        // 2. Physically unlock the button
        if (nextButton != null)
        {
            nextButton.interactable = true;
        }
    }

    // For your MCQ Buttons
    public void SubmitAnswer(bool isCorrect)
    {
        if (isCorrect)
        {
            UnlockNavigation(); // This saves the memory too!
        }
    }
}