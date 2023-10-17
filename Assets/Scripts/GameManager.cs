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

    public PlayerControl MyPlayerControl;


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        MyPlayerControl = GameObject.FindObjectOfType<IsMinePlayer>().GetComponent<PlayerControl>();
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
