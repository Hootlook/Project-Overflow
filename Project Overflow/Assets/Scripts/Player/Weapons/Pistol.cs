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

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 20, ~(1 << LayerMask.NameToLayer("Player"))))
        {
            if (hit.collider.attachedRigidbody != null)
            {
                hit.collider.attachedRigidbody.AddForceAtPosition(Player.Instance.cam.worldCam.forward * 250, hit.point);
            }
        }
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
