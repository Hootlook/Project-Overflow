using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crowbar : WeaponBase
{
    public void Swing()
    {
        FxManager.EmitSound(sound[0], true);

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 2, ~(1 << LayerMask.NameToLayer("Player"))))
        {
            if(hit.collider)
            {
                Player.Instance.cam.AddPistolKickBack(5);
                Player.Instance.armsAnim.Play("crowbar hit");
            }
            if (hit.collider.attachedRigidbody != null)
            {
                hit.collider.attachedRigidbody.AddForceAtPosition(Player.Instance.cam.worldCam.forward * 250, hit.point);
            }
        }
        else
        {
            Player.Instance.armsAnim.Play("crowbar swing");
        }
    }

    public override void Update()
    {
        base.Update();

        if (counter >= timer)
        {
            if (Player.Instance.inputs.GetButton("Fire1"))
            {
                Swing();
                counter = 0;
            }
        }
    }
}
