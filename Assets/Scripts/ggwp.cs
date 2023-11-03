using GameLibrary.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ggwp : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            SendMess();
        }
    }

    public void LoadGame ()
    {
        MainSystem.instance.doMainThread(() => SceneManager.LoadScene("Game"));
    }
  

    async void SendMess()
    {
        await TransportHandler.Transport.SendTo(new DataPacket((byte)OperationCode.Message, new Dictionary<object, object> { { (byte)ParameterCode.Message, "GGWP" } }));
    }

    private void OnEnable ()
    {

        Handled.OnAutorized += LoadGame;
        Handled.OnConnected += OnConnected;
    }

   
    private void OnDisable()
    {
        Handled.OnAutorized -= LoadGame;
        Handled.OnConnected -= OnConnected;
    }

    private void Handled_OnGetMessage(string message)
    {
        Debug.Log($"Я получил и сделаль: {message}");
    }

    private void OnConnected()
    {
       // GameObject.Find("Canvas").SetActive(false);
    }
}
