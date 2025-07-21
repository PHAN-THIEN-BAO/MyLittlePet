using UnityEngine;
using UnityEngine.UI;
namespace DialogueSystem
{
    public class DialogueLine : DialogueBaseClass
    {
        private Text textHolder;
        [Header ("Text Options")]
        [SerializeField] private string input;
        [SerializeField] private Color textColor;
        [SerializeField] private Font textFont;
        [Header("Time parameters")]
        [SerializeField] private float delay;
        private void Awake()
        {
            textHolder = GetComponent<Text>();
            if (textHolder == null)
            {
                Debug.LogError("No Text component found!");
                return;
            }
            if (string.IsNullOrEmpty(input))
            {
                Debug.LogWarning("Input text is empty!");
                return;
            }
            StartCoroutine(WriteText(input, textHolder, textColor, textFont));
        }
    }
}