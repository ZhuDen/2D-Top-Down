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
    public delegate void GetMessage(Vector3 pos, string id);
    public static event GetMessage OnGetMessage;

    public delegate void GetString(string _res, string id);
    public static event GetString OnGetString;

    public delegate void Autorized();
    public static event Autorized OnAutorized;

    public delegate void Connected();
    public static event Connected OnConnected;

    public delegate void GetPlayers(List<TeamMember> _players);
    public static event GetPlayers OnGetPlayers;

    static Client Client = null;
    public static void HandleReceivedData(DataPacket packet)
    {


        //Временно для теста
        Dictionary<string, Transform> Clients = new Dictionary<string, Transform>();

        if (packet.Rpc == false)
        {
            switch ((OperationCode)packet.operationCode)
            {
                case OperationCode.Unknown:
                    Debug.Log("Unknown command received: " + packet.operationCode.ToString());
                    break;

                case OperationCode.Authorisation:

                    if (packet.Data[(byte)ParameterCode.Message].ToString() == "Success")
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
                        Debug.Log($"Authorisation {packet.Data[(byte)ParameterCode.Name].ToString()}");
                        Debug.Log("ID: " + packet.Data[(byte)ParameterCode.Id].ToString());
                        TransportHandler.Transport.Id = packet.Data[(byte)ParameterCode.Id].ToString();
                    }
                    else
                    {
                        Debug.Log(packet.Data[(byte)ParameterCode.Message]);
                        MainSystem.instance.doMainThread(() => UIManager.Instance.State.SwitchToPanel("Login"));
                    }
                    break;

                case OperationCode.Registration:

                    if (packet.Data[(byte)ParameterCode.Message].ToString() == "Registration successful")
                    {
                        MainSystem.instance.ClientData.Name = packet.Data[(byte)ParameterCode.Name].ToString();
                        MainSystem.instance.ClientData.ID = packet.Data[(byte)ParameterCode.Id].ToString();
                        MainSystem.instance.ClientData.Icon = packet.Data[(byte)ParameterCode.IconIndex].ToString();
                        MainSystem.instance.ClientData.Lvl = packet.Data[(byte)ParameterCode.LVL].ToString();
                        MainSystem.instance.ClientData.Exp = packet.Data[(byte)ParameterCode.Exp].ToString();
                        /*  MainSystem.instance.doMainThread(() => UIManager.Instance.Name.text = MainSystem.instance.ClientData.Name);
                          MainSystem.instance.doMainThread(() => UIManager.Instance.UUID.text = "UUID: " + MainSystem.instance.ClientData.ID);
                          MainSystem.instance.doMainThread(() => UIManager.Instance.Exp.text = MainSystem.instance.ClientData.Exp);
                          MainSystem.instance.doMainThread(() => UIManager.Instance.Lvl.text = MainSystem.instance.ClientData.Lvl);
                          MainSystem.instance.doMainThread(() => UIManager.Instance.Icon.sprite = UIManager.Instance.IconData.Icon[Convert.ToInt32(MainSystem.instance.ClientData.Icon)].Image);*/
                        MainSystem.instance.doMainThread(() => UIManager.Instance.State.SwitchToPanel("AuthGood"));

                        Debug.Log($" Lvl: {MainSystem.instance.ClientData.Lvl}");
                    }
                    else
                    {
                        Debug.Log(packet.Data[(byte)ParameterCode.Message]);
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
                        string message = packet.Data[(byte)ParameterCode.Message].ToString();
                        MainSystem.instance.doMainThread(() => OnGetString?.Invoke(message, packet.Data[(byte)ParameterCode.Id].ToString()));
                    }
                    else
                    {
                        Debug.Log($"Received message: Null");
                    }
                    break;

                case OperationCode.Connect:
                    Debug.Log($"Connected: {packet.Data[(byte)ParameterCode.Message].ToString()}");
                    //  MainSystem.instance.ClientData.ID = packet.Data[ParameterCode.Id].ToString();
                    MainSystem.instance.doMainThread(() => OnConnected?.Invoke());

                    break;
                case OperationCode.SetTeam:
                    Debug.Log("New con");
                    TransportHandler.Transport.SendTo(new DataPacket((byte)OperationCode.GetInfoRoom, new Dictionary<byte, object> { { (byte)ParameterCode.Message, "Update" } }, SendClientFlag.FullRoom));

                    break;
                case OperationCode.GetInfoRoom:
                    Debug.Log($"UUID {((Room)packet.Data[(byte)ParameterCode.TeamMember]).Team[0].netClient.Id}");
                    

                    if (MainSystem.instance.MyRoom != null)
                    {
                        if (((Room)packet.Data[(byte)ParameterCode.TeamMember]).UUID == MainSystem.instance.MyRoom.UUID)
                        {
                            MainSystem.instance.MyRoom = (Room)packet.Data[(byte)ParameterCode.TeamMember];
                        }
                        MainSystem.instance.doMainThread(() => OnGetPlayers?.Invoke(((Room)packet.Data[(byte)ParameterCode.TeamMember]).Team));
                    }
                    else
                    {
                        MainSystem.instance.MyRoom = (Room)packet.Data[(byte)ParameterCode.TeamMember];
                        MainSystem.instance.doMainThread(() => OnGetPlayers?.Invoke(((Room)packet.Data[(byte)ParameterCode.TeamMember]).Team));
                        Debug.Log($"Count{ MainSystem.instance.MyRoom.Team.Count}");
                    }


                    break;
                case OperationCode.MyTransform:
                    OnGetMessage?.Invoke(new Vector3(float.Parse(packet.Data[(byte)ParameterCode.X].ToString()), float.Parse(packet.Data[(byte)ParameterCode.Y].ToString()), float.Parse(packet.Data[(byte)ParameterCode.Z].ToString())), packet.Data[(byte)ParameterCode.Id].ToString());

                    break;

                case OperationCode.GetClientsInfo:
                    Debug.Log("CLIENTS INFO");
                    try
                    {
                        MainSystem.instance.ClientData = (ClientData)packet.Data[(byte)ParameterCode.Client];
                    }
                    catch (Exception ex) { }
                    Debug.Log($"Hello {MainSystem.instance.ClientData.ID} {MainSystem.instance.ClientData.NetTransform.Position.x}");
                    break;

                default:
                    Debug.Log($"Unnown Operation: ");
                    break;
            }
        }
        else {

            if (packet.Data.ContainsKey((byte)ParameterCode.Message))
            {
                MainSystem.instance.doMainThread(() => OnGetString?.Invoke(packet.Data[(byte)ParameterCode.Message].ToString(), packet.Data[(byte)ParameterCode.Id].ToString()));
            }

            Debug.Log($"RPC REQUEST: { packet.Data.Keys}");

        }

    }

}
