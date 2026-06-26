using UnityEngine;


[System.Serializable]
public class DialogueLine
{
    public PlayerSide speaker;
    [TextArea] public string text;
    public float displayDuration = 3f;
}