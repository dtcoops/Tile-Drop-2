using System.Collections;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    
    private static DialogueManager _instance;

    public static DialogueManager Instance
    {get
        {
            if (_instance == null)
            {
                var go = new GameObject("_DialogueManager");
                _instance = go.AddComponent<DialogueManager>();
            }
            return _instance;
        }
    }

   void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySequence(DialogueSequence sequence)
    {
        StartCoroutine(PlayRoutine(sequence));
    }

    IEnumerator PlayRoutine(DialogueSequence sequence)
    {
        foreach (var line in sequence.lines)
        {
            DialogueUI.Instance.ShowLine(line);
            yield return new WaitForSeconds(line.displayDuration);
        }
        DialogueUI.Instance.Hide();
    }
}