using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCamera : MonoBehaviour
{
    [Range(0, 2)]
    public float cameraSensitivity;
    public float cameraY;
    public float cameraX;
    public Transform rotor;
    public Transform pivot;
    public Transform worldCam;
    public Transform viewCam;
    public float amount = 0.01f;
    public float maxAmout = 0.04f;
    public float smoothAmout = 5;
    private Vector3 fallDistance;
    private Vector3 kickBack;

    [Header("HUD Part")]
    public Transform HUD;
    public Image screenEffect;
    public Image crosshair;

    private void OnEnable()
    {
        Player.OnLanding += LandingBob;
    }

    private void OnDisable()
    {
        Player.OnLanding -= LandingBob;
    }

    void Update()
    {
        cameraX += Player.Instance.inputs.GetAxisRaw("Mouse X") * cameraSensitivity;
        cameraY -= Player.Instance.inputs.GetAxisRaw("Mouse Y") * cameraSensitivity;

        cameraY = Mathf.Clamp(cameraY, -90, 90);

        rotor.localRotation = Quaternion.Euler(Vector3.up * cameraX);

        pivot.localRotation = Quaternion.Euler(Vector3.right * cameraY);

        float movementX = -Player.Instance.inputs.GetAxisRaw("Mouse X") * amount;
        float movementY = -Player.Instance.inputs.GetAxisRaw("Mouse Y") * amount;
        movementX = Mathf.Clamp(movementX, -maxAmout, maxAmout);
        movementY = Mathf.Clamp(movementY, -maxAmout, maxAmout);

        if (Player.Instance.cc.isGrounded)
        {
            /*
            float x = 0, y = 0;
            y += Player.Instance.cc.velocity.magnitude / 100 * Mathf.Sin(((Player.Instance.move.jogSpeed + 4) * 2) * Time.time);
            x += Player.Instance.cc.velocity.magnitude / 100 * Mathf.Sin((Player.Instance.move.jogSpeed + 4) * Time.time);
            worldCam.localPosition = Vector3.Lerp(worldCam.localPosition, new Vector3(x, worldCam.localPosition.z, y), 0.5f);
            */
        }

        fallDistance = Vector3.up * Mathf.Clamp(fallDistance.y, -1, 1);
        fallDistance = Vector3.Lerp(fallDistance, Vector3.zero, Time.deltaTime * 10);
        kickBack = Vector3.Lerp(kickBack, Vector3.zero, Time.deltaTime * 10);


        worldCam.localRotation = Quaternion.Slerp(worldCam.localRotation, Quaternion.Euler(kickBack), Time.deltaTime * 50);

        viewCam.localPosition = Vector3.Lerp(viewCam.localPosition, new Vector3(movementX, movementY, 0), Time.deltaTime * smoothAmout);

        transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.up * (Player.Instance.cc.height) + fallDistance, Time.deltaTime * 10);

        HudPart();
    }

    private void LandingBob()
    {
        fallDistance = Vector3.up * Player.Instance.cc.velocity.y;
    }

    public void AddPistolKickBack(float kickAmount, float dispersion = 2)
    {
        kickBack = transform.InverseTransformDirection(Vector3.right) * kickAmount + transform.InverseTransformDirection(Vector3.up) * UnityEngine.Random.Range(-dispersion, dispersion);
    }

    private void HudPart()
    {
        if (screenEffect.color.a > 0)
        {
            Color color = screenEffect.color;
            color.a -= Time.deltaTime * 1.5f;
            screenEffect.color = color;
        }
    }
}