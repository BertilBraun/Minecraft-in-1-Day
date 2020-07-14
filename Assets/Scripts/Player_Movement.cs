using Assets.Minecraft;
using UnityEngine;

class Player_Movement : MonoBehaviour
{
    public float speed = 12f;
    public float gravity = -9.81f;
    public float jumpForce = 1.5f;

    public World world;

    bool isGrounded = false;
    float yVel = 0f;

    void Update()
    {
        bool sprinting = Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.LeftShift);

        yVel += gravity * Time.deltaTime;
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
            yVel = jumpForce;

        Vector3 updateVel = (transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical") * (sprinting ? 2f : 1f)) * speed;
        
        updateVel += new Vector3(0f, yVel, 0f);
        updateVel *= Time.deltaTime;

        transform.position += new Vector3(0, updateVel.y, 0);
        if (Collision())
        {
            transform.position -= new Vector3(0, updateVel.y, 0);
            yVel = -2f * Time.deltaTime;
        }

        transform.position += new Vector3(updateVel.x, 0, 0);
        if (Collision())
            transform.position -= new Vector3(updateVel.x, 0, 0);

        transform.position += new Vector3(0, 0, updateVel.z);
        if (Collision())
            transform.position -= new Vector3(0, 0, updateVel.z);

        if (transform.position.y < -20)
            world.SpawnPlayer();
    }

    bool Collision()
    {
        Vector3 dim = new Vector3(0.8f, 1.2f, 0.8f);
        Vector3 min = transform.position - dim / 2;
        Vector3 max = transform.position + dim / 2;

        for (int x = (int)min.x; x <= (int)max.x; x++)
            for (int y = (int)(min.y - 0.3f); y <= (int)max.y; y++)
                for (int z = (int)min.z; z <= (int)max.z; z++)
                    if (BlockDictionary.Get(world.GetBlock(x, y, z)).Solid)
                    {
                        if (y == (int)(min.y - 0.3f))
                            isGrounded = true;
                        return true;
                    }
        return false;
    }
}