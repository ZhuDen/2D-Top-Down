using GameLibrary.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynchronizeTransfrom : MonoBehaviour
{
    
    public Vector3 NewPos;
    public float AngleZ;
    public PlayerControl playerControl;

    public Vector3[] Points = new Vector3[10];
    public int IndexCurrentPoint, IndexAddedPoint;

    public float Vertical, Horizontal, SpeedMove;

    private void OnEnable()
    {
        GameManager.OnTransUpdate += OnServerSinc;
        Handled.OnGetMessage += OnGetPos;
        Handled.OnGetMessageAxis += OnGetAxis;

    }

    private void OnDisable()
    {
        GameManager.OnTransUpdate -= OnServerSinc;
        Handled.OnGetMessage -= OnGetPos;
        Handled.OnGetMessageAxis -= OnGetAxis;
    }

    private void OnServerSinc()
    {
        if (!playerControl.isMinePlayer.IsMine()) return;
        SendMess();
        SendControlPlayer();
    }

    void Update ()
    {
        transform.position += Vector3.up * Vertical * SpeedMove * Time.deltaTime;
        transform.position += Vector3.right * Horizontal * SpeedMove * Time.deltaTime;
    

        if (!playerControl.isMinePlayer.IsMine())
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 0f, AngleZ), 20f * Time.deltaTime);
            if (Vector3.Distance(transform.position, NewPos) > 0.5f)
            {
                transform.position = Vector3.Lerp(transform.position, NewPos, 10f * Time.deltaTime);
            }
        }
    }

    async void SendMess()
    {
        await TransportHandler.Transport.SendTo(new DataPacket((byte)OperationCode.MyTransform, new Dictionary<object, object> { { (byte)ParameterCode.X, transform.position.x }, { (byte)ParameterCode.Y, transform.position.y }, { (byte)ParameterCode.Z, transform.eulerAngles.z }, { (byte)ParameterCode.Id, TransportHandler.Transport.Id } }, SendClientFlag.All));
    }

    async void SendControlPlayer()
    {
        await TransportHandler.Transport.SendTo(new DataPacket((byte)OperationCode.Message, new Dictionary<object, object> { { (byte)MyParameters.ControlAxis, playerControl.Vertical + "|" + playerControl.Horizontal + "$" + playerControl.SpeedMove }, { (byte)ParameterCode.Id, TransportHandler.Transport.Id } }, SendClientFlag.All, true));
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
    private void OnGetAxis(string _axis, string _id)
    {
        if (playerControl.isMinePlayer.ID == _id)
        {
            Vertical = float.Parse(_axis.Substring(0, _axis.IndexOf('|')));
            _axis = _axis.Remove(0, _axis.IndexOf('|') + 1);
            Horizontal = float.Parse(_axis.Substring(0, _axis.IndexOf('$')));
            SpeedMove = float.Parse(_axis.Remove(0, _axis.LastIndexOf("$") + 1));
            CancelInvoke("InvokeResetAxis");
            Invoke("InvokeResetAxis", 1.5f);
        }

    }

    public void InvokeResetAxis ()
    {
        Vertical = 0;
        Horizontal = 0;
    }

}
