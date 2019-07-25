using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private static Player _instance;
    public static Player Instance => _instance;

    [HideInInspector]
    public CharacterController cc;
    [HideInInspector]
    public PlayerMovements move;
    [HideInInspector]
    public PlayerInputs inputs;
    [HideInInspector]
    public PlayerCamera cam;
    [HideInInspector]
    public WeaponManager weaponManager;
    [HideInInspector]
    public Animator armsAnim;

    public AudioClip[] sound;
    public List<AudioClip> footSteps;

    public float footStepTiming;
    int randomFootStep;

    public delegate void PlayerEvent();
    public static event PlayerEvent OnLanding;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Update()
    {
        if (Instance.cc.isGrounded)
        {
            StepsSound();

            if (Instance.move.hasLanded)
            {
                OnLanding?.Invoke();
                Instance.move.hasLanded = false;
            }
        }
        else
        {
            if (!Instance.move.hasLanded) Instance.move.hasLanded = true;
        }

    }

    void Start()
    {
        move = GetComponent<PlayerMovements>();
        inputs = GetComponent<PlayerInputs>();
        cam = GetComponentInChildren<PlayerCamera>();
        cc = GetComponent<CharacterController>();
        weaponManager = GetComponent<WeaponManager>();
        armsAnim = weaponManager.arms.GetComponent<Animator>();
    }

    private void StepsSound()
    {
        if(Player.Instance.cc.velocity.magnitude > 0)
        {
            if (footStepTiming < 85)
            {
                footStepTiming += Time.deltaTime + Instance.cc.velocity.magnitude;
            }
            else
            {
                if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit, 1.5f, ~(1 << LayerMask.NameToLayer("Player"))))
                {
                    randomFootStep = UnityEngine.Random.Range(1, 5);

                    if (footSteps.Find(clip => clip.name == hit.transform.tag + randomFootStep) == null)
                    {
                        FxManager.EmitSound(footSteps.Find(clip => clip.name == "concrete" + randomFootStep), true);
                    }
                    else
                    {
                        FxManager.EmitSound(footSteps.Find(clip => clip.name == hit.transform.tag + randomFootStep), true);
                    }

                    footStepTiming = 0;
                }
            }
        }
    }
}

