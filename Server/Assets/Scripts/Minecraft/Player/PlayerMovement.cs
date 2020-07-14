using Assets.Scripts.Minecraft.WorldManage;
using System;
using UnityEngine;

namespace Assets.Scripts.Minecraft.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        public float speed = 8f;
        public float gravity = -9.81f;
        public float jumpForce = 1.5f;

        [HideInInspector]
        public bool isFlying = false;
        private bool isGrounded = false;
        private float yVel = 0f;

        public bool[] inputs;
        public Guid id;

        public void Init(Guid _id)
        {
            id = _id;
        }

        private void Start()
        {
            gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
            speed *= Time.fixedDeltaTime;
            jumpForce *= Time.fixedDeltaTime;

            inputs = new bool[8];
        }

        /// <summary>Processes player input and moves the player.</summary>
        public void FixedUpdate()
        {
            if (inputs[4])
                isFlying = !isFlying;

            Vector2 inputDirection = Vector2.zero;
            if (inputs[0])
                inputDirection.y += 1;
            if (inputs[1])
                inputDirection.y -= 1;
            if (inputs[2])
                inputDirection.x -= 1;
            if (inputs[3])
                inputDirection.x += 1;


            if (!isFlying)
                UpdateWalking(inputDirection);
            else
                UpdateFlying(inputDirection);

            if (transform.position.y < -20)
                transform.position = World.Get.GenerateSpawnPoint();

            PacketSender.PlayerTransform(this);
        }

        void UpdateFlying(Vector2 inputDirection)
        {
            Vector3 updateVel = (transform.right * inputDirection.x + transform.forward * inputDirection.y) * speed * 2f;

            if (inputs[7])
                updateVel += new Vector3(0f, -10f, 0f);
            if (inputs[5])
                updateVel += new Vector3(0f, 10f, 0f);

            transform.position += updateVel;
        }

        void UpdateWalking(Vector2 inputDirection)
        {
            yVel += gravity;
            if (isGrounded && inputs[5])
                yVel = jumpForce;

            Vector3 updateVel = (transform.right * inputDirection.x + transform.forward * inputDirection.y * (inputs[6] ? 1.5f : 1f)) * speed;

            updateVel += new Vector3(0f, yVel, 0f);

            transform.position += new Vector3(0, updateVel.y, 0);
            if (PlayerCollider.Collision(transform, out isGrounded))
            {
                transform.position -= new Vector3(0, updateVel.y, 0);
                yVel = -2f * Time.fixedDeltaTime;
            }

            transform.position += new Vector3(updateVel.x, 0, 0);
            if (PlayerCollider.Collision(transform, out bool _))
                transform.position -= new Vector3(updateVel.x, 0, 0);

            transform.position += new Vector3(0, 0, updateVel.z);
            if (PlayerCollider.Collision(transform, out bool _))
                transform.position -= new Vector3(0, 0, updateVel.z);
        }
    }
}
