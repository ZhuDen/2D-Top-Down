using GameLibrary.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynchronizeTransfrom : MonoBehaviour
{
    
    public Vector3 NewPos;
    public float AngleZ;
    public PlayerControl playerControl;

    private void OnEnable()
    {
        GameManager.OnTransUpdate += OnServerSinc;
        Handled.OnGetMessage += OnGetPos;

    }

    private void OnDisable()
    {
        GameManager.OnTransUpdate -= OnServerSinc;
        Handled.OnGetMessage -= OnGetPos;
   
    }

    private void OnServerSinc()
    {
        if (!playerControl.isMinePlayer.IsMine()) return;
        SendMess();
    }

    void Update ()
    {
        if (playerControl.isMinePlayer.IsMine()) return;

        if (Vector3.Distance(transform.position, NewPos) < 2f)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(NewPos.x, NewPos.y, 0), 10f * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 0f, AngleZ), 20f * Time.deltaTime);
        }
        else
        {
            transform.position = NewPos;
        }
    }

    async void SendMess()
    {
        await TransportHandler.Transport.SendTo(new DataPacket((byte)OperationCode.MyTransform, new Dictionary<object, object> { { (byte)ParameterCode.X, transform.position.x }, { (byte)ParameterCode.Y, transform.position.y }, { (byte)ParameterCode.Z, transform.eulerAngles.z }, { (byte)ParameterCode.Id, TransportHandler.Transport.Id } }, SendClientFlag.All));
    }

    private void OnGetPos(Vector3 pos, string _id)
    {
        if (playerControl.isMinePlayer.ID == _id)
        {
            NewPos = pos;
            AngleZ = pos.z;
            // fixed bug teleportation player
            NewPos.z = 0;
        }

    }

    
}
