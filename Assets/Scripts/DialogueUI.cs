using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance;

    [SerializeField] private GameObject banner;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image leftPortrait;
    [SerializeField] private Image rightPortrait;

    [Header("Speaker Highlight")]
    [SerializeField] private Color activeColor = Color.white;
    [SerializeField] private Color inactiveColor = new Color(0.4f, 0.4f, 0.4f, 1f);

    void Awake()
    {
        Instance = this;
        banner.SetActive(false);
    }

    public void ShowLine(DialogueLine line)
    {
        banner.SetActive(true);
        dialogueText.text = line.text;

        // Highlight active speaker, dim inactive
        if (line.speaker == PlayerSide.Left)
        {
            leftPortrait.color = activeColor;
            rightPortrait.color = inactiveColor;
        }
        else
        {
            leftPortrait.color = inactiveColor;
            rightPortrait.color = activeColor;
        }
    }

    public void Hide()
    {
        banner.SetActive(false);
    }
}