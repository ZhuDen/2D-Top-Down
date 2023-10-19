using UnityEngine;
using UnityEngine.Events;

public class SkillKeyboard : MonoBehaviour
{
    public delegate void ReplaceKey(KeyCode keyCode);
    public ReplaceKey OnReplaceKey;
   
    public void OnSetKey (KeyCode _keyCode)
    {
        OnReplaceKey?.Invoke(_keyCode);
    }
}
