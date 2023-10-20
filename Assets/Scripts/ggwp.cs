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
        await TransportHandler.Transport.SendTo(new DataPacket(OperationCode.Message, new Dictionary<ParameterCode, object> { { ParameterCode.Message, "GGWP" } }));
    }

    private void OnEnable ()
    {

        Handled.OnAutorized += LoadGame;
    }

   
    private void OnDisable()
    {
        Handled.OnAutorized -= LoadGame;
    }

    private void Handled_OnGetMessage(string message)
    {
        Debug.Log($"� ������� � �������: {message}");
    }

}
