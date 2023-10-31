using UnityEngine;

public class IsMinePlayer : MonoBehaviour
{
    public string ID;

    public bool IsMine()
    {
        return ID == TransportHandler.Transport.Id;
    }
}
