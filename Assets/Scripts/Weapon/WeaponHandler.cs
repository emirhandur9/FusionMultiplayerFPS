using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class WeaponHandler : NetworkBehaviour
{
    [Networked(OnChanged = nameof(OnFireChanged))]
    public bool isFiring { get; set; }

    //[Networked(OnChanged = nameof(OnDeathChanged))]
    //public int death { get; set; }

    public ParticleSystem fireParticle;
    public Transform aimPoint;
    public LayerMask collisionLayers;

    float lastTimeFired = 0f;

    HPHandler hpHandler;
    NetworkPlayer networkPlayer;

    //public const int DefaultAmmoCount = 8;
    public const int DefaultMagazineCount = 4;
    public const int MagazineInAmmoMax = 8;

    [SerializeField] AmmoUIHandler ammoUIHandler;
    public int AmmoInMagazineCount { get; set; }
    public int MagazineCount { get; set; }

    public int TotalAmmoCount { get; set; }
    public bool IsReloading { get; set; }

    [SerializeField] private GameObject casing;
    [SerializeField] Transform casingSpawn;
    [SerializeField] Vector3 casingForce;
    [SerializeField] ParticleSystem[] shootParticle;
    [SerializeField] Animator pistolAnimator;

    private void Awake()
    {
        hpHandler = GetComponent<HPHandler>();
        networkPlayer = GetComponent<NetworkPlayer>();

    }
    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            SetupWeaponAndAmmos();
        }
    }
    public override void FixedUpdateNetwork()
    {
        //if (!Object.HasStateAuthority) return;
        //BURADA BUNUN OLMASI LAZIM DEGIL CUNKU BURASININ HER CLIENTTE CAGRILMASI GEREKIYO, VISUAL ICIN SAYILIR
        //OBJECT STATE AUTHORITY FIRE'IN ICINDE YAPILIYOR
        if (hpHandler.isDead)
            return;

        if (GetInput(out NetworkInputData networkInputData))
        {
            if (networkInputData.isFireButtonPressed)
            {
                Fire(networkInputData.aimForwardVector);
            }
        }
    }
    public void Fire(Vector3 aimForwardVector)
    {
        if (IsReloading) return;

        if (Time.time - lastTimeFired < 0.15f)
            return;

        if (TotalAmmoCount <= 0 && MagazineCount <= 0) return; //No ammo and magazine left

        TotalAmmoCount--;
        AmmoInMagazineCount--;
        ammoUIHandler.UpdateAmmoCountText(AmmoInMagazineCount);

        CheckForReload();

        StartCoroutine(FireEffectCO());

        Runner.LagCompensation.Raycast(aimPoint.position, aimForwardVector, 100, Object.InputAuthority, out var hitInfo, collisionLayers, HitOptions.IncludePhysX);

        float hitDistance = 100;
        bool isHitOtherPlayer = false;

        if (hitInfo.Distance > 0)
        {
            hitDistance = hitInfo.Distance;
        }

        if (hitInfo.Hitbox != null)
        {
            isHitOtherPlayer = true;

            if (Object.HasStateAuthority)
            {
                Debug.Log(hitInfo.Hitbox.transform.name);
                if (hitInfo.Hitbox.transform.root.CompareTag("Player"))
                {
                    hitInfo.Hitbox.transform.root.GetComponent<HPHandler>().OnTakeDamage(networkPlayer.nickname.ToString(), hpHandler);
                }
                else if (hitInfo.Hitbox.transform.root.CompareTag("NPC"))
                {
                    Debug.Log("npc founded");
                    hitInfo.Hitbox.transform.root.GetComponent<NPCController>().Kill();
                }

            }
        }
        else if (hitInfo.Collider != null)
        {

        }

        if (isHitOtherPlayer)
        {
            Debug.DrawRay(aimPoint.position, aimForwardVector * hitDistance, Color.red, 1);
        }
        else
        {
            Debug.DrawRay(aimPoint.position, aimForwardVector * hitDistance, Color.green, 1);
        }

        lastTimeFired = Time.time;

    }

    IEnumerator FireEffectCO()
    {
        isFiring = true;

        GameObject c = Instantiate(casing, casingSpawn.position, casingSpawn.rotation);
        c.GetComponent<Rigidbody>().AddForce(casingSpawn.transform.TransformDirection(casingForce * Random.Range(0.9f, 1.1f)));
        c.transform.SetParent(transform);
        pistolAnimator.CrossFade("Shoot", 0.02f, 0, 0);
        for (int i = 0; i < shootParticle.Length; i++)
        {
            shootParticle[i].Emit(10);
        }

        yield return new WaitForSeconds(0.09f);

        isFiring = false;
    }
    public static void OnFireChanged(Changed<WeaponHandler> changed)
    {
        bool isFiringCurrent = changed.Behaviour.isFiring;

        changed.LoadOld();

        bool isFiringOld = changed.Behaviour.isFiring;

        if (isFiringCurrent && !isFiringOld)
        {
            changed.Behaviour.OnFireRemote();
        }
    }

    public void OnFireRemote()
    {
        if (!Object.InputAuthority)
            fireParticle.Play();
    }

    public void SetupWeaponAndAmmos()
    {
        MagazineCount = DefaultMagazineCount;
        TotalAmmoCount = MagazineInAmmoMax * MagazineCount;
        AmmoInMagazineCount = 0;
        Reload();
    }

    private void CheckForReload()
    {
        if (AmmoInMagazineCount <= 0 && MagazineCount > 0)
        {
            Reload();
        }

        if (AmmoInMagazineCount <= 0 && MagazineCount <= 0 && TotalAmmoCount > 0)
        {
            Reload();
            //AmmoInMagazineCount = TotalAmmoCount;
            //ammoUIHandler.UpdateAmmoCountText(AmmoInMagazineCount);
        }
    }

    public void Reload()
    {

        if (MagazineCount > 0)
        {
            MagazineCount--;
            AmmoInMagazineCount = TotalAmmoCount >= MagazineInAmmoMax ? MagazineInAmmoMax : TotalAmmoCount;
            ammoUIHandler.UpdateMagazineCountText(MagazineCount);
            ammoUIHandler.UpdateAmmoCountText(AmmoInMagazineCount);
            IsReloading = true;
        }
        else if (TotalAmmoCount > 0)
        {
            AmmoInMagazineCount = TotalAmmoCount;
            ammoUIHandler.UpdateAmmoCountText(AmmoInMagazineCount);
            IsReloading = true;
        }
        pistolAnimator.SetBool("reloading", IsReloading);
    }


}
