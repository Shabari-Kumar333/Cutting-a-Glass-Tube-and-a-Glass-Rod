using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Events;

public class SlideManager : MonoBehaviour
{
    [System.Serializable]
    public class SlideEventData
    {
        public List<GameObject> enableObjects;
        public List<GameObject> disableObjects;
        public UnityEvent onSlideShow;
    }

    [Header("Slides (Panels)")]
    public List<GameObject> slides = new List<GameObject>();

    [Header("Camera Positions (Optional)")]
    public List<Transform> cameraPositions = new List<Transform>();
    public Camera mainCamera;

    [Header("Camera Smooth Settings (NEW)")]
    public float cameraMoveSpeed = 5f;

    [Header("UI Buttons")]
    public Button nextButton;
    public Button prevButton;

    [Header("Slide Text")]
    public TextMeshProUGUI slideText;

    [Header("Page Completion Settings")]
    public bool usePageCompletionLock = false;

    [Header("Slides Without Lock (Index List)")]
    public List<int> noLockSlides = new List<int>();

    [Header("Slide Events (Same Index as Slides)")]
    public List<SlideEventData> slideEvents = new List<SlideEventData>();

    private int currentSlideIndex = 0;
    private List<bool> slideCompleted = new List<bool>();

    private Vector3 targetPosition;
    private Quaternion targetRotation;

    void Start()
    {
        slideCompleted.Clear();
        for (int i = 0; i < slides.Count; i++)
        {
            slideCompleted.Add(false);
        }

        ShowSlide(currentSlideIndex);
        prevButton.interactable = false;
    }

    void Update()
    {
        SmoothCameraMove();
    }

    void ShowSlide(int index)
    {
        for (int i = 0; i < slides.Count; i++)
        {
            slides[i].SetActive(i == index);
        }

        HandleSlideObjects(index);
        UpdateSlideText();
        UpdateCameraPosition();
        UpdateButtons();
    }

    void HandleSlideObjects(int index)
    {
        if (index >= slideEvents.Count) return;

        SlideEventData data = slideEvents[index];

        if (data.enableObjects != null)
            foreach (GameObject obj in data.enableObjects)
                if (obj != null) obj.SetActive(true);

        if (data.disableObjects != null)
            foreach (GameObject obj in data.disableObjects)
                if (obj != null) obj.SetActive(false);

        data.onSlideShow?.Invoke();
    }

    void UpdateSlideText()
    {
        if (slideText != null)
            slideText.text = (currentSlideIndex + 1) + "/" + slides.Count;
    }

    void UpdateButtons()
    {
        prevButton.interactable = currentSlideIndex > 0;

        if (!usePageCompletionLock)
        {
            nextButton.interactable = currentSlideIndex < slides.Count - 1;
        }
        else
        {
            if (noLockSlides.Contains(currentSlideIndex))
            {
                nextButton.interactable = true;
            }
            else
            {
                nextButton.interactable = slideCompleted[currentSlideIndex];
            }
        }
    }

    // 🔥 Modified – Now Smooth
    void UpdateCameraPosition()
    {
        if (mainCamera != null &&
            cameraPositions.Count > currentSlideIndex &&
            cameraPositions[currentSlideIndex] != null)
        {
            targetPosition = cameraPositions[currentSlideIndex].position;
            targetRotation = cameraPositions[currentSlideIndex].rotation;
        }
    }

    void SmoothCameraMove()
    {
        if (mainCamera == null) return;

        mainCamera.transform.position = Vector3.Lerp(
            mainCamera.transform.position,
            targetPosition,
            Time.deltaTime * cameraMoveSpeed
        );

        mainCamera.transform.rotation = Quaternion.Lerp(
            mainCamera.transform.rotation,
            targetRotation,
            Time.deltaTime * cameraMoveSpeed
        );
    }

    public void NextSlide()
    {
        if (currentSlideIndex < slides.Count - 1)
        {
            currentSlideIndex++;
            ShowSlide(currentSlideIndex);
        }
    }

    public void PrevSlide()
    {
        if (currentSlideIndex > 0)
        {
            currentSlideIndex--;
            ShowSlide(currentSlideIndex);
        }
    }

    public void CompleteCurrentPage()
    {
        if (!usePageCompletionLock) return;

        slideCompleted[currentSlideIndex] = true;
        nextButton.interactable = true;
        Debug.Log("✅ Slide Completed: " + currentSlideIndex);
    }

    public void LockNextButton()
    {
        if (!usePageCompletionLock) return;

        slideCompleted[currentSlideIndex] = false;
        nextButton.interactable = false;
    }
}
