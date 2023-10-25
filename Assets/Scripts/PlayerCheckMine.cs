using UnityEngine;

public class PlayerCheckMine : MonoBehaviour
{
    public string MyID;

    private void Awake()
    {
        
    }

    public bool IsMine ()
    {
        return MyID == PlayerPrefs.GetString("MyCurrentID");
    }
}
