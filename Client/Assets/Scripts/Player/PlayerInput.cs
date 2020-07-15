using Assets.Scripts.Player;
using System;
using UnityEngine;

public struct InputData
{
    public readonly bool[] inputs;
    public readonly byte mWheel;

    public InputData(bool init = false)
    {
        this.inputs = new bool[8] { false, false, false, false, false, false, false, false };
        this.mWheel = 0;
    }
    public InputData(bool[] inputs, byte mWheel)
    {
        this.inputs = inputs;
        this.mWheel = mWheel;
    }
}

public class PlayerInput : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform cameraTransform;

    private float cameraVerticalRotation;
    private float playerHorizontalRotation;

    public bool isFlying = false;

    void Start()
    {
        // TODO reenable SetCursor(false);
        cameraVerticalRotation = cameraTransform.localEulerAngles.x;
        playerHorizontalRotation = transform.eulerAngles.y;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            isFlying = !isFlying;

        if (Input.GetKeyDown(KeyCode.Escape))
            SetCursor(!Cursor.visible);
    }
    private void FixedUpdate()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
            Look();

        int scrollDeltaSign = Math.Sign(Input.mouseScrollDelta.y);
        int mWheelInput = scrollDeltaSign < 0 ? 2 : scrollDeltaSign;

        InputData data = new InputData(new bool[]
           {
            Input.GetKey(KeyCode.W),
            Input.GetKey(KeyCode.S),
            Input.GetKey(KeyCode.A),
            Input.GetKey(KeyCode.D),
            isFlying,
            Input.GetKey(KeyCode.Space),
            Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift),
            Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)
           }, (byte)mWheelInput);

        PacketSender.PlayerInput(data);
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
