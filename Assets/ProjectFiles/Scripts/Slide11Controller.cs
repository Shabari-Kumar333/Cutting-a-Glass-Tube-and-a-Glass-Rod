using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class Slide11Controller : MonoBehaviour
{
    [Header("UI")]
    public GameObject upArrow;
    public GameObject downArrow;
    public GameObject correctImage;
    public GameObject wrongImage;
    public GameObject wrongPanel;

    [Header("Buttons")]
    public Button correctButton;
    public Button wrongButton;

    [Header("Animation")]
    public Animator animator;
    public string animatorBoolName = "PlayAnim";
    public float delayAfterCorrectImage = 0.5f;
    public float delayBeforeAnimation = 1f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip correctAudio;
    public AudioClip wrongAudio;

    [Header("Animation Audio (NEW)")]
    public AudioClip animationAudio;   // 👈 NEW Reference Only

    [Header("Event")]
    public UnityEvent onSlideLocked;

    private bool slideLocked = false;
    private bool animationCompleted = false;

    void OnEnable()
    {
        if (animationCompleted)
        {
            upArrow.SetActive(false);
            downArrow.SetActive(false);
            correctImage.SetActive(true);
            wrongPanel.SetActive(false);
            slideLocked = true;
        }
        else
        {
            upArrow.SetActive(true);
            downArrow.SetActive(true);

            correctImage.SetActive(false);
            wrongImage.SetActive(false);
            wrongPanel.SetActive(false);

            slideLocked = false;
        }
    }

    void Start()
    {
        correctButton.onClick.AddListener(OnCorrectClick);
        wrongButton.onClick.AddListener(OnWrongClick);
    }

    void OnWrongClick()
    {
        if (slideLocked) return;

        wrongImage.SetActive(true);
        wrongPanel.SetActive(true);

        if (audioSource != null && wrongAudio != null)
        {
            audioSource.PlayOneShot(wrongAudio);
        }
    }

    void OnCorrectClick()
    {
        if (slideLocked) return;

        if (audioSource != null && correctAudio != null)
        {
            audioSource.PlayOneShot(correctAudio);
        }

        StartCoroutine(CorrectSequence());
    }

    IEnumerator CorrectSequence()
    {
        correctImage.SetActive(true);

        yield return new WaitForSeconds(delayAfterCorrectImage);

        wrongPanel.SetActive(false);
        upArrow.SetActive(false);
        downArrow.SetActive(false);

        yield return new WaitForSeconds(delayBeforeAnimation);

        if (animator != null)
        {
            animator.SetBool(animatorBoolName, true);

            // 🔊 PLAY ANIMATION AUDIO HERE
            if (audioSource != null && animationAudio != null)
            {
                audioSource.PlayOneShot(animationAudio);
            }

            float length = animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(length);
        }

        slideLocked = true;
        animationCompleted = true;

        onSlideLocked?.Invoke();
    }
}
