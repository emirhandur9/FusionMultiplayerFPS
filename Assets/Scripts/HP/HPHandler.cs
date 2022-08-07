using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using System;

public class HPHandler : NetworkBehaviour
{
    [Networked(OnChanged = nameof(OnHPChanged))]
    byte HP { get; set; }

    [Networked(OnChanged = nameof(OnStateChanged))]
    public bool isDead { get; set; }


    [Networked(OnChanged = nameof(OnDeathCountChanged))]
    public int deathCount { get; set; }


    [Networked(OnChanged = nameof(OnKillCountChanged))]
    public int killCount { get; set; }


    bool isInitialized = false;
    const byte startingHP = 5;

    public Color onHitColor;
    public Image hitImage;

    public MeshRenderer bodyMeshRenderer;
    Color defaultMeshBodyColor;

    public GameObject playerModel;
    public GameObject deathGameobjectPrefab;

    HitboxRoot hitboxRoot;
    CharacterMovementHandler characterMovementHandler;

    NetworkInGameMessages networkInGameMessages;
    NetworkPlayer networkPlayer;

    WeaponHandler weaponHandler;
    private void Awake()
    {
        characterMovementHandler = GetComponent<CharacterMovementHandler>();
        hitboxRoot = GetComponentInChildren<HitboxRoot>();
        networkPlayer = GetComponent<NetworkPlayer>();
        networkInGameMessages = GetComponent<NetworkInGameMessages>();
        weaponHandler = GetComponent<WeaponHandler>();
    }
    private void Start()
    {
        HP = startingHP;
        isDead = false;
        defaultMeshBodyColor = bodyMeshRenderer.material.color;
        isInitialized = true;
    }

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            deathCount = 0;
        }
        else
        {
            Debug.Log(networkPlayer.nickname.ToString() + " " + deathCount);
        }
    }

    IEnumerator OnHitCO()
    {
        bodyMeshRenderer.material.color = Color.red;

        if(Object.HasInputAuthority)
        {
            hitImage.color = onHitColor;
        }

        yield return new WaitForSeconds(.2f);

        bodyMeshRenderer.material.color = defaultMeshBodyColor;
        if(Object.HasInputAuthority && !isDead)
        {
            hitImage.color = new Color(0, 0, 0, 0);
        }
    }

    public void OnTakeDamage(string damageCausedByPlayerNickname, HPHandler killer)
    {
        if (isDead) return;

        HP -= 1;


        if(HP <= 0)
        {
            networkInGameMessages.SendInGameMessages(damageCausedByPlayerNickname, $"Killed <b> {networkPlayer.nickname.ToString()} </b>");
            StartCoroutine(ServerReviveCO());
            isDead = true;
            deathCount++;
            killer.KilledSomebody();
        }
    }

    private void KilledSomebody()
    {
        killCount++;
    }

    static void OnHPChanged(Changed<HPHandler> changed)
    {
        byte newHP = changed.Behaviour.HP;

        changed.LoadOld();

        byte oldHP = changed.Behaviour.HP;


        if(newHP < oldHP)
        {
            changed.Behaviour.OnHpReduced();
        }
    }
    private void OnHpReduced()
    {
        if (!isInitialized) return;

        StartCoroutine(OnHitCO());

    }

    private IEnumerator ServerReviveCO()
    {
        yield return new WaitForSeconds(2.0f);
        characterMovementHandler.RequestSpawn();
    }
    static void OnStateChanged(Changed<HPHandler> changed)
    {
        bool isDeadCurrent = changed.Behaviour.isDead;

        changed.LoadOld();

        bool isDeadOld = changed.Behaviour.isDead;

        if (isDeadCurrent)
            changed.Behaviour.OnDeath();
        else if (!isDeadCurrent && isDeadOld)
            changed.Behaviour.OnRevive();
    }
    private void OnDeath()
    {
        playerModel.gameObject.SetActive(false);
        hitboxRoot.HitboxRootActive = false;
        characterMovementHandler.SetCharacterControllerEnabled(false);
        Instantiate(deathGameobjectPrefab, transform.position, Quaternion.identity);

    }

    private void OnRevive()
    {
        if (Object.HasInputAuthority)
            hitImage.color = new Color(0, 0, 0, 0);

        playerModel.gameObject.SetActive(true);
        hitboxRoot.HitboxRootActive = true;
        characterMovementHandler.SetCharacterControllerEnabled(true);
    }

    public void OnRespawned()
    {
        HP = startingHP;
        isDead = false;
        weaponHandler.SetupWeaponAndAmmos();
    }

    public static void OnDeathCountChanged(Changed<HPHandler> changed)
    {
        Debug.Log("death count changed");
        changed.Behaviour.DeathCountChangedRemote();
    }

    public void DeathCountChangedRemote()
    {
        NetworkPlayer.Local.localScoreboard.UpdateDeath(networkPlayer.nickname.ToString());
    }

    public static void OnKillCountChanged(Changed<HPHandler> changed)
    {
        changed.Behaviour.KillCountChangedRemote();
    }

    public void KillCountChangedRemote()
    {
        NetworkPlayer.Local.localScoreboard.UpdateKill(networkPlayer.nickname.ToString());
    }

}
