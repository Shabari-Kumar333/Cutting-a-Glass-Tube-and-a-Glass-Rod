using UnityEngine;

public class SlideEventManager : MonoBehaviour
{
    [Header("Target Slide")]
    public GameObject slide19GameObject;

    [Header("Objects to Control")]
    public GameObject objectToEnable;
    public GameObject objectToDisable;

    void Update()
    {
        // Safety Check: Did you forget to drag the slide?
        if (slide19GameObject == null)
        {
            Debug.LogError("STOP! You forgot to drag Slide19 into the Inspector!");
            return;
        }

        // The Logic
        if (slide19GameObject.activeInHierarchy)
        {
            // We think we are on Slide 19
            // Debug.Log("Script says: Slide 19 is ACTIVE"); 

            if (objectToEnable != null && !objectToEnable.activeSelf)
                objectToEnable.SetActive(true);

            if (objectToDisable != null && objectToDisable.activeSelf)
                objectToDisable.SetActive(false);
        }
        else
        {
            // We think we are NOT on Slide 19
            // Debug.Log("Script says: Slide 19 is INACTIVE");

            if (objectToEnable != null && objectToEnable.activeSelf)
                objectToEnable.SetActive(false);

            if (objectToDisable != null && !objectToDisable.activeSelf)
                objectToDisable.SetActive(true);
        }
    }
}