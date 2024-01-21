using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject runeTypePicker, hand, namePlate, table;
    public RuneStack runeStack;
    public string playerName;

    public void Start()
    {
        TransformNamePlate();

        foreach (TextMeshProUGUI text in namePlate.GetComponentsInChildren<TextMeshProUGUI>()) text.text = playerName;
    }

    public void TransformNamePlate()
    {
        Vector3 pos = namePlate.transform.localPosition;

        pos.z = Mathf.Abs((transform.rotation.eulerAngles.y + table.transform.rotation.eulerAngles.y) / 90 % 2) == 1 ? 4.39f : 7.2f;

        namePlate.transform.localPosition = pos;
    }

    public void Draw()
    {
        runeStack.Draw(this);
    }

    public void PickRuneType(RuneType type)
    {
        runeTypePicker.transform.GetChild((int)type).GetComponent<SelectRuneType>().ChooseRune();
    }

    public void MoveRuneToHand(RuneType type)
    {
        runeTypePicker.transform.GetChild((int)type).GetComponent<SelectRuneType>().MoveToHand(this);
    }

    public void SelectHandRune(int idx)
    {
        hand.transform.GetChild(idx).GetComponent<HandRune>().SelectToPlay();
    }
}
