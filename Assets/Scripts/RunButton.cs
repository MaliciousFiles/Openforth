using UnityEngine;

public class RunButton : Clickable
{
    private float origY;

    private void Start()
    {
        origY = gameObject.transform.position.y;
    }

    private float vel = 0;
    private Color oldColor = Color.clear;

    private void Update()
    {
        Material mat = GetComponent<MeshRenderer>().material;

        Vector3 pos = gameObject.transform.localPosition;
        pos.y = Mathf.SmoothDamp(pos.y, origY - (MouseOver && Input.GetMouseButton(0) ? 0.4f : 0), ref vel, 0.5f);
        gameObject.transform.localPosition = pos;

        if (pos.y < origY - 0.25)
        {

            if (oldColor == Color.clear) oldColor = mat.color;
            mat.color = Color.green;

            if (MouseOver && Input.GetMouseButtonUp(0))
            {
                Debug.Log("clicked!");
            }
        }
        else
        {
            if (oldColor != Color.clear) mat.color = oldColor;
            oldColor = Color.clear;
        }
    }
}
