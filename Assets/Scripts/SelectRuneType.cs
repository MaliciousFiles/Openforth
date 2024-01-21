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
                GameController.CurrentPlayer.PickRuneType(type);
            }
            else
            {
                GameController.CurrentPlayer.MoveRuneToHand(type);
            }
        }
    }

    public void ChooseRune()
    {
        anim.enabled = true;
        anim.Play("Rune Spin");
        StartCoroutine(Util.OnAnimationFinish(gameObject, "Rune Spin", () => { ClickEnabled = true; anim.enabled = false; }));

        other.fadeOut = true;

        // TODO: DEBUGGING
        rune = RuneDecks.Draw(type);
        if (type == RuneType.Symbol)
        {
            int r = -1;
            if (Input.GetKey(KeyCode.Alpha0)) r = 0;
            else if (Input.GetKey(KeyCode.Alpha1)) r = 1;
            else if (Input.GetKey(KeyCode.Alpha2)) r = 2;
            else if (Input.GetKey(KeyCode.Alpha3)) r = 3;
            else if (Input.GetKey(KeyCode.Alpha4)) r = 4;
            else if (Input.GetKey(KeyCode.Alpha5)) r = 5;
            else if (Input.GetKey(KeyCode.Alpha6)) r = 6;
            else if (Input.GetKey(KeyCode.Alpha7)) r = 7;
            else if (Input.GetKey(KeyCode.Alpha8)) r = 8;
            else if (Input.GetKey(KeyCode.Alpha9)) r = 9;

            if (r > -1) rune = new SymbolRune((RuneSymbol)r, "S" + r);
        }

        Clickable.SetAllClickable(false);
    }

    public void MoveToHand(PlayerController player)
    {
        Transform newRune = Instantiate(transform);
        newRune.name = "Rune";
        newRune.SetParent(transform.parent, false);
        Destroy(newRune.GetComponent<Animator>());
        Destroy(newRune.GetComponent<SelectRuneType>());
        player.hand.GetComponent<HandController>().AddRune(new RuneObject(newRune.gameObject, rune));

        transform.parent.gameObject.SetActive(false);
        Clickable.SetAllClickable(false);

        Reset();
        other.Reset();
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
            text.fontSize = type == RuneType.Number ? 0.9f : 0.42f;
            text.transform.parent.localPosition = new(0, -0.91f, 0);
        }

        if (fadeOut)
        {
            mat.color = new(mat.color.r, mat.color.g, mat.color.b, Mathf.Clamp01(mat.color.a - Time.deltaTime * 1.5f));
        }
    }
}
