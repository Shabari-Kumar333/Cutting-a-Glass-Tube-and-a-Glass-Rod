using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Events;

public class SlideManager_TH : MonoBehaviour
{
    [System.Serializable]
    public class SlideEventData
    {
        public List<GameObject> enableObjects;   // objects to enable in this slide
        public List<GameObject> disableObjects;  // objects to disable in this slide
        public UnityEvent onSlideShow;           // custom event
    }

    [Header("Slides (Panels)")]
    public List<GameObject> slides = new List<GameObject>();

    [Header("Camera Positions (Optional)")]
    public List<Transform> cameraPositions = new List<Transform>();
    public Camera mainCamera;

    [Header("UI Buttons")]
    public Button nextButton;
    public Button prevButton;

    [Header("Slide Text")]
    public TextMeshProUGUI slideText;

    [Header("Page Completion Settings")]
    public bool usePageCompletionLock = false;

    [Header("Slide Events (Same Index as Slides)")]
    public List<SlideEventData> slideEvents = new List<SlideEventData>(); // ✅ NEW LIST

    private int currentSlideIndex = 0;

    void Start()
    {
        ShowSlide(currentSlideIndex);

        prevButton.interactable = false;

        if (usePageCompletionLock)
            nextButton.interactable = false;
        else
            nextButton.interactable = true;
    }

    void ShowSlide(int index)
    {
        for (int i = 0; i < slides.Count; i++)
        {
            slides[i].SetActive(i == index);
        }

        HandleSlideObjects(index); // ✅ NEW
        UpdateSlideText();
        UpdateButtons();
        UpdateCameraPosition();
    }

    void HandleSlideObjects(int index)
    {
        if (index >= slideEvents.Count) return;

        SlideEventData data = slideEvents[index];

        // Enable objects
        if (data.enableObjects != null)
        {
            foreach (GameObject obj in data.enableObjects)
            {
                if (obj != null)
                    obj.SetActive(true);
            }
        }

        // Disable objects
        if (data.disableObjects != null)
        {
            foreach (GameObject obj in data.disableObjects)
            {
                if (obj != null)
                    obj.SetActive(false);
            }
        }

        // Invoke custom event
        data.onSlideShow?.Invoke();
    }

    void UpdateSlideText()
    {
        if (slideText != null)
        {
            slideText.text = (currentSlideIndex + 1) + "/" + slides.Count;
        }
    }

    void UpdateButtons()
    {
        prevButton.interactable = currentSlideIndex > 0;

        if (!usePageCompletionLock)
        {
            nextButton.interactable = currentSlideIndex < slides.Count - 1;
        }
    }

    void UpdateCameraPosition()
    {
        if (mainCamera != null && cameraPositions.Count > currentSlideIndex && cameraPositions[currentSlideIndex] != null)
        {
            mainCamera.transform.position = cameraPositions[currentSlideIndex].position;
            mainCamera.transform.rotation = cameraPositions[currentSlideIndex].rotation;
        }
    }

    public void NextSlide()
    {
        if (currentSlideIndex < slides.Count - 1)
        {
            currentSlideIndex++;
            ShowSlide(currentSlideIndex);

            if (usePageCompletionLock)
                nextButton.interactable = false;
        }
    }

    public void PrevSlide()
    {
        if (currentSlideIndex > 0)
        {
            currentSlideIndex--;
            ShowSlide(currentSlideIndex);

            if (usePageCompletionLock)
                nextButton.interactable = true;
        }
    }

    public void CompleteCurrentPage()
    {
        if (!usePageCompletionLock) return;

        Debug.Log("✅ Current Page Completed: " + currentSlideIndex);
        nextButton.interactable = true;
    }

    public void LockNextButton()
    {
        if (!usePageCompletionLock) return;

        nextButton.interactable = false;
    }
}
