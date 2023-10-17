using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public Transform PlayerTransform, CameraTransform, SpawnSkills;
    public float SpeedRotate, SpeedMove, SpeedCamera;

    private Vector3 MousePos;
    public Skills SelectedSkill;

    public delegate void DeselectSkill();
    public static event DeselectSkill OnDeselectSkill;

    void Update()
    {
        RotatePlayer();
        MovePlayer();
        CameraFollow();
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
        PlayerTransform.position += Vector3.up * Input.GetAxisRaw("Vertical") * SpeedMove * Time.deltaTime;
        PlayerTransform.position += Vector3.right * Input.GetAxisRaw("Horizontal") * SpeedMove * Time.deltaTime;
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
}
