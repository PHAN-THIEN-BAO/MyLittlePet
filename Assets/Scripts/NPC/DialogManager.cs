using UnityEngine;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance;

    public bool hasTalkedToBob = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}