using System.Xml;
using UnityEngine;

public class RuneObject
{
    public const RuneObject SYNTAX_ERROR = null;
    
    public GameObject gameObject;
    public Rune rune;
    
    public RuneObject(GameObject gameObject, Rune rune)
    {
        this.gameObject = gameObject;
        this.rune = rune;
    }
}