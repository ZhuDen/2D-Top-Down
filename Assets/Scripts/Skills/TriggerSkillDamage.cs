using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSkillDamage : MonoBehaviour
{
    public int CurrentDamage;
    public string PlayerSpawnedSkillID;

    public void SetDataDamage (int _damageAmount, string _playerID)
    {
        CurrentDamage = _damageAmount;
        PlayerSpawnedSkillID = _playerID;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Tags>() != null)
        {
            if(collision.GetComponent<Tags>().MyTag == Tags.AllTags.Player)
            {
                collision.GetComponent<PlayerControl>().SetDamage(CurrentDamage, PlayerSpawnedSkillID);
                TextsSpawner.Instance.SpawnTextDamage(CurrentDamage, collision.transform.position);
            }
        }
    }
}
