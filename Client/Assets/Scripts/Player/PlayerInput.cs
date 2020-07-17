using System;
using UnityEngine;

public class InputData
{
    public bool[] inputs = new bool[8];
    public int mWheel = 0;
    public Quaternion rotation = Quaternion.identity;
}

public class PlayerInput : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform cameraTransform;

    private float cameraVerticalRotation;
    private float playerHorizontalRotation;

    public InputData data = new InputData();

    void Start()
    {
        SetCursor(false);
        cameraVerticalRotation = cameraTransform.localEulerAngles.x;
        playerHorizontalRotation = transform.eulerAngles.y;
    }

    private void Update()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
            Look();

        if (Input.GetKeyDown(KeyCode.F))
            data.inputs[4] = true;

        if (Input.GetKeyDown(KeyCode.Escape))
            SetCursor(!Cursor.visible);

        if (data.mWheel == 0)
            data.mWheel = Math.Sign(Input.mouseScrollDelta.y);
    }
    private void FixedUpdate()
    {
        data.inputs[0] = Input.GetKey(KeyCode.W);
        data.inputs[1] = Input.GetKey(KeyCode.S);
        data.inputs[2] = Input.GetKey(KeyCode.A);
        data.inputs[3] = Input.GetKey(KeyCode.D);

        data.inputs[5] = Input.GetKey(KeyCode.Space);
        data.inputs[6] = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        data.inputs[7] = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

        data.rotation = cameraTransform.rotation;
        PacketSender.PlayerInput(data);
        data = new InputData();
    }
    
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
