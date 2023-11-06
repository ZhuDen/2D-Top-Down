using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynchronizeSkills : MonoBehaviour
{
    public PlayerControl MyPlayerControl;
    [System.Serializable]
    public class Skill
    {
        public Skills NameSkill;
        public GameObject PrefabsSkills;
    }
    public Skill[] AllSkills;

    private void OnEnable()
    {
        Handled.OnSpawnSkill += SpawnSkill;
    }

    private void OnDisable()
    {
        Handled.OnSpawnSkill -= SpawnSkill;
    }

    public void SpawnSkill (string _typeSkill, string _id, string _nameSkill, string _damage, string _mousePos)
    {
        if (MyPlayerControl.isMinePlayer.ID == _id)
        {
          

            TypeSkills type_skill = (TypeSkills)System.Enum.Parse(typeof(TypeSkills), _typeSkill);
            GameObject skill_gm = GetSkill(_nameSkill);

            if (skill_gm == null) return;

            float mouseXpos = float.Parse(_mousePos.Substring(0, _mousePos.IndexOf('|')));
            float mouseYpos = float.Parse(_mousePos.Remove(0, _mousePos.LastIndexOf("|") + 1));
            Debug.Log("getted: " + mouseXpos + "|" + mouseYpos);

            if (type_skill == TypeSkills.AmedByPoint)
            {
                MyPlayerControl.playerAnimatorControl.SetElectricSkill();
               // Vector2 posMouse = Camera.main.ScreenToWorldPoint(new Vector3(mouseXpos, mouseYpos, 0));
                GameObject skill = Instantiate(skill_gm, MyPlayerControl.SpawnSkills.position, skill_gm.transform.rotation);
                skill.GetComponent<MoveAndExplosion>().StartMove(new Vector3(mouseXpos, mouseYpos, 0), int.Parse(_damage), _id);
            }
            if (type_skill == TypeSkills.NonDirectional)
            {
                GameObject skill = Instantiate(skill_gm, MyPlayerControl.transform.position, skill_gm.transform.rotation);
                // MyPlayerControl.transform.position = new Vector3(MyPlayerControl.transform.position.x + Random.Range(-8f, 8f), MyPlayerControl.transform.position.y + Random.Range(-8f, 8f), 0);
            }
        }
    }

    public GameObject GetSkill (string nameSkill)
    {
        for (int i = 0; i < AllSkills.Length; i++)
        {
            if(nameSkill == AllSkills[i].NameSkill.ToString())
            {
                return AllSkills[i].PrefabsSkills;
            }
        }
        return null;
    }
}
