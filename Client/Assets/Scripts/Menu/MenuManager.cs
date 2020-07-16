using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.Menu
{
    public class MenuManager : MonoBehaviour
    {
        public InputField usernameField;
        public Button connectButton;

        public GameObject menuChunkPrefab;
        public Transform backgroundParent;

        private void Start()
        {
            connectButton.onClick.AddListener(OnConnect);
            usernameField.ActivateInputField();

            foreach (var path in Directory.GetFiles("Assets/Chunks/"))
                if (path.EndsWith(".dat"))
                    Instantiate(menuChunkPrefab, backgroundParent).GetComponent<MenuChunkRenderer>().Init(path);
        }

        private void OnConnect()
        {
            Settings.username = usernameField.text;
            if (Settings.username != "")
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            else
                usernameField.ActivateInputField();
        }
    }
}
