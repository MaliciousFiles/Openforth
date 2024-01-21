using UnityEngine;

public class RunButton : Clickable
{
    public SpellMatController mat;

    private float origY;
    private bool clicked;
    private bool running;

    private void Start()
    {
        origY = gameObject.transform.position.y;
    }

    public void Click()
    {
        clicked = !clicked;

        Clickable.SetAllClickable(false);
    }

    private void OnMouseUpAsButton()
    {
        if (ClickEnabled) Click();
    }

    private float vel = 0;

    private void Update()
    {
        float dist = 0.4f;
        float buffer = 0.01f;

        Vector3 pos = gameObject.transform.localPosition;
        pos.y = Mathf.SmoothDamp(pos.y, origY - (clicked ? dist : 0), ref vel, 0.5f);
        gameObject.transform.localPosition = pos;

        if (!running && pos.y < origY - dist + buffer)
        {
            running = true;
            Clickable.SetAllClickable(false);
            mat.RunSpell(() => clicked = false);
        }
        else if (!clicked && pos.y > origY - buffer)
        {
            running = false;
            ClickEnabled = true;
        }
    }
}
