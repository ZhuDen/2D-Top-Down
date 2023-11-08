using GameLibrary.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public GameObject[] MySkills;
    public Transform PlayerTransform, PlayerFollowTrans, CameraTransform, SpawnSkills;
    public float SpeedRotate, SpeedMove, SpeedCamera;

    private Vector3 MousePos;
    public Skills SelectedSkill;

    public delegate void DeselectSkill();
    public static event DeselectSkill OnDeselectSkill;

    public PlayerAnimatorControl playerAnimatorControl;
    public PlayerStats playerStats;
    public IsMinePlayer isMinePlayer;
    public Vector3 MyPositionNew, MyPositionOld;
    public float Vertical, Horizontal;

    /*  private void Awake()
      {
          CameraTransform = Camera.main.transform;
          PlayerTransform = this.transform;
      }
      */

    private void Start()
    {
        MyPositionNew = transform.position;
        // PlayerFollowTrans.SetParent(null);
    }

    private void OnEnable()
    {
        GameManager.OnInitPlayer += OnInitPlayer;
    }

    private void OnDisable()
    {
        GameManager.OnInitPlayer -= OnInitPlayer;
    }

    private void OnInitPlayer()
    {
        if (!isMinePlayer.IsMine()) return;

        CameraTransform = Camera.main.transform;
        PlayerTransform = this.transform;
        UISpawner.Instance.SpawnSkills(MySkills, this);
    }

    void Update()
    {
        if (!isMinePlayer.IsMine()) return;

        RotatePlayer();
        MovePlayer();
        CameraFollow();

        if (Input.GetKeyDown(KeyCode.G))
        {
            TransportHandler.Transport.SendTo(new DataPacket((byte)OperationCode.SetTeam, new Dictionary<object, object> { { (byte)ParameterCode.Message, "SetTeam" } }, SendClientFlag.Me, false));
            //TransportHandler.Transport.SendTo(new DataPacket(OperationCode.GetAllRoom, new Dictionary<ParameterCode, object> { { ParameterCode.Message, "Update"}}, SendClientFlag.Me));
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            TransportHandler.Transport.SendTo(new DataPacket((byte)OperationCode.SetTeam, new Dictionary<object, object> { { (byte)ParameterCode.Message, "SetTeam" } }, SendClientFlag.Me, true));
            //TransportHandler.Transport.SendTo(new DataPacket(OperationCode.GetAllRoom, new Dictionary<ParameterCode, object> { { ParameterCode.Message, "Update"}}, SendClientFlag.Me));
        }
    }

    public void DeselectAllSkills ()
    {
        OnDeselectSkill?.Invoke();
    }

    public void CameraFollow ()
    {
        CameraTransform.position = Vector3.Lerp(CameraTransform.position, new Vector3(PlayerTransform.position.x, PlayerTransform.position.y, -10f), SpeedCamera * Time.deltaTime);
    }

    public void MovePlayer ()
    {
        Vertical = Input.GetAxisRaw("Vertical");
        Horizontal = Input.GetAxisRaw("Horizontal");

        playerAnimatorControl.SetWalk((int)Input.GetAxisRaw("Vertical"));

    }

    public void RotatePlayer ()
    {
        MousePos = Input.mousePosition;
        MousePos.z = Camera.main.orthographicSize;

        Vector3 objectPos = Camera.main.WorldToScreenPoint(PlayerTransform.position);
        MousePos.x = MousePos.x - objectPos.x;
        MousePos.y = MousePos.y - objectPos.y;

        float angle = Mathf.Atan2(MousePos.y, MousePos.x) * Mathf.Rad2Deg;
        PlayerTransform.rotation = Quaternion.Lerp(PlayerTransform.rotation, Quaternion.Euler(new Vector3(0, 0, angle)), SpeedRotate * Time.deltaTime);
    }

    public void SetDamage (int _damage, string _playerEnemyId)
    {
        if (isMinePlayer.IsMine())
        {
            if (isMinePlayer.ID != _playerEnemyId)
            {
                playerStats.UpdateHP(_damage, PlayerStats.TypeSummation.Minus);
            }
        }
    }
}
