using GameLibrary.Common;
using GameLibrary.Extension;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public delegate void TransUpdate();
    public static event TransUpdate OnTransUpdate;

    public delegate void ServerSinc();
    public static event ServerSinc OnServerSinc;

    public delegate void ClickMouse();
    public static event ClickMouse OnClickMouse;

    public delegate void InitPlayer();
    public static event InitPlayer OnInitPlayer;


    private float TmTransFix = 0.1f, TmSincFix = 0f;

    [HideInInspector]
    public PlayerControl MyPlayerControl;

    public GameObject PrefabPlayer;

    public Text TextPing;

    public DateTime dateTimeOld, dateTimeNew;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        TransportHandler.Transport.SendTo(new DataPacket((byte)OperationCode.SetTeam, new Dictionary<byte, object> { { (byte)ParameterCode.Message, "SetTeam" } }, SendClientFlag.Me));
    }

    private void OnEnable()
    {
        Handled.OnGetPlayers += OnGetPlayers;
    }

    private void OnDisable()
    {
        Handled.OnGetPlayers -= OnGetPlayers;

        TransportHandler.Transport.SendTo(new DataPacket((byte)OperationCode.Disconnect, new Dictionary<byte, object> { { (byte)ParameterCode.Message, "Desconnected" } }, SendClientFlag.Me));

    }

    private void OnGetPlayers(List<TeamMember> _players)
    {
        // check all ID players
        GameObject[] AllPlayers = GameObject.FindGameObjectsWithTag("Player");
        List<string> players_id = new List<string>();
        for (int i = 0; i < AllPlayers.Length; i++)
        {
            players_id.Add(AllPlayers[i].GetComponent<IsMinePlayer>().ID);
        }
        foreach (TeamMember player in _players)
        {
            bool isSpawn = true;
            if(players_id.Count > 0)
            {
                if (players_id.Contains(player.netClient.Id))
                {
                    isSpawn = false;
                }
            }
            if (isSpawn)
            {
                GameObject spawn_player = Instantiate(PrefabPlayer, new Vector3(0, 0, 0), PrefabPlayer.transform.rotation);
                spawn_player.GetComponent<IsMinePlayer>().ID = player.netClient.Id;
                spawn_player.name = "Player_" + UnityEngine.Random.Range(11, 99);
                Debug.Log("SETTED: " + player.netClient.Id);
                if (player.netClient.Id == TransportHandler.Transport.Id)
                {
                    MyPlayerControl = GameObject.FindObjectOfType<IsMinePlayer>().GetComponent<PlayerControl>();
                    OnInitPlayer?.Invoke();
                }
            }
        }
       
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                OnClickMouse?.Invoke();
            }
        }
    }

    private void FixedUpdate()
    {
        TmTransFix += Time.unscaledDeltaTime;
        if (TmTransFix >= 0.09f)
        {
            OnTransUpdate?.Invoke();
            SendPingTest();
            TmTransFix = 0f;
        }

        TmSincFix += Time.unscaledDeltaTime;
        if (TmSincFix >= 10f)
        {
            OnServerSinc?.Invoke();
            TmSincFix = 0f;
        }

    }

    async void SendPingTest ()
    {
        dateTimeOld = DateTime.Now;
        await TransportHandler.Transport.SendTo(new DataPacket((byte)OperationCode.Message, new Dictionary<byte, object> { { (byte)MyParameters.Ping, "Ping" }}, SendClientFlag.Me));

    }

    public void UpdatePing ()
    {
        TimeSpan pingTime = DateTime.Now - dateTimeOld;
        TextPing.text = string.Format("ѕинг: {0} мс", pingTime.Milliseconds);
    }
}
