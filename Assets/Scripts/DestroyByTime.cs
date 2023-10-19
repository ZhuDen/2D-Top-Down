using UnityEngine;

public class DestroyByTime : MonoBehaviour
{
    public float Time;

    void Start()
    {
        Destroy(gameObject, Time);
    }
    
}
