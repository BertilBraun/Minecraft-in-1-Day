using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerMovementPrediction : MonoBehaviour
    {
        public PlayerInput input;

        Dictionary<int, InputData> inputDict = new Dictionary<int, InputData>();

        public float speed = 8f;
        public float gravity = -9.81f;
        public float jumpForce = 1.5f;

        public bool isFlying = false;
        public bool isGrounded = false;
        public bool isSwimming = false;
        public float yVel = 0f;

        private void Start()
        {
            //gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
            //speed *= Time.fixedDeltaTime;
            //jumpForce *= Time.fixedDeltaTime;
        }

        private void Update()
        {
            if (isFlying)
                UpdateFlying();
            else
                UpdateWalking();
        }
        public void FixedUpdate()
        {
            // TODO think about this
            //int ServerTick = GameManager.Get.ServerTick;
            //TimeSpan ServerPing = GameManager.Get.ServerPing;

            //int ServerTickToExecuteOn = ServerTick + (int)(ServerPing.TotalSeconds / Time.fixedDeltaTime);
            //inputDict[ServerTickToExecuteOn] = input.data;

            //if (!inputDict.ContainsKey(ServerTick))
            //    return;

            //InputData data = inputDict[ServerTick];
            //inputDict.Remove(ServerTick);

            //Vector2 inputDirection = Vector2.zero;
            //if (data.inputs[0])
            //    inputDirection.y += 1;
            //if (data.inputs[1])
            //    inputDirection.y -= 1;
            //if (data.inputs[2])
            //    inputDirection.x -= 1;
            //if (data.inputs[3])
            //    inputDirection.x += 1;

            //if (isFlying)
            //    UpdateFlying(inputDirection, data);
            //else
            //    UpdateWalking(inputDirection, data);
        }

        void UpdateFlying()
        {
            Vector3 updateVel = (transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical")) * (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? 1.5f : 1f) * speed * 2f;

            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                updateVel += new Vector3(0f, -10f, 0f);
            if (Input.GetKey(KeyCode.Space))
                updateVel += new Vector3(0f, 10f, 0f);

            transform.position += updateVel * Time.deltaTime;
        }

        void UpdateWalking()
        {
            yVel += gravity * Time.deltaTime;
            if (isGrounded && Input.GetKey(KeyCode.Space))
                yVel = jumpForce;

            Vector3 updateVel = (transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical") * (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? 1.5f : 1f)) * speed;

            updateVel += new Vector3(0f, yVel, 0f);
            updateVel *= Time.deltaTime;

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
        void UpdateFlying(Vector2 inputDirection, InputData data)
        {
            Vector3 updateVel = (transform.right * inputDirection.x + transform.forward * inputDirection.y) * speed * 2f;

            if (data.inputs[7])
                updateVel += new Vector3(0f, -3f, 0f);
            if (data.inputs[5])
                updateVel += new Vector3(0f, 3f, 0f);

            transform.position += updateVel;
        }

        void UpdateWalking(Vector2 inputDirection, InputData data)
        {
            yVel += gravity;
            if (isGrounded && data.inputs[5])
                yVel = jumpForce;

            Vector3 updateVel = (transform.right * inputDirection.x + transform.forward * inputDirection.y * (data.inputs[6] ? 1.5f : 1f)) * speed;

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
