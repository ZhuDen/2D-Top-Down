using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLibrary;
using GameLibrary.Tools;
using GameLibrary.Common;
using GameLibrary.Extension;
using Mono;
using System;
using UnityEngine.SceneManagement;

public static class Handled 
{
    public delegate void GetMessage(Vector3 pos);
    public static event GetMessage OnGetMessage;

    public delegate void Autorized();
    public static event Autorized OnAutorized;

    static Client Client = null;
    public static void HandleReceivedData(DataPacket packet)
    {


        //�������� ��� �����
        Dictionary<string, Transform> Clients = new Dictionary<string, Transform>();


        switch (packet.operationCode)
        {
            case OperationCode.Unknown:
                Debug.Log("Unknown command received: " + packet.operationCode.ToString());
                break;

            case OperationCode.Authorisation:

                if (packet.Data[ParameterCode.Message].ToString() == "Success")
                {

                    OnAutorized?.Invoke();
                    /* MainSystem.instance.ClientData.Name = packet.Data[ParameterCode.Name].ToString();
                     MainSystem.instance.ClientData.ID = packet.Data[ParameterCode.Id].ToString();
                     MainSystem.instance.ClientData.Icon = packet.Data[ParameterCode.IconIndex].ToString();
                     MainSystem.instance.ClientData.Lvl = packet.Data[ParameterCode.LVL].ToString();
                     MainSystem.instance.ClientData.Exp = packet.Data[ParameterCode.Exp].ToString();
                     MainSystem.instance.doMainThread(() => UIManager.Instance.Name.text = MainSystem.instance.ClientData.Name);
                     MainSystem.instance.doMainThread(() => UIManager.Instance.UUID.text = "UUID: " + MainSystem.instance.ClientData.ID);
                     MainSystem.instance.doMainThread(() => UIManager.Instance.Exp.text = MainSystem.instance.ClientData.Exp);
                     MainSystem.instance.doMainThread(() => UIManager.Instance.Lvl.text = MainSystem.instance.ClientData.Lvl);
                     MainSystem.instance.doMainThread(() => UIManager.Instance.Icon.sprite = UIManager.Instance.IconData.Icon[Convert.ToInt32(MainSystem.instance.ClientData.Icon)].Image);
                     MainSystem.instance.doMainThread(() => UIManager.Instance.State.SwitchToPanel("AuthGood"));*/

                    Debug.Log($" Lvl: {MainSystem.instance.ClientData.Lvl}");
                    Debug.Log($"Authorisation {packet.Data[ParameterCode.Name].ToString()}") ;
                }
                else 
                {
                    Debug.Log(packet.Data[ParameterCode.Message]);
                    MainSystem.instance.doMainThread(() => UIManager.Instance.State.SwitchToPanel("Login"));
                }
                break;

            case OperationCode.Registration:

                if (packet.Data[ParameterCode.Message].ToString() == "Registration successful")
                {
                    MainSystem.instance.ClientData.Name = packet.Data[ParameterCode.Name].ToString();
                    MainSystem.instance.ClientData.ID = packet.Data[ParameterCode.Id].ToString();
                    MainSystem.instance.ClientData.Icon = packet.Data[ParameterCode.IconIndex].ToString();
                    MainSystem.instance.ClientData.Lvl = packet.Data[ParameterCode.LVL].ToString();
                    MainSystem.instance.ClientData.Exp = packet.Data[ParameterCode.Exp].ToString();
                    MainSystem.instance.doMainThread(() => UIManager.Instance.Name.text = MainSystem.instance.ClientData.Name);
                    MainSystem.instance.doMainThread(() => UIManager.Instance.UUID.text = "UUID: " + MainSystem.instance.ClientData.ID);
                    MainSystem.instance.doMainThread(() => UIManager.Instance.Exp.text = MainSystem.instance.ClientData.Exp);
                    MainSystem.instance.doMainThread(() => UIManager.Instance.Lvl.text = MainSystem.instance.ClientData.Lvl);
                    MainSystem.instance.doMainThread(() => UIManager.Instance.Icon.sprite = UIManager.Instance.IconData.Icon[Convert.ToInt32(MainSystem.instance.ClientData.Icon)].Image);
                    MainSystem.instance.doMainThread(() => UIManager.Instance.State.SwitchToPanel("AuthGood"));

                    Debug.Log($" Lvl: {MainSystem.instance.ClientData.Lvl}");
                }
                else
                {
                    Debug.Log(packet.Data[ParameterCode.Message]);
                    MainSystem.instance.doMainThread(() => UIManager.Instance.State.SwitchToPanel("Registration"));
                }
                break;

            case OperationCode.Disconnect:
                Debug.Log("Server requested disconnection");
                break;

            case OperationCode.SetDamage:
                //SendScript.Instance.Data = ((ClientData)packet.Data[ParameterCode.Count]).Massive.Length;
                break;

            case OperationCode.Message:
                if (packet.Data != null)
                {
                    SendScript.Instance.Return++;
                    //TestSend ss = new TestSend();
                    float message = (float)packet.Data[ParameterCode.Message];
                  //  OnGetMessage?.Invoke(message);
                    Debug.Log($"Received message: {message} ");
                }
                else {
                    Debug.Log($"Received message: Null");
                }
                break;

            case OperationCode.Connect:
                Debug.Log($"Connected: {packet.Data[ParameterCode.Message].ToString()}");
                //MainSystem.instance.ClientData.ID = packet.Data[ParameterCode.Id].ToString();
                break;
            case OperationCode.MyTransform:
                OnGetMessage?.Invoke(new Vector3(float.Parse(packet.Data[ParameterCode.X].ToString()), float.Parse(packet.Data[ParameterCode.Y].ToString()), float.Parse(packet.Data[ParameterCode.Z].ToString())));
                //MainSystem.instance.ClientData.ID = packet.Data[ParameterCode.Id].ToString();
                break;

            case OperationCode.GetClientsInfo:
                Debug.Log("CLIENTS INFO");
                try
                {
                    MainSystem.instance.ClientData = (ClientData)packet.Data[ParameterCode.Client];
                }
                catch (Exception ex) { }
                Debug.Log($"Hello {MainSystem.instance.ClientData.ID} {MainSystem.instance.ClientData.NetTransform.Position.x}");
                break;

            default:
                Debug.Log($"Unnown Operation: ");
                break;
        }

    }

}
