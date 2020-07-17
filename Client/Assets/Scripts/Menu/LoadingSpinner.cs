using UnityEngine;

namespace Assets.Scripts.Menu
{
    public class LoadingSpinner : MonoBehaviour
    {
        public float speed = 20f;

        private void Update()
        {
            transform.Rotate(new Vector3(0, 0, 1), Time.deltaTime * speed);
        }
    }
}
