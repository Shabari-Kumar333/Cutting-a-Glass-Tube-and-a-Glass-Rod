using UnityEngine;
using UnityEngine.Events;

public class RaycastClickEvent : MonoBehaviour
{
    [Header("Target To Detect")]
    public Transform targetObject;

    [Header("Event To Trigger")]
    public UnityEvent onTargetClicked;

    [Header("Particle Object (GameObject)")]
    public GameObject particleObject;

    [Header("Raycast Settings")]
    public float rayDistance = 100f;
    public LayerMask rayLayerMask = ~0;

    // 🌊 Water Control (LERP TO TARGET)
    [Header("Water Height Control")]
    public Renderer waterRenderer;
    public float targetFillHeight;
    public float lerpSpeed = 2f;

    // 🎬 Animation Control
    [Header("Animation Settings")]
    public Animator animator;                 // Assign Animator
    public string animationStateName;         // Exact state name in Animator
    public UnityEvent onAnimationComplete;    // Event fired after animation

    private Material waterMat;
    private int fillID;
    private bool animateWater = false;
    private bool waitingForAnimEnd = false;

    void Start()
    {
        if (waterRenderer != null)
        {
            waterMat = waterRenderer.material;
            fillID = Shader.PropertyToID("_FillHeight");
        }
    }

    void Update()
    {
        HandleRaycastClick();
        AnimateWater();
        CheckAnimationFinished();
    }

    void HandleRaycastClick()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        if (Camera.main == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance, rayLayerMask))
        {  
            if (hit.transform == targetObject)
            {
                onTargetClicked?.Invoke();
            }
        }
    }

    void AnimateWater()
    {
        if (!animateWater || waterMat == null) return;

        float current = waterMat.GetFloat(fillID);
        float newHeight = Mathf.Lerp(current, targetFillHeight, Time.deltaTime * lerpSpeed);
        waterMat.SetFloat(fillID, newHeight);

        if (Mathf.Abs(newHeight - targetFillHeight) < 0.001f)
        {
            waterMat.SetFloat(fillID, targetFillHeight);
            animateWater = false;
        }
    }

    void CheckAnimationFinished()
    {
        if (!waitingForAnimEnd || animator == null) return;

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        if (state.IsName(animationStateName) && state.normalizedTime >= 1f)
        {
            waitingForAnimEnd = false;
            onAnimationComplete?.Invoke();
        }
    }

    // 🌊 CALL THIS FROM UNITY EVENT
    public void LerpToTargetHeight()
    {
        if (waterMat == null) return;
        animateWater = true;
    }

    // 🎬 PLAY ANIMATION FROM EVENT
    public void PlayAnimation()
    {
        if (animator == null || string.IsNullOrEmpty(animationStateName)) return;

        animator.Play(animationStateName, 0, 0f);
        waitingForAnimEnd = true;
    }

    public void SetWaterHeightInstant(float height)
    {
        if (waterMat == null) return;
        waterMat.SetFloat(fillID, height);
    }

    // ✨ Particle Controls
    public void OnParticle()
    {
        if (particleObject != null)
            particleObject.SetActive(true);
    }

    public void OffParticle()
    {
        if (particleObject != null)
            particleObject.SetActive(false);
    }
    public void AnimationFinished()
{
    waitingForAnimEnd = false;
    onAnimationComplete?.Invoke();
}

}
