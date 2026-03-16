using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class MultiStepDialerController : MonoBehaviour
{
    [Header("All Input Fields (Top first, Down last)")]
    public TMP_InputField[] dialFields;

    [Header("Manual Answers (Top Boxes Only)")]
    public float[] manualAnswers;

    [Header("Auto-Fill Values (Down Boxes Only)")]
    public float[] autoFillValues;

    [Header("Correct Symbols")]
    public GameObject[] successIcons;

    [Header("Buttons")]
    public Button validateButton;
    public Button autoFillButton;

    [Header("Settings")]
    public int maxWrongAttempts = 3;

    [Header("Events")]
    public UnityEvent OnCorrectAnswer;
    public UnityEvent OnAllAnswersCorrect;
    public UnityEvent OnReferenceEvent;

    public UnityEvent OnWrongAnswer; // ⭐ NEW EVENT

    const float TOL = 0.001f;

    int activeIndex = 0;
    int wrongAttempts = 0;
    bool allSolved = false;

    void Start()
    {
        int totalCount = manualAnswers.Length + autoFillValues.Length;

        if (dialFields.Length != totalCount || successIcons.Length != totalCount)
        {
            Debug.LogError("Field/Icon size mismatch");
            return;
        }

        for (int i = 0; i < dialFields.Length; i++)
        {
            dialFields[i].text = "";
            dialFields[i].interactable = (i == 0);
            successIcons[i]?.SetActive(false);
        }

        autoFillButton?.gameObject.SetActive(false);

        validateButton?.onClick.AddListener(OnValidatePressed);
        autoFillButton?.onClick.AddListener(AutoFillAll);
    }

    public void OnDecimalPressed()
    {
        if (activeIndex >= manualAnswers.Length) return;

        if (!dialFields[activeIndex].text.Contains("."))
        {
            if (dialFields[activeIndex].text == "")
                dialFields[activeIndex].text = "0.";
            else
                dialFields[activeIndex].text += ".";
        }
    }

    public void OnDigitPressed(string digit)
    {
        if (activeIndex >= manualAnswers.Length) return;
        dialFields[activeIndex].text += digit;
    }

    public void OnBackspacePressed()
    {
        if (activeIndex >= manualAnswers.Length) return;

        string t = dialFields[activeIndex].text;
        if (t.Length > 0)
            dialFields[activeIndex].text = t[..^1];
    }

    // ================= CHECK =================

    public void OnValidatePressed()
    {
        if (allSolved || activeIndex >= manualAnswers.Length) return;

        if (!float.TryParse(dialFields[activeIndex].text, out float val))
            return;

        if (Mathf.Abs(val - manualAnswers[activeIndex]) > TOL)
        {
            dialFields[activeIndex].text = "";
            wrongAttempts++;

            OnWrongAnswer?.Invoke(); // ⭐ TRIGGER WRONG EVENT

            if (wrongAttempts >= maxWrongAttempts)
            {
                autoFillButton?.gameObject.SetActive(true);
            }
            return;
        }

        // ✅ Correct
        successIcons[activeIndex]?.SetActive(true);
        dialFields[activeIndex].interactable = false;
        OnCorrectAnswer?.Invoke();

        activeIndex++;

        if (activeIndex < manualAnswers.Length)
        {
            dialFields[activeIndex].interactable = true;
        }
        else
        {
            AutoFillDownBoxes();
        }
    }

    void AutoFillDownBoxes()
    {
        int start = manualAnswers.Length;

        for (int i = 0; i < autoFillValues.Length; i++)
        {
            int idx = start + i;
            dialFields[idx].text = autoFillValues[i].ToString();
            dialFields[idx].interactable = false;
            successIcons[idx]?.SetActive(true);
        }

        FinishPuzzle();
    }

    public void AutoFillAll()
    {
        for (int i = 0; i < manualAnswers.Length; i++)
        {
            dialFields[i].text = manualAnswers[i].ToString();
            dialFields[i].interactable = false;
            successIcons[i]?.SetActive(true);
        }

        AutoFillDownBoxes();
    }

    void FinishPuzzle()
    {
        if (allSolved) return;

        allSolved = true;
        autoFillButton?.gameObject.SetActive(false);

        OnReferenceEvent?.Invoke();
        OnAllAnswersCorrect?.Invoke();
    }

    public void ResetAll()
    {
        activeIndex = 0;
        wrongAttempts = 0;
        allSolved = false;

        autoFillButton?.gameObject.SetActive(false);

        for (int i = 0; i < dialFields.Length; i++)
        {
            dialFields[i].text = "";
            dialFields[i].interactable = (i == 0);
            successIcons[i]?.SetActive(false);
        }
    }
}
