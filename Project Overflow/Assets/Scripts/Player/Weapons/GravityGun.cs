using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityGun : WeaponBase
{
    public bool isGravitating;
    private bool triggerReleased = true;
    private RaycastHit target;
    Rigidbody targetBackup;

    Quaternion objectRotation;
    public Light gunLight;
    public Light muzzleLight;
    public Transform flare;
    public ParticleSystem sparks;
    public Electric lightning;

    public AudioSource localSource;
    public SkinnedMeshRenderer mesh;
    float blendShapeVal;

    float clawRetractTimer = 1;
    float clawRetractCount;

    float flareSize;
    float emission;
    float forceAmount;
    private float flareTarget;
    private float emissionTarget;

    private void OnEnable()
    {
        muzzleLight.gameObject.SetActive(true);
        flare.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        muzzleLight.gameObject.SetActive(false);
        flare.gameObject.SetActive(false);
        if (isGravitating) isGravitating = false;
    }

    public override void Update()
    {
        base.Update();

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit attracted, 5, ~(1 << LayerMask.NameToLayer("Player"))))
        {
            if (attracted.collider.attachedRigidbody != null)
            {
                ClawsAction(true);

                if (canShoot())
                {
                    if (Player.Instance.inputs.GetButton("Fire1"))
                    {
                        Shoot(attracted);
                    }
                }
            }
            else if (canShoot() && Player.Instance.inputs.GetButton("Fire1"))
            {
                DryFire();
            }
            else
            {
                ClawsAction(false);
            }
        }
        else if(canShoot() && Player.Instance.inputs.GetButton("Fire1") && !isGravitating)
        {
            DryFire();
        }
        else
        {
            ClawsAction(false);
        }



        if (isGravitating)
        {
            if (Player.Instance.inputs.GetButtonDown("Fire2"))
            {
                DropObject();
            }
        }
        else
        {
            if (canShoot() && triggerReleased)
            {
                if (Player.Instance.inputs.GetButton("Fire2"))
                {
                    Attract();
                }
            }
        }

        if (Player.Instance.inputs.GetButtonUp("Fire2") && !triggerReleased) triggerReleased = !triggerReleased;

        if (!isGravitating && triggerReleased && Player.Instance.inputs.GetButton("Fire2")) flareTarget = 0.04f;
        else if (!isGravitating) flareTarget = 0.01f;

        if (gunLight.intensity > 2)
        {
            if(!lightning.gameObject.activeSelf) lightning.gameObject.SetActive(true);
            gunLight.intensity -= Time.deltaTime * 70;
            if (flareTarget != 0.01f) flareTarget = 0.01f;

        }
        else
        {
            if (lightning.gameObject.activeSelf) lightning.gameObject.SetActive(false);
            if (emissionTarget != 0.6f && !isGravitating) emissionTarget = 0.6f;
        }

        emission = Mathf.Lerp(emission, emissionTarget, Time.deltaTime * 7);
        mesh.material.SetColor("_EmissionColor", new Color(2, emission, 0));

        flareSize = Mathf.Lerp(flareSize, flareTarget, Time.deltaTime * 7);
        flare.localScale = Vector3.one * flareSize;
    }

    private void FixedUpdate()
    {
        if (isGravitating && target.collider != null)
        {
            if(target.collider.attachedRigidbody != targetBackup)
            {
                targetBackup = target.collider.attachedRigidbody;
                Player.Instance.armsAnim.SetBool("isHolding", true);
                FxManager.EmitSound(sound[2], true);
                localSource.pitch = Time.timeScale;
                emissionTarget = 1;
                flareTarget = 0.04f;
                localSource.Play();
                blendShapeVal = 0;
                forceAmount = 0;
            }

            Vector3 gravityPole = (Camera.main.transform.position + Camera.main.transform.TransformDirection(Vector3.forward) * 2);

            forceAmount = Mathf.Lerp(forceAmount, 30, Time.deltaTime * 1.5f);

            Vector3 force = (gravityPole - target.transform.position) * forceAmount;

            target.collider.attachedRigidbody.velocity = Vector3.zero;

            target.collider.attachedRigidbody.AddForce(force, ForceMode.VelocityChange);
        }
    }
    public void Shoot(RaycastHit pushed)
    {
        Player.Instance.cam.screenEffect.color = new Color(255, 255, 255, .5f);
        Player.Instance.armsAnim.SetBool("isHolding", false);
        Player.Instance.armsAnim.Play("GravityGun shoot");
        Player.Instance.cam.AddPistolKickBack(-20, 1);
        FxManager.EmitSound(sound[7], true);
        FxManager.EmitSound(sound[6], true);
        sparks.transform.position = pushed.collider.transform.position;
        lightning.transformPointB = pushed.collider.transform;
        gunLight.intensity = 25;
        targetBackup = null;
        localSource.Stop();
        blendShapeVal = 0;
        flareSize = 0.15f;
        emission = 2;
        sparks.Play();
        counter = 0;

        pushed.collider.attachedRigidbody.AddForceAtPosition(Player.Instance.cam.worldCam.forward * 1000, pushed.point);

        if (isGravitating) isGravitating = false;
    }

    private void Attract()
    {
        if (Physics.BoxCast(Camera.main.transform.position + Vector3.up * 0.25f, Vector3.one * 0.25f, Camera.main.transform.forward, out target, Quaternion.identity, 20, ~(1 << LayerMask.NameToLayer("Player"))))
        {
            if (target.collider.attachedRigidbody != null)
            {
                if (target.distance <= 5)
                {
                    isGravitating = true;
                }
                else
                {
                    target.collider.attachedRigidbody.AddForceAtPosition((Camera.main.transform.position - target.transform.position) * (20 / target.distance), target.point);
                }
            }
            else
            {
                FxManager.EmitSound(sound[4], true);
                counter = 0;
            }
        }
    }

    private void DropObject()
    {
        Player.Instance.armsAnim.SetBool("isHolding", false);
        FxManager.EmitSound(sound[3], true);
        triggerReleased = !triggerReleased;
        isGravitating = false;
        targetBackup = null;
        localSource.Stop();
        counter = 0;
    }

    private void DryFire()
    {
        FxManager.EmitSound(sound[9], true);
        counter = 0;
    }

    void ClawsAction(bool clawsState)
    {
        if (clawRetractCount < clawRetractTimer) clawRetractCount += Time.deltaTime;

        if (clawsState)
        {
            if (blendShapeVal != 0 && !isGravitating && clawRetractCount >= clawRetractTimer)
            {
                blendShapeVal = 0;
                clawRetractCount = 0;
                FxManager.EmitSound(sound[0], true);
            }
        }
        else
        {
            if (blendShapeVal != 100 && !isGravitating && clawRetractCount >= clawRetractTimer)
            {
                blendShapeVal = 100;
                clawRetractCount = 0;
                FxManager.EmitSound(sound[1], true);
            }
        }

        if (blendShapeVal > 0 || blendShapeVal < 100) mesh.SetBlendShapeWeight(0, Mathf.Lerp(mesh.GetBlendShapeWeight(0), blendShapeVal, Time.deltaTime * 10));
    }

    public override void WeaponSpecificSetup()
    {
        base.WeaponSpecificSetup();
        sparks.gameObject.layer = LayerMask.NameToLayer("Default");
        lightning.gameObject.layer = LayerMask.NameToLayer("Default");
    }
}
