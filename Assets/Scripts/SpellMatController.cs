using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;
using Random = UnityEngine.Random;

public class SpellMatController : MonoBehaviour
{
    private const float SPACING = 0.14f; // just what it turns out to be
    
    public GameObject selectorPrefab;

    private new Collider collider;
    
    [CanBeNull] private GameObject selected, rune;
    private int placingRuneStage = -1, selectedIdx = -1;
    private KeyValuePair<Vector3, Quaternion> origRunePos;
    private Vector3 shakeEffect;
    
    private void Start()
    {
        collider = GetComponent<Collider>();
    }

    private void Update()
    {
        var sprites = selectorPrefab.transform.parent.GetComponentsInChildren<SpriteRenderer>();
        
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit);

        if (hit.collider == collider)
        {
            var x = transform.InverseTransformPoint(hit.point).x;
            
            for (var i = sprites.Length-1; i >= 0; i--)
            {
                var sprite = sprites[i];

                if (x < sprite.transform.localPosition.x)
                {
                    if (selected != sprite.gameObject)
                    {
                        if (selected) selected.GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0.12f);
                        
                        sprite.color += new Color(0, 0, 0.12f);
                        selected = sprite.gameObject;
                        selectedIdx = i;
                    }
                    break;
                }
            }
        }
        else
        {
            if (selected) selected.GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0.12f);
            selected = null;
        }

        switch (placingRuneStage)
        {
            case 0:
            {
                const float startingX = 0.41f;
                
                Vector3 targetPos = transform.TransformPoint(startingX - SPACING * selectedIdx, 1, 0);
                Quaternion targetRot = Quaternion.Euler(180, 0, 0);

                rune.transform.localPosition = Vector3.MoveTowards(rune.transform.localPosition, targetPos,
                    Vector3.Distance(origRunePos.Key, targetPos) * Time.deltaTime * 1.2f);
                rune.transform.localRotation = Quaternion.RotateTowards(rune.transform.localRotation, targetRot,
                    Quaternion.Angle(origRunePos.Value, targetRot) * Time.deltaTime * 1.2f);

                bool done = true;
                TextMeshProUGUI[] texts = selectorPrefab.transform.parent.GetComponentsInChildren<TextMeshProUGUI>();
                for (int i = selectedIdx; i < texts.Length; i++)
                {
                    Transform text = texts[i].transform;

                    Vector3 target = new(startingX - SPACING * (i+1), 0, 0);
                    
                    text.localPosition = Vector3.MoveTowards(text.localPosition, target,
                        SPACING * Time.deltaTime * 2f);

                    done &= text.localPosition == target;
                }
                
                if (done && rune.transform.localPosition == targetPos && rune.transform.localRotation == targetRot)
                {
                    placingRuneStage++;
                    origRunePos = KeyValuePair.Create(rune.transform.localPosition, rune.transform.localRotation);
                }

                break;
            }
            case 1:
            {
                var targetPos = origRunePos.Key - new Vector3(0, 0.33f, 0);
                var curPos = rune.transform.localPosition - shakeEffect;

                curPos = Vector3.MoveTowards(curPos, targetPos,
                    Vector3.Distance(origRunePos.Key, targetPos) * Time.deltaTime / 1.5f);
                rune.transform.localPosition = curPos;

                if (rune.transform.localPosition == targetPos)
                {
                    placingRuneStage = -1;
                    
                    Transform text = rune.transform.GetChild(0).GetChild(0);
                    text.SetParent(selectorPrefab.transform.parent);
                    text.SetSiblingIndex(selectedIdx + 1);
                    
                    rune.GetComponent<HandRune>().Destroy();
                    rune = null;
                    selectedIdx = -1;
                }
                else
                {
                    var range = 0.05f;
                    rune.transform.localPosition += shakeEffect = new Vector3(Random.Range(-range, range), 0, Random.Range(-range, range));
                }
                break;
            }
        }
    }

    private void OnMouseUpAsButton()
    {
        if (rune && placingRuneStage == -1)
        {
            placingRuneStage = 0;

            var script = rune.GetComponent<HandRune>();
            script.OverrideHoverColors = false;
            
            rune.transform.SetParent(null);

            RemoveSelectors(false);
        }
    }

    public void DrawSelectors(GameObject rune)
    {
        this.rune = rune;
        origRunePos = KeyValuePair.Create(rune.transform.localPosition, rune.transform.localRotation);

        var runes = selectorPrefab.transform.parent.GetComponentsInChildren<TextMeshProUGUI>();
        for (var i = 0; i <= runes.Length; i++)
        {
            var selector = Instantiate(selectorPrefab, selectorPrefab.transform.parent, true);

            selector.SetActive(true);
            selector.transform.localPosition -= new Vector3(i * SPACING, 0, 0);
        }
    }

    // fullReset: clear not only displaying the selectors but also various instance variables
    public void RemoveSelectors(bool fullReset = true)
    {
        selected = null;
        if (fullReset)
        {
            rune = null;
            placingRuneStage = selectedIdx = -1;
        }
        
        var sprites = selectorPrefab.transform.parent.GetComponentsInChildren<SpriteRenderer>();
        foreach (var sprite in sprites)
        {
            Destroy(sprite.gameObject);
        }
    }
}
