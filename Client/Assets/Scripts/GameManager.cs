using Assets;
using Assets.Minecraft;
using Assets.Minecraft.Interactions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Get;

    [SerializeField]
    GameObject worldObject;
    [SerializeField]
    GameObject loadingPanel;

    [SerializeField]
    GameObject localPlayerPrefab;
    [SerializeField]
    GameObject playerPrefab;

    [SerializeField]
    GameObject droppedBlockPrefab;

    public Dictionary<int, DroppedBlock> droppedBlocks = new Dictionary<int, DroppedBlock>();
    public Dictionary<Guid, PlayerManager> players = new Dictionary<Guid, PlayerManager>();

    [HideInInspector]
    public GameObject localPlayer;
    [HideInInspector]
    public Transform localPlayerTransform;
    [HideInInspector]
    public PlayerManager localPlayerManager;

    public int ServerTick = 0;
    public TimeSpan ServerPing;


    private void Awake()
    {
        if (Get == null)
            Get = this;
        else if (Get != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    private void Start()
    {
        Client.Get.ConnectToServer();
    }

    public void SpawnPlayer(Guid id, string username, Vector3 position, Quaternion rotation)
    {
        GameObject player;
        if (id == Client.Get.myId)
        {
            // TODO
            Debug.Log("Instantiating Local Player");
            localPlayer = player = Instantiate(localPlayerPrefab, position, rotation);
            localPlayerManager = localPlayer.GetComponent<PlayerManager>();
            localPlayerTransform = localPlayer.transform;
            worldObject.SetActive(true);
            localPlayer.SetActive(false);
            StartCoroutine(DisableLoadingPanel());
        }
        else
        {
            // TODO
            Debug.Log("Instantiating New Player");
            player = Instantiate(playerPrefab, position, rotation);
        }

        // TODO
        player.GetComponent<PlayerManager>().Initialize(id, username);
        players.Add(id, player.GetComponent<PlayerManager>());
    }

    IEnumerator DisableLoadingPanel()
    {
        yield return new WaitForSeconds(Settings.timeToShowLoadingPanel);

        localPlayer.SetActive(true);
        loadingPanel.SetActive(false);
    }

    public void CreateDroppedBlock(int id, BlockType type, Vector3 pos)
    {
        DroppedBlock block = Instantiate(droppedBlockPrefab, pos, Quaternion.identity).GetComponent<DroppedBlock>();
        block.Init(type, pos);
        droppedBlocks.Add(id, block);
    }
}
