using GameLibrary.Common;
using GameLibrary.Extension;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public delegate void TransUpdate();
    public static event TransUpdate OnTransUpdate;

    public delegate void ServerSinc();
    public static event ServerSinc OnServerSinc;

    public delegate void ClickMouse();
    public static event ClickMouse OnClickMouse;


    private float TmTransFix = 0.1f, TmSincFix = 0f;

    [HideInInspector]
    public PlayerControl MyPlayerControl;

    public GameObject PrefabPlayer;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        TransportHandler.Transport.SendTo(new DataPacket((byte)OperationCode.SetTeam, new Dictionary<ParameterCode, object> { { ParameterCode.Message, "SetTeam" } }, SendClientFlag.Me));
    }

    private void OnEnable()
    {
        Handled.OnGetPlayers += OnGetPlayers;
    }

    private void OnDisable()
    {
        Handled.OnGetPlayers -= OnGetPlayers;
    }

    private void OnGetPlayers(List<TeamMember> _players)
    {
        Debug.Log("spawned");
        foreach (TeamMember player in _players)
        {
            Debug.Log("sdffsdfdsf");
            GameObject spawn_player = Instantiate(PrefabPlayer, new Vector3(0, 0, 0), PrefabPlayer.transform.rotation);
            spawn_player.GetComponent<IsMinePlayer>().ID = player.netClient.Id;
            if(player.netClient.Id == TransportHandler.Transport.Id)
            {
                MyPlayerControl = GameObject.FindObjectOfType<IsMinePlayer>().GetComponent<PlayerControl>();
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
            TmTransFix = 0f;
        }

        TmSincFix += Time.unscaledDeltaTime;
        if (TmSincFix >= 10f)
        {
            OnServerSinc?.Invoke();
            TmSincFix = 0f;
        }

    }
}
