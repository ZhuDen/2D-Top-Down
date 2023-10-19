using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIStateManager : MonoBehaviour
{
    public List<SerializableDictionary> panels;

    public string CurrentKey = "Login";

    private void Awake()
    {
        SwitchToPanel(CurrentKey);
    }

    public void SwitchToPanel(string panelName)
    {
        CurrentKey = panelName;

        foreach (var panel in panels)
        {
            panel.value.SetActive(false);
        }

        if (panels.Any(x => x.key == panelName))
        {
            var panelToActivate = panels.FindAll(x => x.key == panelName);

            foreach (var panelAct in panelToActivate)
            {
                panelAct.value.SetActive(true);
            }
        }
    }


}

[System.Serializable]
public class SerializableDictionary
{
    public string key;
    public GameObject value;
}
