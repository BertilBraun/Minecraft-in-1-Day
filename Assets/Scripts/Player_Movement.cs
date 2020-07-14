using Assets.Minecraft;
using Assets.Scripts;
using UnityEngine;

class Player_Movement : MonoBehaviour
{
    public float speed = 12f;
    public float gravity = -9.81f;
    public float jumpForce = 1.5f;

    public World world;

    public bool isFlying = false;
    public bool isGrounded = false;
    public float yVel = 0f;

    void Start()
    {
        PlayerCollider.Init(world, transform);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            isFlying = !isFlying;

        if (!isFlying)
            UpdateWalking();
        else
            UpdateFlying();

        if (transform.position.y < -20)
            world.SpawnPlayer();
    }

    void UpdateFlying()
    {
        Vector3 updateVel = (transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical")) * speed * 5f;
        
        if (Input.GetKey(KeyCode.LeftControl))
            updateVel += new Vector3(0f, -10f, 0f);
        else if (Input.GetKey(KeyCode.Space))
            updateVel += new Vector3(0f, 10f, 0f);

        transform.position += updateVel * Time.deltaTime;
    }

    void UpdateWalking()
    {
        bool sprinting = Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.LeftShift);

        yVel += gravity * Time.deltaTime;
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
            yVel = jumpForce;

        Vector3 updateVel = (transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical") * (sprinting ? 2f : 1f)) * speed;

        updateVel += new Vector3(0f, yVel, 0f);
        updateVel *= Time.deltaTime;

        transform.position += new Vector3(0, updateVel.y, 0);
        if (PlayerCollider.Collision(out isGrounded))
        {
            transform.position -= new Vector3(0, updateVel.y, 0);
            yVel = -2f * Time.deltaTime;
        }

        transform.position += new Vector3(updateVel.x, 0, 0);
        if (PlayerCollider.Collision(out bool _))
            transform.position -= new Vector3(updateVel.x, 0, 0);

        transform.position += new Vector3(0, 0, updateVel.z);
        if (PlayerCollider.Collision(out bool _))
            transform.position -= new Vector3(0, 0, updateVel.z);
    }
}