using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalCameraHandler : MonoBehaviour
{
    public Transform cameraAnchorPoint;

    Vector2 viewInput;

    float cameraRotationX;
    float cameraRotationY;

    NetworkCharacterControllerPrototypeCustom networkCharacterControllerPrototypeCustom; 
    Camera localCamera;
    private void Awake()
    {
        localCamera = GetComponent<Camera>();
        networkCharacterControllerPrototypeCustom = GetComponentInParent<NetworkCharacterControllerPrototypeCustom>();
    }

    private void Start()
    {
        if (localCamera.enabled)
        {
            localCamera.transform.parent = null;
        }
    }

    private void LateUpdate()
    {
        if (cameraAnchorPoint == null) return;

        if (!localCamera.enabled) return;

        localCamera.transform.position = cameraAnchorPoint.position;

        cameraRotationX += viewInput.y * Time.deltaTime * networkCharacterControllerPrototypeCustom.viewUpDownRotationSpeed;
        cameraRotationX = Mathf.Clamp(cameraRotationX, -90, 90);

        cameraRotationY += viewInput.x * Time.deltaTime * networkCharacterControllerPrototypeCustom.rotationSpeed;

        localCamera.transform.rotation = Quaternion.Euler(cameraRotationX, cameraRotationY, 0);
    }

    public void SetViewInput(Vector2 viewInput)
    {
        this.viewInput = viewInput;
    }
}
