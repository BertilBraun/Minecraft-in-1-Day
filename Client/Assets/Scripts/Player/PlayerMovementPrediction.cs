using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerMovementPrediction : MonoBehaviour
    {
        public PlayerInput input;

        public float speed = 8f;
        public float gravity = -9.81f;
        public float jumpForce = 1.5f;

        public bool isFlying = false;
        public bool isGrounded = false;
        public float yVel = 0f;

        private void Start()
        {
            gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
            speed *= Time.fixedDeltaTime;
            jumpForce *= Time.fixedDeltaTime;
        }

        public void FixedUpdate()
        {
            return; // TODO work on client side prediction
            if (isFlying)
                UpdateFlying();
            else
                UpdateWalking();
        }

        void UpdateFlying()
        {
            Vector3 updateVel = (transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical")) * speed * 2f;

            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                updateVel += new Vector3(0f, -3f, 0f);
            if (Input.GetKey(KeyCode.Space))
                updateVel += new Vector3(0f, 3f, 0f);

            transform.position += updateVel;
        }

        void UpdateWalking()
        {
            yVel += gravity;
            if (Input.GetKey(KeyCode.Space) && isGrounded)
                yVel = jumpForce;

            float yspeed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? 1.5f : 1f;
            Vector3 updateVel = (transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical") * yspeed) * speed;

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
