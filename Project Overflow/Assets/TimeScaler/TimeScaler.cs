using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeScaler : MonoBehaviour
{
    public TextMeshPro text;
    public GameObject button1;
    public GameObject button0;

    void Start()
    {
        text.text = Time.timeScale.ToString();
    }

    void Update()
    {
    }
}
