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

        if (clickEnabled) ModifyColors();
    }

    private void OnMouseExit()
    {
        mouseOver = false;

        if (clickEnabled) ResetColors();
    }

    private void ModifyColors()
    {
        foreach (Material mat in GetMaterials())
        {
            mat.color = Color.Lerp(mat.color, Color.white, 0.15f);
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
