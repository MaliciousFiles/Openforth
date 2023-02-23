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
    
    public GameObject selectorPrefab, targetsPrefab;

    private new Collider collider;
    
    [CanBeNull] private GameObject selected;
    [CanBeNull] private RuneObject rune;
    private int placingRuneStage = -1, selectedIdx = -1;
    private KeyValuePair<Vector3, Quaternion> origRunePos;
    private Vector3 shakeEffect;

    private readonly List<RuneObject> runeList = new();
    
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

        if (placingRuneStage > -1)
        {
            Transform t = rune.gameObject.transform;
            switch (placingRuneStage)
            {
                case 0:
                {
                    const float startingX = 0.41f;

                    Vector3 targetPos = transform.TransformPoint(startingX - SPACING * selectedIdx, 1, 0);
                    Quaternion targetRot = Quaternion.Euler(180, 0, 0);

                    t.localPosition = Vector3.MoveTowards(t.localPosition, targetPos,
                        Vector3.Distance(origRunePos.Key, targetPos) * Time.deltaTime * 1.2f);
                    t.localRotation = Quaternion.RotateTowards(t.localRotation, targetRot,
                        Quaternion.Angle(origRunePos.Value, targetRot) * Time.deltaTime * 1.2f);

                    bool done = true;
                    TextMeshProUGUI[] texts =
                        selectorPrefab.transform.parent.GetComponentsInChildren<TextMeshProUGUI>();
                    for (int i = selectedIdx; i < texts.Length; i++)
                    {
                        Transform text = texts[i].transform;

                        Vector3 target = new(startingX - SPACING * (i + 1), 0, 0);

                        text.localPosition = Vector3.MoveTowards(text.localPosition, target,
                            SPACING * Time.deltaTime * 2f);

                        done &= text.localPosition == target;
                    }

                    if (done && t.localPosition == targetPos && t.localRotation == targetRot)
                    {
                        placingRuneStage++;
                        origRunePos = KeyValuePair.Create(t.localPosition, t.localRotation);
                    }

                    break;
                }
                case 1:
                {
                    var targetPos = origRunePos.Key - new Vector3(0, 0.33f, 0);
                    var curPos = t.localPosition - shakeEffect;

                    curPos = Vector3.MoveTowards(curPos, targetPos,
                        Vector3.Distance(origRunePos.Key, targetPos) * Time.deltaTime / 1.5f);
                    t.localPosition = curPos;

                    if (t.localPosition == targetPos)
                    {
                        rune.gameObject.GetComponent<HandRune>().Destroy();

                        rune.gameObject = rune.gameObject.GetComponentInChildren<TextMeshProUGUI>().gameObject;
                        Transform text = rune.gameObject.transform;
                        text.SetParent(selectorPrefab.transform.parent);
                        text.SetSiblingIndex(selectedIdx + 1);

                        CalculateTargetBoxes();
                        
                        placingRuneStage = -1;
                        rune = null;
                        selectedIdx = -1;
                    }
                    else
                    {
                        var range = 0.05f;
                        t.localPosition += shakeEffect =
                            new Vector3(Random.Range(-range, range), 0, Random.Range(-range, range));
                    }

                    break;
                }
            }
        }
    }

    private void OnMouseUpAsButton()
    {
        if (rune != null && placingRuneStage == -1)
        {
            Clickable.SetAllClickable(false);
            
            runeList.Insert(selectedIdx, rune);
            placingRuneStage = 0;

            var script = rune.gameObject.GetComponent<HandRune>();
            script.OverrideHoverColors = false;
            
            rune.gameObject.transform.SetParent(null);
            
            RemoveSelectors(false);
        }
    }

    public void DrawSelectors(RuneObject rune)
    {
        this.rune = rune;
        origRunePos = KeyValuePair.Create(rune.gameObject.transform.localPosition, rune.gameObject.transform.localRotation);

        var runes = selectorPrefab.transform.parent.GetComponentsInChildren<TextMeshProUGUI>();
        for (var i = 0; i <= runes.Length; i++)
        {
            var selector = Instantiate(selectorPrefab, selectorPrefab.transform.parent);

            selector.SetActive(true);
            selector.transform.localPosition -= new Vector3(i * SPACING, 0, 0);
        }

        SpriteRenderer[] sprites = targetsPrefab.transform.parent.GetComponentsInChildren<SpriteRenderer>();
        foreach (var s in sprites)
        {
            Destroy(s.gameObject);
        }
    }

    // fullReset: reset everything, not just the selector bars
    public void RemoveSelectors(bool fullReset = true)
    {
        selected = null;
        if (fullReset)
        {
            rune = null;
            placingRuneStage = selectedIdx = -1;

            CalculateTargetBoxes();
        }
        
        var sprites = selectorPrefab.transform.parent.GetComponentsInChildren<SpriteRenderer>();
        foreach (var sprite in sprites)
        {
            Destroy(sprite.gameObject);
        }
    }

    private void CalculateTargetBoxes()
    {
        Stack<RuneObject> stack = new();
        Dictionary<RuneObject, int> targets = new();
        Dictionary<RuneObject, SpriteRenderer> squares = new();
        
        for (int i = 0; i < runeList.Count; i++)
        {
            RuneObject rune = runeList[i];
            
            stack.Push(rune);
            
            if (rune.rune.GetType() == typeof(SymbolRune))
            {
                int numTargets = ((SymbolRune)rune.rune).GetTargets(stack);

                for (int j = 1; j <= numTargets; j++)
                {
                    numTargets += targets.GetValueOrDefault(runeList[i - j], 0);
                }

                for (int j = 1; j <= numTargets; j++)
                {
                    if (squares.TryGetValue(runeList[i - j], out var sprite))
                    {
                        sprite.size -= new Vector2(0.2f, 0.2f);
                    }
                }
                
                targets[rune] = numTargets;
                
                SpriteRenderer square = Instantiate(targetsPrefab, targetsPrefab.transform.parent).GetComponent<SpriteRenderer>();
                square.transform.localPosition = new(-SPACING * i, 0, 0);
                square.gameObject.SetActive(true);
                squares[rune] = square;

                if (numTargets == -1)
                {
                    square.color = Color.red;
                    break;
                }
                
                square.size = new(numTargets + 1, 1);
                square.transform.localPosition += new Vector3(numTargets * SPACING / 2, 0, 0);
            }
        }
    }
}
