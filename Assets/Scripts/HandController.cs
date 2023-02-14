using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    private readonly Dictionary<int, KeyValuePair<Vector3, Quaternion>> origPositions = new();
    private readonly List<int> resetList = new(); // buffer for fixing werid bug detailed below (#LateUpdate)

    public void AddRune(GameObject rune)
    {
        rune.AddComponent<HandRune>();
        rune.transform.SetParent(transform);

        resetList.Add(rune.transform.GetInstanceID());
        origPositions[rune.transform.GetInstanceID()] = KeyValuePair.Create(rune.transform.localPosition, rune.transform.localRotation);
    }

    private void Update()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform t = transform.GetChild(i);

            Vector3 target = GetTargetPosition(i);
            Quaternion angleTarget = Quaternion.Euler(127.086f, 0, 0);
            float speed = 0.75f; // in seconds

            t.localPosition = Vector3.MoveTowards(t.localPosition, target, Vector3.Distance(origPositions[t.GetInstanceID()].Key, target) * Time.deltaTime / speed);
            t.localRotation = Quaternion.RotateTowards(t.localRotation, angleTarget, Quaternion.Angle(origPositions[t.GetInstanceID()].Value, angleTarget) * Time.deltaTime / speed);

            if (t.localPosition == target && t.localRotation == angleTarget) origPositions[t.GetInstanceID()] = KeyValuePair.Create(target, angleTarget);
        }
    }

    // fixes weird bug where local rotation is cleared in the same frame it is reparented
    private void LateUpdate()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform t = transform.GetChild(i);

            if (Quaternion.Angle(t.localRotation, Quaternion.identity) == 0 && resetList.Contains(t.GetInstanceID()))
            {
                resetList.Remove(t.GetInstanceID());
                t.localRotation = origPositions[t.GetInstanceID()].Value;
            }
        }
    }

    private Vector3 GetTargetPosition(int i)
    {
        // 1.6 = rune width (1.5) + 0.1 spacing
        return new((i - (transform.childCount - 1) / 2f) * 1.6f, 0);
    }
}
