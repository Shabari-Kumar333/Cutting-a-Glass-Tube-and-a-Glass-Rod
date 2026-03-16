using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class DualLiquidFillController : MonoBehaviour
{
    [Header("Renderers (NOT Materials)")]
    [SerializeField] private Renderer increaseRenderer;
    [SerializeField] private Renderer decreaseRenderer;

    private Material increaseMaterial;
    private Material decreaseMaterial;

    [Header("Speed Mode Settings")]
    [SerializeField] private float fillSpeed = 0.5f;
    [SerializeField] private float minFill = -0.5f;
    [SerializeField] private float maxFill = 0.5f;

    [Header("Time Mode Settings")]
    [SerializeField] private bool useTime = false;
    [SerializeField] private float transferDuration = 2f;

    [Header("Advanced Time Settings (Transfer Only)")]
    [SerializeField] private float increaseDuration = 2f;
    [SerializeField] private float decreaseDuration = 2f;

    [Header("Events")]
    public UnityEvent OnIncreaseFillComplete;

    private Coroutine increaseRoutine;
    private Coroutine decreaseRoutine;
    private Coroutine transferRoutine;

    private bool eventFired = false;

    void Start()
    {
        increaseMaterial = increaseRenderer.material;
        decreaseMaterial = decreaseRenderer.material;
    }

    public void IncreaseFill()
    {
        eventFired = false;
        if (increaseRoutine != null) StopCoroutine(increaseRoutine);
        increaseRoutine = StartCoroutine(ChangeFill(increaseMaterial, maxFill, true));
    }

    public void DecreaseFill()
    {
        if (decreaseRoutine != null) StopCoroutine(decreaseRoutine);
        decreaseRoutine = StartCoroutine(ChangeFill(decreaseMaterial, minFill, false));
    }

    public void TransferLiquid()
    {
        eventFired = false;
        if (transferRoutine != null) StopCoroutine(transferRoutine);
        transferRoutine = StartCoroutine(TransferRoutine());
    }

    IEnumerator TransferRoutine()
    {
        float startIncrease = increaseMaterial.GetFloat("_FillHeight");
        float startDecrease = decreaseMaterial.GetFloat("_FillHeight");

        float incCurrent = startIncrease;
        float decCurrent = startDecrease;

        float elapsed = 0f;
        float maxDuration = Mathf.Max(increaseDuration, decreaseDuration);

        while (true)
        {
            if (useTime)
            {
                elapsed += Time.deltaTime;

                float incT = Mathf.Clamp01(elapsed / increaseDuration);
                float decT = Mathf.Clamp01(elapsed / decreaseDuration);

                incCurrent = Mathf.Lerp(startIncrease, maxFill, incT);
                decCurrent = Mathf.Lerp(startDecrease, minFill, decT);
            }
            else
            {
                incCurrent = Mathf.MoveTowards(incCurrent, maxFill, fillSpeed * Time.deltaTime);
                decCurrent = Mathf.MoveTowards(decCurrent, minFill, fillSpeed * Time.deltaTime);
            }

            increaseMaterial.SetFloat("_FillHeight", incCurrent);
            decreaseMaterial.SetFloat("_FillHeight", decCurrent);

            // 🔥 RELIABLE EVENT CHECK
            if (!eventFired && incCurrent >= maxFill - 0.001f)
            {
                eventFired = true;
                Debug.Log("EVENT: Increase reached max fill");
                OnIncreaseFillComplete?.Invoke();
            }

            if (Mathf.Abs(incCurrent - maxFill) < 0.001f &&
                Mathf.Abs(decCurrent - minFill) < 0.001f)
                break;

            yield return null;
        }

        increaseMaterial.SetFloat("_FillHeight", maxFill);
        decreaseMaterial.SetFloat("_FillHeight", minFill);
    }

    IEnumerator ChangeFill(Material mat, float target, bool isIncreaseSide)
    {
        float start = mat.GetFloat("_FillHeight");
        float current = start;
        float elapsed = 0f;

        while (Mathf.Abs(current - target) > 0.001f)
        {
            if (useTime)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / transferDuration);
                current = Mathf.Lerp(start, target, t);
            }
            else
            {
                current = Mathf.MoveTowards(current, target, fillSpeed * Time.deltaTime);
            }

            mat.SetFloat("_FillHeight", current);

            // 🔥 RELIABLE EVENT CHECK
            if (isIncreaseSide && !eventFired && current >= maxFill - 0.001f)
            {
                eventFired = true;
                Debug.Log("EVENT: Increase reached max fill");
                OnIncreaseFillComplete?.Invoke();
            }

            yield return null;
        }

        mat.SetFloat("_FillHeight", target);
    }
}
