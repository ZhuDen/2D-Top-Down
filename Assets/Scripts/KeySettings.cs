using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class KeySettings : MonoBehaviour
{
    public static KeySettings init;

    public KeyCode SigKey = KeyCode.G;

    public KeyCode ActionKey_1 = KeyCode.Q;
    public KeyCode ActionKey_2 = KeyCode.W;
    public KeyCode ActionKey_3 = KeyCode.E;
    public KeyCode ActionKey_4 = KeyCode.R;

    private bool isSettingKey = false;
    private KeyCode bufferkey;

    private void Awake()
    {
        init = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(SigKey))
        {
            Setter();
        }
    }

    public async void Setter()
    {

            KeyCode selectedKey = await SetKey();
            SetActionKey(ref ActionKey_1, selectedKey);
        
    }

    private void SetActionKey(ref KeyCode actionKey, KeyCode newKey)
    {
        if (newKey != KeyCode.None)
        {
            actionKey = newKey;
        }
    }

    private async Task<KeyCode> SetKey()
    {
        KeyCode selectedKey = KeyCode.None;
        bool waitingForKey = true;

        while (waitingForKey)
        {
            foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode) && keyCode != SigKey)
                {
                    selectedKey = keyCode;
                    waitingForKey = false;
                    break;
                }
            }

            await Task.Yield();
        }

        return selectedKey;
    }

    private async Task<KeyCode> SetKeyElement()
    {
        KeyCode selectedKey = KeyCode.None;
        bool waitingForKey = true;

        while (waitingForKey)
        {
            foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode) && keyCode != SigKey)
                {
                    selectedKey = keyCode;
                    waitingForKey = false;
                    break;
                }
            }

            await Task.Yield();
        }

        return selectedKey;
    }
}
