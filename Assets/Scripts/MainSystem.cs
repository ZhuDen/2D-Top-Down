using GameLibrary.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using GameLibrary.Extension;

public class MainSystem : MonoBehaviour
{
    public static MainSystem instance;
    public string Name;
    public string UUID;
    public ClientData ClientData;
    public Room MyRoom;

    public InputField LogingField, PaswordField;

    internal void doMainThread(object v)
    {
        throw new NotImplementedException();
    }

    public SynchronizationContext _MainThread;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        ClientData = new ClientData();
        _MainThread = SynchronizationContext.Current;
    }
    //стереть нахуй потом
    private void Start()
    {
        Autorisation("Admin" , "Admin");

       
    }

    public void ClickButAutorisation()
    {
        Autorisation(LogingField.text, PaswordField.text);
    }

    public void ClickButRegistration ()
    {
        Registration(LogingField.text, PaswordField.text, "Den");
    }

    public void Autorisation(string Login, string Pass)
    {
        _authorisation(Login,Pass);
    }
    public void Registration(string Login, string Pass, string Name)
    {
        _registration(Login, Pass, Name);
    }
    private async Task _authorisation( string Login, string Pass)
    {
        await TransportHandler.Transport.SendTo(new DataPacket(OperationCode.Authorisation, new Dictionary<ParameterCode, object> { { ParameterCode.Login, Login }, { ParameterCode.Password, Pass } }));
    }
    private async Task _registration(string Login, string Pass, string Name)
    {
        await TransportHandler.Transport.SendTo(new DataPacket(OperationCode.Registration, new Dictionary<ParameterCode, object> { { ParameterCode.Login, Login }, { ParameterCode.Password, Pass }, { ParameterCode.Name, Name } }));
    }

    public void doMainThread(Action updateAction)
    {
        // Пример выполнения переданного действия в главном потоке
        _MainThread.Post(_ => {
            updateAction.Invoke(); // Вызов переданного делегата (метода)
        }, null);
    }


}
