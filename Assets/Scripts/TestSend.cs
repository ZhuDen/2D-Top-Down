using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLibrary;
using GameLibrary.Tools;
using GameLibrary.Common;

public class TestSend : MonoBehaviour
{
    public static TestSend Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            SendMess();
        }
    }

    async void SendMess()
    {
        await TransportHandler.Transport.SendTo(new DataPacket((byte)OperationCode.Message, new Dictionary<ParameterCode, object> { { ParameterCode.Message, "Test: " } }));
    }
}
