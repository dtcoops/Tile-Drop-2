using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueSequence sequence;
    [SerializeField] private bool fireOnce = true;
    private bool fired;

    private void OnTriggerEnter(Collider other)
    {
        if (fired && fireOnce) return;
        if (!other.CompareTag("Player")) return;

        fired = true;
        DialogueManager.Instance.PlaySequence(sequence);
    }
}