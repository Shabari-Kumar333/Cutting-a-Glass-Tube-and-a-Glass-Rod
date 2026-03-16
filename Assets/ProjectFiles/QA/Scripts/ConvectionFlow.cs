using UnityEngine;
using System.Collections.Generic;

public class RandomFloatInBox : MonoBehaviour
{
    public GameObject floatPrefab;
    public int count = 10;
    public float speed = 0.3f;
    public float directionChangeTime = 2f;

    private List<Transform> floaters = new List<Transform>();
    private List<Vector3> directions = new List<Vector3>();
    private BoxCollider box;

    void Start()
    {
        box = GetComponent<BoxCollider>();

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = GetRandomPointInBox();
            GameObject obj = Instantiate(floatPrefab, pos, Quaternion.identity, transform);
            floaters.Add(obj.transform);
            directions.Add(Random.onUnitSphere);
        }

        InvokeRepeating(nameof(ChangeDirections), directionChangeTime, directionChangeTime);
    }

    void Update()
    {
        for (int i = 0; i < floaters.Count; i++)
        {
            Transform t = floaters[i];
            t.position += directions[i] * speed * Time.deltaTime;

            Vector3 localPos = transform.InverseTransformPoint(t.position);
            Vector3 half = box.size * 0.5f;

            Vector3 dir = directions[i]; // copy

            if (Mathf.Abs(localPos.x) > half.x) dir.x *= -1;
            if (Mathf.Abs(localPos.y) > half.y) dir.y *= -1;
            if (Mathf.Abs(localPos.z) > half.z) dir.z *= -1;

            directions[i] = dir; // assign back
        }
    }

    void ChangeDirections()
    {
        for (int i = 0; i < directions.Count; i++)
            directions[i] = Random.onUnitSphere;
    }

    Vector3 GetRandomPointInBox()
    {
        Vector3 center = box.center;
        Vector3 size = box.size;

        Vector3 randomLocal = new Vector3(
            Random.Range(-size.x / 2, size.x / 2),
            Random.Range(-size.y / 2, size.y / 2),
            Random.Range(-size.z / 2, size.z / 2)
        );

        return transform.TransformPoint(center + randomLocal);
    }
}
