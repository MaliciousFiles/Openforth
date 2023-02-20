using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class Clickable : MonoBehaviour
{
    private static readonly List<Clickable> instances = new();

    private bool clickEnabled = true;
    public bool ClickEnabled
    {
        get { return clickEnabled; }
        set
        {
            if (value && mouseOver) { if (!clickEnabled) ModifyColors(); }
            else ResetColors();

            clickEnabled = value;
        }
    }

    private bool mouseOver;
    public bool MouseOver
    {
        get { return mouseOver && clickEnabled; }
    }

    // if given a non-null value, it will override the colors to either be always selected or deselected
    private bool? overrideHoverColors;
    public bool? OverrideHoverColors
    {
        get { return overrideHoverColors; }
        set
        {
            if (value ?? !MouseOver) ModifyColors();
            else ResetColors();

            overrideHoverColors = value;
        }
    }

    private Color[] origColors;

    public static void SetAllClickable(bool enabled)
    {
        foreach (Clickable obj in instances)
        {
            obj.ClickEnabled = enabled;
        }
    }

    private void OnDestroy()
    {
        instances.Remove(this);
    }

    private void Awake()
    {
        instances.Add(this);

        origColors = GetMaterials().Select(mat => mat.color).ToArray();
    }

    private void OnMouseEnter()
    {
        mouseOver = true;

        if (clickEnabled && overrideHoverColors == null) ModifyColors();
    }

    private void OnMouseExit()
    {
        mouseOver = false;

        if (clickEnabled && overrideHoverColors == null) ResetColors();
    }

    private void ModifyColors()
    {
        Material[] mats = GetMaterials();
        for (int i = 0; i < mats.Length; i++)
        {
            mats[i].color = Color.Lerp(origColors[i], Color.white, 0.15f);
        }
    }

    private void ResetColors()
    {
        Material[] mats = GetMaterials();
        for (int i = 0; i < mats.Length; i++)
        {

            mats[i].color = origColors[i];
        }
    }

    private Material[] GetMaterials()
    {
        return GetComponents<MeshRenderer>().Concat(GetComponentsInChildren<MeshRenderer>()).Select(r => r.materials).SelectMany(i => i).ToArray();
    }
}
