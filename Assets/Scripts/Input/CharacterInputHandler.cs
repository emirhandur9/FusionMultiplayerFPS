using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInputHandler : MonoBehaviour
{
    LocalCameraHandler localCameraHandler;
    CharacterMovementHandler characterMovementHandler;
    WeaponHandler weaponHandler;

    Vector2 moveInputVector = Vector2.zero;
    Vector2 viewInputVector = Vector2.zero;
    bool isJumpButtonPressed;
    bool isFireButtonPressed;

    private void Awake()
    {
        localCameraHandler = GetComponentInChildren<LocalCameraHandler>();
        characterMovementHandler = GetComponent<CharacterMovementHandler>();
        weaponHandler = GetComponent<WeaponHandler>();
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Update()
    {
        if (!characterMovementHandler.Object.HasInputAuthority) return;

        viewInputVector.x = Input.GetAxis("Mouse X");
        viewInputVector.y = Input.GetAxis("Mouse Y") * -1f;

        moveInputVector.x = Input.GetAxis("Horizontal");
        moveInputVector.y = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Space))
            isJumpButtonPressed = true;

        if (Input.GetMouseButtonDown(0))
            isFireButtonPressed = true;

        localCameraHandler.SetViewInput(viewInputVector);


        if (Input.GetKeyDown(KeyCode.R) && !weaponHandler.IsReloading) //No need for synch
            weaponHandler.Reload();
    }

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();

        networkInputData.aimForwardVector = localCameraHandler.transform.forward;
        networkInputData.movementInput = moveInputVector;
        networkInputData.isJumpPressed = isJumpButtonPressed;
        networkInputData.isFireButtonPressed = isFireButtonPressed;

        //reset
        isJumpButtonPressed = false;
        isFireButtonPressed = false;
        return networkInputData;
    }
}
