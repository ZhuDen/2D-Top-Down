using GameLibrary.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPos : MonoBehaviour
{
    public Transform Enemy;
    public Vector3 NewPos;

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
        Debug.Log("loh");
        SendMess();
    }

    void Update ()
    {
        if (Vector3.Distance(Enemy.localPosition, NewPos) < 2f)
        {
            Enemy.localPosition = Vector3.Lerp(Enemy.localPosition, NewPos, 10f * Time.deltaTime);
        }
        else
        {
            Enemy.localPosition =  NewPos;
        }
    }

    async void SendMess()
    {
        await TransportHandler.Transport.SendTo(new DataPacket(OperationCode.MyTransform, new Dictionary<ParameterCode, object> { { ParameterCode.X, transform.position.x }, { ParameterCode.Y, transform.position.y }, { ParameterCode.Z, 0 } }, SendClientFlag.All));
    }

    private void OnGetPos(Vector3 pos)
    {
        NewPos = pos;
        Debug.Log("dfssfd");
    }

    
}
