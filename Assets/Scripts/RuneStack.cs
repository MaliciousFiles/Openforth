using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class RuneStack : Clickable
{
    public GameObject runeTypePicker;

    private float spacing;
    private float offset;

    private void Start()
    {
        RuneDecks.InitializeDecks();

        spacing = transform.GetChild(1).position.y - transform.GetChild(0).position.y;
        offset = transform.GetChild(0).localPosition.y;
    }

    private void OnMouseUpAsButton()
    {
        if (ClickEnabled)
        {
            Transform topRune = transform.GetChild(transform.childCount - 1);
            topRune.SetParent(null);

            Animator anim = topRune.GetComponent<Animator>();
            anim.enabled = true;
            anim.Play("Choose Rune");
            StartCoroutine(Util.OnAnimationFinish(topRune.gameObject, "Choose Rune", () =>
            {
                Destroy(topRune.gameObject);

                runeTypePicker.SetActive(true);
            }));

            Transform newRune = Instantiate(transform.GetChild(0));
            newRune.name = "Rune";
            newRune.SetParent(transform);
            newRune.SetSiblingIndex(0);
            newRune.localPosition = new(0, offset - spacing, 0);

            Clickable.SetAllClickable(false);
        }
    }

    private void Update()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);

            child.localPosition = Vector3.MoveTowards(child.localPosition, new(0, offset + i * spacing, 0), spacing * Time.deltaTime * 1.5f);
        }
    }
}