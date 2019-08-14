using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour, IEntityBase
{
    public TimeScaler machine;
    public AudioClip[] sound;
    public GameObject sun;
    private bool toggle;
    private bool rotating;

    public void OnInteract()
    {
        if(transform.name == "btn +")
        {
            Time.timeScale += 0.02f;
        }
        if(transform.name == "btn -")
        {
            Time.timeScale -= 0.02f;
        }
        if(transform.name == "sun")
        {
            toggle = !toggle;

            if (toggle)
            {
                StartCoroutine(rotateObject(sun, Quaternion.Euler(Vector3.right * 100), 5));
            }
            else
            {
                StartCoroutine(rotateObject(sun, Quaternion.Euler(Vector3.right * -100), 5));
            }
        }

        FxManager.EmitSound(sound[0], false);
        machine.text.text = Math.Round(Time.timeScale,2).ToString();
    }

    private IEnumerator TimeOfday(float v)
    {
        float timer = 10;
        float counter = 0;

        while (counter < timer)
        {
            counter += Time.deltaTime;

            sun.transform.rotation = Quaternion.Slerp(sun.transform.rotation, Quaternion.Euler(Vector3.right * v), counter / timer);

            yield return null;

        }
    }

    IEnumerator rotateObject(GameObject gameObjectToMove, Quaternion newRot, float duration)
    {
        if (rotating)
        {
            yield break;
        }
        rotating = true;

        Quaternion currentRot = gameObjectToMove.transform.rotation;

        float counter = 0;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            gameObjectToMove.transform.rotation = Quaternion.Lerp(currentRot, newRot, counter / duration);
            yield return null;
        }
        rotating = false;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
