using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

public class CharacterMovementHandler : NetworkBehaviour
{

    bool isSpawnRequested;

    NetworkCharacterControllerPrototypeCustom networkCharacterControllerPrototypeCustom;
    Camera localCamera;
    HPHandler hpHandler;
    NetworkInGameMessages networkInGameMessages;
    NetworkPlayer networkPlayer;

    private void Awake()
    {
        networkCharacterControllerPrototypeCustom = GetComponent<NetworkCharacterControllerPrototypeCustom>();
        localCamera = GetComponentInChildren<Camera>();
        hpHandler = GetComponent<HPHandler>();
        networkInGameMessages = GetComponent<NetworkInGameMessages>();
        networkPlayer = GetComponent<NetworkPlayer>();
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority) return;
        if (isSpawnRequested)
        {
            Respawn();
            return;
        }
        if (hpHandler.isDead) return;


        if (GetInput(out NetworkInputData networkInputData))
        {
            transform.forward = networkInputData.aimForwardVector;

            Quaternion rotation = transform.rotation;
            rotation.eulerAngles = new Vector3(0, rotation.eulerAngles.y, rotation.eulerAngles.z);
            transform.rotation = rotation;

            Vector3 moveDirection = transform.forward * networkInputData.movementInput.y + transform.right * networkInputData.movementInput.x;
            moveDirection.Normalize();

            networkCharacterControllerPrototypeCustom.Move(moveDirection);

            if (networkInputData.isJumpPressed) networkCharacterControllerPrototypeCustom.Jump();

            CheckFallRespawn();
        }
    }


    public void CheckFallRespawn()
    {
        if(transform.position.y < -12)
        {
            if (Object.HasStateAuthority)
            {
                networkInGameMessages.SendInGameMessages(networkPlayer.nickname.ToString(), "fell of the world xddd");
                Respawn();
            }

        }
    }

    public void SetCharacterControllerEnabled(bool isEnabled)
    {
        networkCharacterControllerPrototypeCustom.Controller.enabled = isEnabled;
    }

    public void RequestSpawn()
    {
        isSpawnRequested = true;
    }
    private void Respawn()
    {
        networkCharacterControllerPrototypeCustom.TeleportToPosition(Utils.GetRandomSpawnPoint());
        isSpawnRequested = false;
        hpHandler.OnRespawned();

    }
}
