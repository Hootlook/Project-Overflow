using UnityEngine;

public class WeaponBase : MonoBehaviour, IEntityBase
{
    [Header("Weapon Base")]

    public int id;
    public bool isShooting;
    public bool isReloading;
    public bool isArmed;
    public bool isEquipped;

    public Rigidbody rb;
    public AudioSource a;

    public float timer = 1;
    public float counter = 0;

    public AudioClip[] sound;

    void Awake()
    {
        if (transform.parent == null) enabled = false;

        a = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
    }

    public virtual void Update()
    {
        if (counter < timer) counter += Time.deltaTime;
    }

    public void OnInteract()
    {
        transform.SetParent(Player.Instance.weaponManager.rightHolder);
        Player.Instance.weaponManager.SelectWeapon(transform.GetSiblingIndex());
        Player.Instance.weaponManager.SetConstaints(transform);
        rb.isKinematic = true;
        isEquipped = true;
        enabled = true;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        gameObject.layer = LayerMask.NameToLayer("View Model");
        transform.Find("Collisions").gameObject.SetActive(false);

        foreach (Transform t in transform)  
        {
            t.gameObject.layer = LayerMask.NameToLayer("View Model");
        }

        WeaponSpecificSetup();
    }

    public virtual void WeaponSpecificSetup() { }

    protected bool canShoot()
    {
        if (counter >= timer) return true;
        else return false;
    }
}
