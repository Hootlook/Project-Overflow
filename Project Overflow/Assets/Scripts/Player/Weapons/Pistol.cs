using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : WeaponBase
{
    public void Shoot()
    {
        Player.Instance.armsAnim.Play("pistol shoot");
        Player.Instance.cam.AddPistolKickBack(2.5f, 5);
        FxManager.EmitSound(sound[0], true);
    }

    public override void Update()
    {
        base.Update();

        if (counter >= timer)
        {
            if (Player.Instance.inputs.GetButton("Fire2"))
            {
                Shoot();
                counter = 0;
            }
            if (Player.Instance.inputs.GetButtonDown("Fire1"))
            {
                Shoot();
                counter = 0;
            }
        }
    }
}
