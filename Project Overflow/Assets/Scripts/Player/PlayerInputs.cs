using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputs : MonoBehaviour
{
    public bool isDisabled;

    public bool GetButton(string buttonName)
    {
        if (isDisabled) return false;
        return Input.GetButton(buttonName);
    }

    public bool GetButtonDown(string buttonName)
    {
        if (isDisabled) return false;
        return Input.GetButtonDown(buttonName);
    }

    public bool GetButtonUp(string buttonName)
    {
        if (isDisabled) return false;
        return Input.GetButtonUp(buttonName);
    }

    public float GetAxis(string axisName)
    {
        if (isDisabled) return 0;
        return Input.GetAxis(axisName);
    }

    public float GetAxisRaw(string axisName)
    {
        if (isDisabled) return 0;
        return Input.GetAxisRaw(axisName);
    }
}