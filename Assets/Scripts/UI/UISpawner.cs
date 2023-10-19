using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISpawner : MonoBehaviour
{
    public static UISpawner Instance;
    public Transform ParentSkills;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void SpawnSkills (GameObject[] _skills)
    {
        for (int i = 0; i < _skills.Length; i++)
        {
            GameObject skill = Instantiate(_skills[i], ParentSkills);
            skill.GetComponent<SkillKeyboard>().OnSetKey(KeySettings.Instance.ActionKeys[i]);
        }

    }
}
