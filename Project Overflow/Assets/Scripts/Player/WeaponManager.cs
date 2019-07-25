using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class WeaponManager : MonoBehaviour
{
	public int selectedWeapon = 0;
    public int weaponId;
	public Transform rightHolder;
    public Transform[] handBones;
    public Transform arms;
    public WeaponBase currentWeapon;
    public ParentConstraint parentConstraint;
    private ConstraintSource source;
    private ParentConstraint weaponContraint;
    private float tilting;

    void Start()
    {
        SelectWeapon(selectedWeapon);
    }

    void Update()
    {
        if (Player.Instance.cc.isGrounded)
        {
            float x = 0, y = 0;
            y += Player.Instance.cc.velocity.magnitude / 800 * Mathf.Sin(((Player.Instance.move.currentSpeed + 4) * 2) * Time.time);
            x += Player.Instance.cc.velocity.magnitude / 800 * Mathf.Sin((Player.Instance.move.currentSpeed + 4) * Time.time);
            arms.localPosition = Vector3.Lerp(arms.localPosition, new Vector3(x, arms.localPosition.z, y), Time.deltaTime * 50);
            tilting = Mathf.Lerp(tilting, -arms.InverseTransformDirection(Player.Instance.cc.velocity).x, Time.deltaTime * 10);
            arms.localRotation = Quaternion.Lerp(arms.localRotation, Quaternion.Euler(Vector3.forward * tilting), Time.deltaTime * 50);
        }

        int previousSelectedWeapon = selectedWeapon;

        if (Player.Instance.inputs.GetAxis("MouseWheel") > 0f)
        {
            if (selectedWeapon >= rightHolder.childCount - 1) selectedWeapon = 0;
            else selectedWeapon++;
        }

        if (Player.Instance.inputs.GetAxis("MouseWheel") < 0f)
        {
            if (selectedWeapon <= 0) selectedWeapon = rightHolder.childCount - 1;
            else selectedWeapon--;
        }

        if (previousSelectedWeapon != selectedWeapon)
        {
            SelectWeapon(selectedWeapon);
        }

        if (Player.Instance.inputs.GetButtonDown("Interact"))
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 2, ~(1 << LayerMask.NameToLayer("Player"))))  
            {
                if (hit.collider.GetComponentInParent<IEntityBase>() != null)
                {
                    hit.collider.GetComponentInParent<IEntityBase>().OnInteract();
                }
            }
        }
    }

    public void SelectWeapon(int selectedWeapon)
	{
		int i = 0;
		foreach (Transform weapon in rightHolder)
		{
			if (i == selectedWeapon)
			{
                weapon.gameObject.SetActive(true);
                currentWeapon = weapon.GetComponent<WeaponBase>();
                weaponId = currentWeapon.id;
                Player.Instance.armsAnim.SetInteger("weaponId", weaponId);
                Player.Instance.armsAnim.Play("Ref");
            }
			else weapon.gameObject.SetActive(false);
			i++;
		}
	}

    public void SetConstaints(Transform weapon)
    {
        foreach (Transform handBone in handBones)
        {
            string boneName = handBone.name.Remove(handBone.name.Length - 2);

            Transform weaponBone = weapon.Find(weapon.name);

            while (weaponBone.name != boneName)
            {
                if (weaponBone.childCount > 0) weaponBone = weaponBone.GetChild(0);
                else
                {
                    try
                    {
                        weaponBone = weaponBone.parent.GetChild(weaponBone.GetSiblingIndex() + 1);
                    }
                    catch (Exception)
                    {
                        weaponBone = null;
                        break;
                    }
                }
            }

            if (weaponBone == null) continue;

            if (weaponBone.GetComponent<ParentConstraint>() == null)
            {
                weaponContraint = weaponBone.gameObject.AddComponent<ParentConstraint>();
            }

            source.sourceTransform = handBone;

            source.weight = 1;

            weaponContraint.AddSource(source);

            weaponContraint.constraintActive = true;
        }
    }
}
