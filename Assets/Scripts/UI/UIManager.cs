using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; set; }

    public InputField LoginUI;
    public InputField PasswordUI;
    public InputField NameUI;
    public Text Name;
    public Text UUID;
    public Text Lvl;
    public Text Exp;
    public Image Icon;
    public UIStateManager State;
    public IconData IconData;


    private void Awake()
    {
        Instance = this;
        //EventSystem.Instance.Event_Autorisation.AddListener(SetName);
    }

    public void Autorisation() {

        Debug.Log("State: " + State.CurrentKey);

        if (State.CurrentKey == "Login")
        {
            MainSystem.instance.Autorisation(LoginUI.text, PasswordUI.text);
            //State.SwitchToPanel("Wait");
        }
        else if (State.CurrentKey == "Registration")
        {
            MainSystem.instance.Registration(LoginUI.text, PasswordUI.text, NameUI.text);
           // State.SwitchToPanel("Wait");
        }

    }

    public void SetName() {

        Debug.Log("HEllo");
        Name.text = MainSystem.instance.Name;
    }
}
