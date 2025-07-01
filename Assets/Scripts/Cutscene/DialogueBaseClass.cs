using System.Collections;
using UnityEngine;
using UnityEngine.UI;


namespace DialogueSystem { 
public class DialogueBaseClass : MonoBehaviour
    {
        protected IEnumerator WriteText(string input, Text textHolder, Color textColor, Font textFont)
        {
            // Kiểm tra null
            if (textHolder == null || string.IsNullOrEmpty(input))
            {
                Debug.LogWarning("TextHolder is null or input is empty!");
                yield break;
            }

            // Reset text về rỗng trước khi bắt đầu
            textHolder.text = "";
            textHolder.color = textColor;
            textHolder.font = textFont;

            // Viết từng ký tự
            for (int i = 0; i < input.Length; i++)
            {
                textHolder.text += input[i];
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}