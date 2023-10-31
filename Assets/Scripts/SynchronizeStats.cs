using GameLibrary.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SynchronizeStats : MonoBehaviour
{
    public int HP;
    public float Multipler;
    public Image FillHP;
    public string Name;
    public PlayerControl playerControl;

    private void OnEnable()
    {
        GameManager.OnTransUpdate += OnServerSinc;
        Handled.OnGetString += OnGetString;

    }

    private void OnDisable()
    {
        GameManager.OnTransUpdate -= OnServerSinc;
        Handled.OnGetString -= OnGetString;

    }

    private void Update()
    {
        if (!playerControl.isMinePlayer.IsMine())
        {
            FillHP.fillAmount = 0.01f * HP;
        }
    }

    public void OnServerSinc ()
    {
        if (playerControl.isMinePlayer.IsMine())
        {
            if(HP != playerControl.playerStats.HP)
            {
                HP = playerControl.playerStats.HP;
                Multipler = playerControl.playerStats.MultiplerHP;
            }
            //тут надо подумать как каждый раз не отправлять
            SendHP();
        }
    }

    async void SendHP()
    {
        await TransportHandler.Transport.SendTo(new DataPacket((byte)OperationCode.Message, new Dictionary<byte, object> { { (byte)ParameterCode.Message, HP.ToString() + "|" + Multipler }, { (byte)ParameterCode.Id, TransportHandler.Transport.Id } }, SendClientFlag.All, true));
    }

    private void OnGetString(string _res, string _id)
    {
        Debug.Log(playerControl.isMinePlayer.ID + " || " +_id);
        if (playerControl.isMinePlayer.ID == _id)
        {
            if (!playerControl.isMinePlayer.IsMine())
            {
                HP = int.Parse(_res.Substring(0, _res.IndexOf('|')));
                Multipler = float.Parse(_res.Remove(0, _res.LastIndexOf("|") + 1));
            }

        }

    }
}
