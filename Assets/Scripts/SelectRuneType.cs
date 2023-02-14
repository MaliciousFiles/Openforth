using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SelectRuneType : Clickable
{
    public RuneType type;

    private SelectRuneType other;
    private Animator anim;
    private TextMeshProUGUI text;
    private Rune rune;
    private Material mat;

    private bool fadeOut = false;
    private bool movementComplete = false;
    private bool baseText = true;

    private void Start()
    {
        ClickEnabled = false;

        mat = GetComponent<MeshRenderer>().material;
        other = Array.Find(transform.parent.GetComponentsInChildren<SelectRuneType>(), r => r != this);
        anim = GetComponent<Animator>();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void OnMouseUpAsButton()
    {
        if (ClickEnabled)
        {
            if (baseText)
            {
                anim.enabled = true;
                anim.Play("Rune Spin");
                StartCoroutine(Util.OnAnimationFinish(gameObject, "Rune Spin", () => { ClickEnabled = true; anim.enabled = false; }));

                other.fadeOut = true;

                rune = RuneDecks.Draw(type);

                Clickable.SetAllClickable(false);
            }
            else
            {
                Transform newRune = Instantiate(transform);
                newRune.name = "Rune";
                newRune.SetParent(transform.parent, false);
                Destroy(newRune.GetComponent<Animator>());
                Destroy(newRune.GetComponent<SelectRuneType>());
                GameObject.Find("Hand").GetComponent<HandController>().AddRune(newRune.gameObject);

                transform.parent.gameObject.SetActive(false);
                Clickable.SetAllClickable(true);

                Reset();
                other.Reset();
            }
        }
    }

    private void Reset()
    {
        mat.color = other.mat.color;

        fadeOut = false;

        movementComplete = false;
        baseText = true;
        text.text = type == RuneType.Number ? "#" : "‡";
        text.fontSize = 1;
        text.color = new(text.color.r, text.color.g, text.color.b, 0);

        transform.localPosition = Vector3.zero;
        text.transform.parent.localPosition = type == RuneType.Number ? new(0, -0.91f, 0) : new(0, -0.99f, -0.023f);
    }

    private void Update()
    {
        bool active = transform.parent.gameObject.activeSelf;

        if (!movementComplete)
        {
            Vector3 target = new(1.6f * (type == RuneType.Number ? -1 : 1), 0);

            transform.localPosition = active ?
                Vector3.MoveTowards(transform.localPosition, target, 1.6f * Time.deltaTime / 1.2f) :
                Vector3.zero;

            if (transform.localPosition == target)
            {
                ClickEnabled = true;
                movementComplete = true;
            }
        }

        float alphaMod = Time.deltaTime * (fadeOut ? -1.5f : !anim.enabled ? 1.2f : baseText ? -1.1f : 1.1f);
        text.color = new(text.color.r, text.color.g, text.color.b, active ? Mathf.Clamp01(text.color.a + alphaMod) : 0);

        if (anim.enabled && baseText && text.color.a == 0)
        {
            baseText = false;
            text.text = rune.display;
            text.fontSize = 0.9f;
            text.transform.parent.localPosition = new(0, -0.91f, 0);
        }

        if (fadeOut)
        {
            mat.color = new(mat.color.r, mat.color.g, mat.color.b, Mathf.Clamp01(mat.color.a - Time.deltaTime * 1.5f));
        }
    }
}
