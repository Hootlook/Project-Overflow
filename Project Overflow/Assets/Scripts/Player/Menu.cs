using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public GameObject menu;
    bool menuState;

    private void Update()
    {
        if (Input.GetButtonDown("Cancel")) menuState = !menuState;

        menu.SetActive(menuState);

        if (menu.activeSelf == true)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            Player.Instance.inputs.isDisabled = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Player.Instance.inputs.isDisabled = false;
        }
    }
}