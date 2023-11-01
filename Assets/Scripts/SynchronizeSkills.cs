using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynchronizeSkills : MonoBehaviour
{
    public PlayerControl MyPlayerControl;
    [System.Serializable]
    public class Skill
    {
        public string SkillName;
        public GameObject PrefabsSkills;
    }
    public Skill[] AllSkills;

    public void SpawnSkill (string _typeSkill, string _id, string nameSkill, string _damage, string mouseX, string mouseY)
    {
        TypeSkills type_skill = (TypeSkills)System.Enum.Parse(typeof(TypeSkills), _typeSkill);
        GameObject skill_gm = GetSkill(nameSkill);

        if (skill_gm == null) return;

        if (type_skill == TypeSkills.AmedByPoint)
        {
            MyPlayerControl.playerAnimatorControl.SetElectricSkill();
            Vector2 posMouse = Camera.main.ScreenToWorldPoint(new Vector3(float.Parse(mouseX), float.Parse(mouseY), 0));
            GameObject skill = Instantiate(skill_gm, MyPlayerControl.SpawnSkills.position, skill_gm.transform.rotation);
            skill.GetComponent<MoveAndExplosion>().StartMove(new Vector3(posMouse.x, posMouse.y, 0), int.Parse(_damage));
        }
        if (type_skill == TypeSkills.NonDirectional)
        {
            GameObject skill = Instantiate(skill_gm, MyPlayerControl.transform.position, skill_gm.transform.rotation);
           // MyPlayerControl.transform.position = new Vector3(MyPlayerControl.transform.position.x + Random.Range(-8f, 8f), MyPlayerControl.transform.position.y + Random.Range(-8f, 8f), 0);
        }
    }

    public GameObject GetSkill (string nameSkill)
    {
        for (int i = 0; i < AllSkills.Length; i++)
        {
            if(nameSkill == AllSkills[i].SkillName)
            {
                return AllSkills[i].PrefabsSkills;
            }
        }
        return null;
    }
}
