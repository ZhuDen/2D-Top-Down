using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLibrary;
using GameLibrary.Tools;
using GameLibrary.Common;

public class SendScript : MonoBehaviour
{
    bool check = false;
    public int Out = 0;
    public int Return = 0;
    public int PerSec = 0;
    private float _time = 0;
    public string time = "";
    private float upt = 0;
    public string Uptime = "";
    int bufferindex = 0;
    float uptmax = 0;
    float j = 0;
    float k = 0;


    public static SendScript Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    async void SendMess()
    {
        Out++;
        //Debug.Log("Hello: " + i++);
        await TransportHandler.Transport.SendTo(new DataPacket(OperationCode.Message, new Dictionary<ParameterCode, object> { { ParameterCode.Message, "Hello: " } }));
    }


    private void FixedUpdate()
    {
        if (check && _time < 60.0f)
        {

            _time += Time.deltaTime;
            PerSec = Return / (int)_time;
            if (Return >= bufferindex)
            {
                
                j = _time;

                bufferindex = Out;
            }

        }
    }

    private void Update()
    {
        if (check && _time < 60.0f)
        {

            k = _time - j;
            time = (_time).ToString("0 Сек.");
            Uptime = k.ToString("0.######## ms");

            SendMess();


        }

        //if (Input.GetKeyDown(KeyCode.D)) { check = !check; _time = 0;  Out = 0; Return = 0; }
        /*if (Input.GetKeyDown(KeyCode.G))
        {
            TransportHandler.Transport.SendTo(new DataPacket(OperationCode.SetTeam, new Dictionary<ParameterCode, object> { { ParameterCode.Message, "SetTeam" } }, SendClientFlag.Me));
            //TransportHandler.Transport.SendTo(new DataPacket(OperationCode.GetAllRoom, new Dictionary<ParameterCode, object> { { ParameterCode.Message, "Update"}}, SendClientFlag.Me));
        }*/

            //if (Input.GetKeyDown(KeyCode.D)) { SendMess(); }
        }

}
