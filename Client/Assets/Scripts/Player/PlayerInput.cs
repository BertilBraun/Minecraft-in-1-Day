using System;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform cameraTransform;

    private float cameraVerticalRotation;
    private float playerHorizontalRotation;

    [HideInInspector]
    public bool isFlying = false;

    void Start()
    {
        // TODO reenable SetCursor(false);
        cameraVerticalRotation = cameraTransform.localEulerAngles.x;
        playerHorizontalRotation = transform.eulerAngles.y;
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SetCursor(!Cursor.visible);

        if (Cursor.lockState == CursorLockMode.Locked)
            Look();

        if (Input.GetKeyDown(KeyCode.F))
            isFlying = !isFlying;

        bool[] _inputs = new bool[]
           {
            Input.GetKey(KeyCode.W),
            Input.GetKey(KeyCode.S),
            Input.GetKey(KeyCode.A),
            Input.GetKey(KeyCode.D),
            Input.GetKeyDown(KeyCode.F),
            Input.GetKey(KeyCode.Space),
            Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift),
            Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)
           };

        int scrollDeltaSign = Math.Sign(Input.mouseScrollDelta.y);
        int mWheelInput = scrollDeltaSign < 0 ? 2 : scrollDeltaSign;

        PacketSender.PlayerInput(_inputs, (byte)mWheelInput);
    }
    
    // TODO Rotation might not work because of player/camera parenting
    void Look()
    {
        float mouseVertical = -Input.GetAxis("Mouse Y");
        float mouseHorizontal = Input.GetAxis("Mouse X");

        cameraVerticalRotation += mouseVertical * mouseSensitivity * Time.fixedDeltaTime;
        playerHorizontalRotation += mouseHorizontal * mouseSensitivity * Time.fixedDeltaTime;

        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(cameraVerticalRotation, 0f, 0f);
        transform.rotation = Quaternion.Euler(0f, playerHorizontalRotation, 0f);
    }

    void SetCursor(bool visible)
    {
        if (!visible)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;

        Cursor.visible = visible;
    }
}
