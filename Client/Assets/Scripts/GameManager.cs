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

    private void Update()
    {
    }

    /// <summary>Spawns a player.</summary>
    /// <param name="_id">The player's ID.</param>
    /// <param name="_name">The player's name.</param>
    /// <param name="_position">The player's starting position.</param>
    /// <param name="_rotation">The player's starting rotation.</param>
    public void SpawnPlayer(Guid _id, string _username, Vector3 _position, Quaternion _rotation)
    {
        GameObject _player;
        if (_id == Client.Get.myId)
        {
            // TODO
            Debug.Log("Instantiating Local Player");
            localPlayer = _player = Instantiate(localPlayerPrefab, _position, _rotation);
            localPlayerManager = localPlayer.GetComponent<PlayerManager>();
            localPlayerTransform = localPlayer.transform;
            worldObject.SetActive(true);
        }
        else
        {
            // TODO
            Debug.Log("Instantiating New Player");
            _player = Instantiate(playerPrefab, _position, _rotation);
        }

        // TODO
        _player.GetComponent<PlayerManager>().Initialize(_id, _username);
        players.Add(_id, _player.GetComponent<PlayerManager>());
    }

    public void CreateDroppedBlock(int id, BlockType type, Vector3 pos)
    {
        DroppedBlock block = Instantiate(droppedBlockPrefab, pos, Quaternion.identity).GetComponent<DroppedBlock>();
        block.Init(type, pos);
        droppedBlocks.Add(id, block);
    }
}
