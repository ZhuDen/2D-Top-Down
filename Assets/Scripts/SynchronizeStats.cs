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
    public Text TextNick;
    public PlayerControl playerControl;

    private void Start()
    {
        if (playerControl.isMinePlayer.IsMine())
        {
            TextNick.text = MainSystem.instance.NameNick;
            SendNick();
        }
    }

    private void OnEnable()
    {
        GameManager.OnTransUpdate += OnServerSinc;
        Handled.OnUpdateNick += OnUpdateNick;
        Handled.OnNewPlayerConnected += OnNewPlayerConnected;
        Handled.OnGetString += OnGetString;

    }

    private void OnDisable()
    {
        GameManager.OnTransUpdate -= OnServerSinc;
        Handled.OnUpdateNick -= OnUpdateNick;
        Handled.OnNewPlayerConnected -= OnNewPlayerConnected;
        Handled.OnGetString -= OnGetString;

    }

    private void Update()
    {
        if (!playerControl.isMinePlayer.IsMine())
        {
            FillHP.fillAmount = 0.01f * HP;
        }
    }

    public void OnNewPlayerConnected ()
    {
        SendNick();
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
            SendNick();
        }
    }

    async void SendHP()
    {
        await TransportHandler.Transport.SendTo(new DataPacket((byte)OperationCode.Message, new Dictionary<object, object> { { (byte)ParameterCode.Message, HP.ToString() + "|" + Multipler }, { (byte)ParameterCode.Id, TransportHandler.Transport.Id } }, SendClientFlag.All, true));
    }

    async void SendNick()
    {
        await TransportHandler.Transport.SendTo(new DataPacket((byte)OperationCode.Message, new Dictionary<object, object> { { (byte)MyParameters.NickName, MainSystem.instance.NameNick }, { (byte)ParameterCode.Id, TransportHandler.Transport.Id } }, SendClientFlag.All, true));
    }

    private void OnGetString(string _res, string _id)
    {
       // Debug.Log(playerControl.isMinePlayer.ID + " || " +_id);
        if (playerControl.isMinePlayer.ID == _id)
        {
            if (!playerControl.isMinePlayer.IsMine())
            {
                HP = int.Parse(_res.Substring(0, _res.IndexOf('|')));
                Multipler = float.Parse(_res.Remove(0, _res.LastIndexOf("|") + 1));
            }

        }

    }

    private void OnUpdateNick(string _res, string _id)
    {
        Debug.Log(_res + " || " + _id);
        if (playerControl.isMinePlayer.ID == _id)
        {
            if (!playerControl.isMinePlayer.IsMine())
            {
                TextNick.text = _res;
            }

        }
    }

}
