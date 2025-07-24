using System.Collections;
using UnityEngine;
using UnityEngine.UI;


namespace DialogueSystem { 
public class DialogueBaseClass : MonoBehaviour
    {
        protected IEnumerator WriteText(string input, Text textHolder, Color textColor, Font textFont)
        {
            if (textHolder == null || string.IsNullOrEmpty(input))
            {
                Debug.LogWarning("TextHolder is null or input is empty!");
                yield break;
            }

            textHolder.text = "";
            textHolder.color = textColor;
            textHolder.font = textFont;

            for (int i = 0; i < input.Length; i++)
            {
                textHolder.text += input[i];
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}