using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolHandler : MonoBehaviour
{
    WeaponHandler weaponHandler;
    Animator animator;
    private void Awake()
    {
        weaponHandler = GetComponentInParent<WeaponHandler>();
        animator = GetComponent<Animator>();
    }

    public void ReloadAnimCompleted()
    {
        weaponHandler.IsReloading = false;
        animator.SetBool("reloading", false);

    }
}
