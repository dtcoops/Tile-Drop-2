using UnityEngine;

[CreateAssetMenu(fileName = "DialogueSequence", menuName = "TileDrop/DialogueSequence")]
public class DialogueSequence : ScriptableObject
{
    public DialogueLine[] lines;
}