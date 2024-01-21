using UnityEngine;
using System.Collections.Generic;

public class HandRune : Clickable
{
    public GameObject spellMat;
    public RuneObject rune;
    private SpellMatController spellMatController;

    private Vector3 origScale;
    private Vector3 origPos;
    private Quaternion origRot;

    private Vector3 targetScale;
    private Vector3 posTransform;
    private Vector3 rotTransform; // in euler angles

    private bool focused;
    private bool inMotion;
    private bool destroy;

    private void Start()
    {
        spellMatController = spellMat.GetComponent<SpellMatController>();
        origScale = transform.localScale;
        origPos = GameController.CurrentPlayer.transform.localPosition;
        origRot = GameController.CurrentPlayer.transform.localRotation;

        targetScale = Vector3.Scale(origScale, new(1.5f, 1, 1.5f));

        posTransform = spellMat.transform.position + new Vector3(0.5f, 8, -2.8f) - origPos;
        rotTransform = new Vector3(20, 0, 0) - origRot.eulerAngles;
    }

    private void OnMouseUpAsButton()
    {
        if (ClickEnabled && transform.parent.parent == GameController.CurrentPlayer.transform)
        {
            GameController.CurrentPlayer.SelectHandRune(transform.GetSiblingIndex());
        }
    }

    public void SelectToPlay()
    {
        SetAllClickable(false);
        ClickEnabled = true;

        focused = !focused;
        inMotion = true;

        OverrideHoverColors = focused ? true : null;
    }

    public void Destroy()
    {
        focused = false;
        inMotion = true;
        destroy = true;
    }

    private void Update()
    {
        Transform player = GameController.CurrentPlayer.transform;

        Vector3 targetPos = origPos + (focused ? posTransform : new());
        Quaternion targetRot = Quaternion.Euler(origRot.eulerAngles + (focused ? rotTransform : new()));

        if (inMotion)
        {
            player.localPosition = Vector3.MoveTowards(player.localPosition, targetPos, posTransform.magnitude * Time.deltaTime);
            player.localRotation = Quaternion.RotateTowards(player.localRotation, targetRot, rotTransform.magnitude * Time.deltaTime);

            transform.localScale = Vector3.MoveTowards(transform.localScale, focused ? targetScale : origScale, Vector3.Distance(origScale, targetScale) * Time.deltaTime * 4);

            const float zMod = 0.32f; // how far to move forward to still be able to see the rune in the game
            const float yMod = 0.06f; // how far to move upward to put this rune above the others
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, new(transform.localPosition.x, focused ? yMod : 0, focused ? zMod : 0), zMod * Time.deltaTime * 4);
        }

        if (inMotion && (focused ? targetPos : origPos) == player.localPosition &&
            (focused ? targetRot : origRot) == player.localRotation)
        {
            SetAllClickable(false);
            ClickEnabled = focused;

            if (destroy)
            {
                Destroy(gameObject);
                return;
            }

            inMotion = false;

            if (focused)
            {
                spellMatController.DrawSelectors(rune);
            }
            else
            {
                spellMatController.RemoveSelectors();
            }
        }
    }
}
