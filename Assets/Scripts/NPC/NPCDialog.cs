using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "NPCDialog", menuName ="NPC dialog")]
public class NPCDialog : ScriptableObject
{
    public string npcName;
    public Sprite npcPortrait;
    public string[] dialogLines;
    public float typingSpeed = 0.05f;
    public AudioClip voiceSound;
    public float voicePitch = 1.0f;

    public bool[] autoProgressLines;
    public float autoProgressDelay = 1.5f;
} 
