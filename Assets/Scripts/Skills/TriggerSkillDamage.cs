using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSkillDamage : MonoBehaviour
{
    public int CurrentDamage;

    public void SetDataDamage (int _damageAmount)
    {
        CurrentDamage = _damageAmount;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Tags>() != null)
        {
            if(collision.GetComponent<Tags>().MyTag == Tags.AllTags.Enemy)
            {
                TextsSpawner.Instance.SpawnTextDamage(CurrentDamage, collision.transform.position);
            }
        }
    }
}
