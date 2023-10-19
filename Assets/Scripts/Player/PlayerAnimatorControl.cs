using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorControl : MonoBehaviour
{
    public Animator BodyAnimator, FootsAnimator;
    public int Walk;

    public void SetWalk (int _direction)
    {
        Walk = _direction;
    }

    public void SetElectricSkill()
    {
        BodyAnimator.SetInteger("ElectricSkill", 1);
        Invoke("BackIntegerElectric", 0.1f);
    }

    public void BackIntegerElectric ()
    {
        BodyAnimator.SetInteger("ElectricSkill", 0);
    }

    private void FixedUpdate()
    {
        FootsAnimator.SetInteger("Walk", Walk);
    }
}
