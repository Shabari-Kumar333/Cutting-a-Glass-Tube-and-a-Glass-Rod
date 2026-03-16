using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ModelClickManager : MonoBehaviour
{
    [System.Serializable]
    public class ModelData
    {
        public GameObject model;        // 3D model with collider
        public GameObject imageObject;  // UI Image GameObject (separate object)
        public bool isCorrect;          // true = correct model
    }

    public ModelData[] models;

    // ✅ Audio
    public AudioSource audioSource;
    public AudioClip correctAudio;
    public AudioClip wrongAudio;

    // ✅ One Event
    public UnityEvent onModelClick;

    void Start()
    {
        // Hide all images at start
        foreach (var m in models)
        {
            if (m.imageObject != null)
                m.imageObject.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject clicked = hit.collider.gameObject;
                Debug.Log("HIT: " + clicked.name);

                foreach (var m in models)
                {
                    if (clicked == m.model || clicked.transform.IsChildOf(m.model.transform))
                    {
                        // ✅ Enable image (never disable again)
                        if (m.imageObject != null && !m.imageObject.activeSelf)
                            m.imageObject.SetActive(true);

                        // ✅ Play audio
                        if (audioSource != null)
                        {
                            if (m.isCorrect && correctAudio != null)
                                audioSource.PlayOneShot(correctAudio);
                            else if (!m.isCorrect && wrongAudio != null)
                                audioSource.PlayOneShot(wrongAudio);
                        }

                        // ✅ Debug
                        if (m.isCorrect)
                            Debug.Log("✅ Correct Model Clicked");
                        else
                            Debug.Log("❌ Wrong Model Clicked");

                        // ✅ Event call
                        onModelClick.Invoke();

                        break;
                    }
                }
            }
        }
    }
}
