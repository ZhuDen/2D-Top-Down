using GameLibrary.Extension;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public static Manager Instance { get; set; }

    public GameObject Client;

    private void Awake()
    {
        Instance = this;
        CreatePlayer();
    }

    public void CreatePlayer()
    {

        Client = GameObject.Instantiate(Client);
        

    }
}
