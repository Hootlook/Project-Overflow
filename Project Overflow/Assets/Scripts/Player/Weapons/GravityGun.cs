using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityGun : WeaponBase
{
    public bool isGrabbed;
    private bool triggerReleased = true;
    private RaycastHit attracted;
    Rigidbody rbBackup;

    Quaternion objectRotation;
    public Light gunLight;
    public Light muzzleLight;
    public ParticleSystem sparks;
    public Electric lightning;

    public AudioSource localSource;
    public SkinnedMeshRenderer mesh;
    float blendShapeVal;

    float clawRetractTimer = 1;
    float clawRetractCount;

    private void OnEnable()
    {
        muzzleLight.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        muzzleLight.gameObject.SetActive(false);
        if(isGrabbed) isGrabbed = false;
    }

    public void Shoot(RaycastHit pushed)
    {
        FxManager.EmitSound(sound[7], true);
        FxManager.EmitSound(sound[6], true);
        Player.Instance.cam.AddPistolKickBack(-20, 1);
        gunLight.intensity = 25;
        sparks.transform.position = pushed.collider.transform.position;
        lightning.transformPointB = pushed.collider.transform;
        sparks.Play();
        localSource.Stop();
        rbBackup = null;

        if (isGrabbed)
        {
            isGrabbed = false;
            pushed.collider.attachedRigidbody.AddForceAtPosition(Player.Instance.cam.worldCam.forward * 1000, pushed.point);
        }
        else
        {
            pushed.collider.attachedRigidbody.AddForceAtPosition(Player.Instance.cam.worldCam.forward * 1000, pushed.point);
        }
    }

    private void Attract()
    {
        if (Physics.BoxCast(Camera.main.transform.position + Vector3.up * 0.25f, Vector3.one * 0.25f, Camera.main.transform.forward, out attracted, Quaternion.identity, 20, ~(1 << LayerMask.NameToLayer("Player"))))
        {
            if (attracted.collider.attachedRigidbody != null)
            {
                if(attracted.distance <= 5)
                {
                    isGrabbed = true;
                }
                else
                {
                    attracted.collider.attachedRigidbody.AddForceAtPosition((Camera.main.transform.position - attracted.transform.position) * (20 / attracted.distance), attracted.point);
                }
            }
        }
        else if (canShoot() && Player.Instance.inputs.GetButton("Fire2"))
        {
            FxManager.EmitSound(sound[9], true);
            counter = 0;
        }
    }

    public override void Update()
    {
        base.Update();

        if (clawRetractCount < clawRetractTimer) clawRetractCount += Time.deltaTime;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit attracted, 5, ~(1 << LayerMask.NameToLayer("Player"))))
        {
            if (attracted.collider.attachedRigidbody != null)
            {
                if (blendShapeVal != 0 && !isGrabbed && clawRetractCount >= clawRetractTimer)
                {
                    blendShapeVal = 0;
                    clawRetractCount = 0;
                    FxManager.EmitSound(sound[0], true);
                }

                if (canShoot())
                {
                    if (Player.Instance.inputs.GetButton("Fire1"))
                    {
                        Shoot(attracted);
                        Player.Instance.cam.screenEffect.color = new Color(255, 255, 255, .5f);
                        triggerReleased = !triggerReleased;
                        counter = 0;
                    }
                }
            }
            else if (canShoot() && Player.Instance.inputs.GetButton("Fire1"))
            {
                FxManager.EmitSound(sound[8], true);
                counter = 0;
            }
            else
            {
                if (blendShapeVal != 100 && !isGrabbed && clawRetractCount >= clawRetractTimer)
                {
                    blendShapeVal = 100;
                    clawRetractCount = 0;
                    FxManager.EmitSound(sound[1], true);
                }
            }
        }
        else if(canShoot() && Player.Instance.inputs.GetButton("Fire1") && !isGrabbed)
        {
            FxManager.EmitSound(sound[8], true);
            counter = 0;
        }
        else
        {
            if(blendShapeVal != 100 && !isGrabbed && clawRetractCount >= clawRetractTimer)
            {
                blendShapeVal = 100;
                clawRetractCount = 0;
                FxManager.EmitSound(sound[1], true);
            }
        }

        mesh.SetBlendShapeWeight(0, Mathf.Lerp(mesh.GetBlendShapeWeight(0), blendShapeVal, Time.deltaTime * 10));

        if (isGrabbed)
        {
            if (Player.Instance.inputs.GetButtonDown("Fire2"))
            {
                isGrabbed = false;
                localSource.Stop();
                rbBackup = null;
                counter = 0;
            }
        }
        else
        {
            if (canShoot())
            {
                if (Player.Instance.inputs.GetButton("Fire2"))
                {
                    Attract();
                }
            }
        }

        if (gunLight.intensity > 2)
        {
            if(!lightning.gameObject.activeSelf) lightning.gameObject.SetActive(true);
            gunLight.intensity -= Time.deltaTime * 70;
        }
        else
        {
            if (lightning.gameObject.activeSelf) lightning.gameObject.SetActive(false);
        }
        if (Input.GetKey(KeyCode.B)) attracted.collider.attachedRigidbody.drag++;
        if (Input.GetKey(KeyCode.N)) attracted.collider.attachedRigidbody.drag--;
    }

    private void FixedUpdate()
    {
        if (isGrabbed && attracted.collider != null)
        {
            if(attracted.collider.attachedRigidbody != rbBackup)
            {
                rbBackup = attracted.collider.attachedRigidbody;
                blendShapeVal = 0;
                localSource.pitch = Time.timeScale;
                FxManager.EmitSound(sound[2], true);
                localSource.Play();
            }

            Vector3 gravityPole = (Camera.main.transform.position + Camera.main.transform.TransformDirection(Vector3.forward) * 2);

            var force = (gravityPole - attracted.collider.transform.position) * 20;

            attracted.collider.attachedRigidbody.velocity = Vector3.zero;

            attracted.collider.attachedRigidbody.AddForce(force, ForceMode.VelocityChange);
        }
    }

    public override void WeaponSpecificSetup()
    {
        base.WeaponSpecificSetup();
        sparks.gameObject.layer = LayerMask.NameToLayer("Default");
        lightning.gameObject.layer = LayerMask.NameToLayer("Default");
    }
}
