using UnityEngine;


[CreateAssetMenu(fileName = "NewNPCDialogue", menuName = "NPC Dialogue")]
public class NPCDialogue : ScriptableObject
{
    public string npcName;
    public Sprite npcPortrait;
    public string[] dialogueLines;
    public bool[] autoProgressLines;
    public bool[] endDialogueLines;
    public float autoProgressDelay = 2.0f;
    public float typingSpeed = 0.05f;
    //public AudioClip voiceSound; 
    //public float voicepitch = 1.0f;
    
    public DialogueChoice[] choices;
}
[System.Serializable]
public class DialogueChoice
{
    public int dialogueIndex;
    public string[] choices;
    public int[] nextDialogueIndexes;
    public DialogueEffect[] choiceEffects;
}