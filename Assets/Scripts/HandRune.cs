using UnityEngine;
using System.Collections.Generic;

public class HandRune : Clickable
{
    public GameObject spellMat;
    private SpellMatController spellMatController;

    private Vector3 origScale;
    private Vector3 origPos;
    private Quaternion origRot;

    private Vector3 targetScale;
    private Vector3 targetPos;
    private Quaternion targetRot;

    private bool focused;
    private bool inMotion;
    private bool destroy;

    private void Start()
    {
        spellMatController = spellMat.GetComponent<SpellMatController>();
        origScale = transform.localScale;
        origPos = Camera.main.transform.localPosition;
        origRot = Camera.main.transform.localRotation;

        targetScale = Vector3.Scale(origScale, new(1.5f, 1, 1.5f));
        targetPos = spellMat.transform.position + new Vector3(0.5f, 8, -2.8f);
        targetRot = Quaternion.Euler(80, 0, 0);
    }

    private void OnMouseUpAsButton()
    {
        if (ClickEnabled)
        {
            SetAllClickable(false);
            ClickEnabled = true;
            
            focused = !focused;
            inMotion = true;

            OverrideHoverColors = focused ? true : null;
        }
    }

    public void Destroy()
    {
        focused = false;
        inMotion = true;
        destroy = true;
    }

    private void Update()
    {
        Transform camera = Camera.main.transform;

        if (inMotion)
        {
            camera.localPosition = Vector3.MoveTowards(camera.localPosition, focused ? targetPos : origPos, Vector3.Distance(origPos, targetPos) * Time.deltaTime);
            camera.localRotation = Quaternion.RotateTowards(camera.localRotation, focused ? targetRot : origRot, Quaternion.Angle(origRot, targetRot) * Time.deltaTime);

            transform.localScale = Vector3.MoveTowards(transform.localScale, focused ? targetScale : origScale, Vector3.Distance(origScale, targetScale) * Time.deltaTime * 4);
            
            const float zMod = 0.32f; // how far to move forward to still be able to see the rune in the game
            const float yMod = 0.06f; // how far to move upward to put this rune above the others
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, new(transform.localPosition.x, focused ? yMod : 0, focused ? zMod : 0), zMod * Time.deltaTime * 4);
        }

        if (inMotion && (focused ? targetPos : origPos) == camera.localPosition &&
            (focused ? targetRot : origRot) == camera.localRotation)
        {
            SetAllClickable(!focused);
            ClickEnabled = true;
            
            if (destroy)
            {
                Destroy(gameObject);
                return;
            }
            
            inMotion = false;
            
            if (focused)
            {
                spellMatController.DrawSelectors(gameObject);
            }
            else
            {
                spellMatController.RemoveSelectors();
            }
        }
    }
}
