using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLibrary;
using GameLibrary.Tools;
using GameLibrary.Common;
using GameLibrary.Extension;
using GameLibrary.SocketServer;

public class Client : MonoBehaviour
{
    public ClientData Data;
    public static Client instance;
    private void Awake()
    {
        instance = this;
    }
}
