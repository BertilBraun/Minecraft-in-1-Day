using Assets.Minecraft;
using Assets.Minecraft.Interactions;
using Assets.Scripts.Minecraft.WorldManage;
using Assets.Scripts.Player;
using System;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;

/// <summary>Sent from server to client.</summary>
public enum ServerPackets
{
    welcome = 1,
    spawnPlayer,
    playerTransform,
    playerDisconnected,
    playerPickup,

    heldItemChanged,

    chunkSend,
    chunkUpdate,

    blockDropped,

    testPacket
}


public class PacketHandler
{
    public readonly static Dictionary<ServerPackets, Action<Packet>> PacketHandlers = new Dictionary<ServerPackets, Action<Packet>>
    {
        { ServerPackets.welcome, Welcome },
        { ServerPackets.spawnPlayer, SpawnPlayer },
        { ServerPackets.playerTransform, PlayerTransform },
        { ServerPackets.playerDisconnected, PlayerDisconnected },
        { ServerPackets.playerPickup, PlayerPickup },

        { ServerPackets.heldItemChanged, HeldItemChanged },

        { ServerPackets.chunkSend, ChunkRecieve },
        { ServerPackets.chunkUpdate, ChunkUpdate },

        { ServerPackets.blockDropped, BlockDropped },

        { ServerPackets.testPacket, TestPacket }
    };

    // TODO remove
    public static void TestPacket(Packet _packet)
    {
        byte[] data = _packet.ReadBytes(_packet.UnreadLength());
        Debug.Log(data.Length);
        for (int i = 0; i < data.Length; i++)
            if (data[i] != i % 2)
                Debug.Log("Error on data transfer");
        Debug.Log("Otherwise no Error occured");
    }

    public static void Welcome(Packet _packet)
    {
        string _msg = _packet.ReadString();
        Guid _myId = _packet.ReadGuid();

        Debug.Log($"Message from server: {_msg}");
        Client.Get.myId = _myId;
        PacketSender.WelcomeReceived();

        // Now that we have the client's id, connect UDP
        Client.Get.udp.Connect(((IPEndPoint)Client.Get.tcp.socket.Client.LocalEndPoint).Port);
    }

    public static void SpawnPlayer(Packet _packet)
    {
        Guid _id = _packet.ReadGuid();
        string _username = _packet.ReadString();
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();

        GameManager.Get.SpawnPlayer(_id, _username, _position, _rotation);
    }

    static float TimeOfLastSync = -2f;
    static float TimeToSync = 0f; // TODO work on client side prediction
    public static void PlayerTransform(Packet _packet)
    {
        Guid _id = _packet.ReadGuid();
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();

        bool _isFlying = _packet.ReadBool();

        PlayerManager player = GameManager.Get.players[_id];

        if (player != GameManager.Get.localPlayerManager)
        {
            player.transform.rotation = _rotation;
            player.transform.position = _position;
        }
        else
        {
            if (Time.time - TimeOfLastSync > TimeToSync)
            {
                TimeOfLastSync = Time.time;
                player.transform.position = _position;
                player.GetComponent<PlayerInput>().isFlying = _isFlying;
            }
        }
    }

    public static void PlayerDisconnected(Packet _packet)
    {
        Guid _id = _packet.ReadGuid();

        GameObject.Destroy(GameManager.Get.players[_id].gameObject);
        GameManager.Get.players.Remove(_id);
    }

    public static void PlayerPickup(Packet _packet)
    {
        int _id = _packet.ReadInt();

        GameManager.Get.droppedBlocks[_id].Destroy();
    }

    private static void BlockDropped(Packet _packet)
    {
        int _id = _packet.ReadInt();
        BlockType type = (BlockType)_packet.ReadByte();
        Vector3 pos = _packet.ReadVector3();

        GameManager.Get.CreateDroppedBlock(_id, type, pos);
    }

    private static void ChunkUpdate(Packet _packet)
    {
        Vector3Int pos = _packet.ReadVector3().ToIntVec();
        BlockType type = (BlockType)_packet.ReadByte();

        World.Get.SetBlock(pos.x, pos.y, pos.z, type);
        // TODO Update Chunk Mesh
        // TODO Should be only place to call SetBlock
    }

    private static Dictionary<Vector2Int, Chunk> loadingChunks = new Dictionary<Vector2Int, Chunk>();
    private static void ChunkRecieve(Packet _packet)
    {
        Vector3Int pos = new Vector3Int(_packet.ReadInt(), _packet.ReadInt(), _packet.ReadInt());
        bool encoded = _packet.ReadBool();
        int count = _packet.ReadInt();
        byte[] data = _packet.ReadBytes(count);

        Vector2Int parentPos = new Vector2Int(pos.x, pos.z);
        if (!loadingChunks.ContainsKey(parentPos))
            loadingChunks[parentPos] = new Chunk(parentPos);

        Chunk parent = loadingChunks[parentPos];

        parent.sections[pos.y] = ChunkSection.Decode(parent, pos, data, encoded);

        foreach (ChunkSection section in parent.sections)
            if (section == null)
                return;

        loadingChunks.Remove(parentPos);
        ChunkManager.Get.AddChunk(parent);
    }

    private static void HeldItemChanged(Packet _packet)
    {
        BlockType type = (BlockType)_packet.ReadByte();

        GameManager.Get.localPlayer.GetComponentInChildren<HeldItemDisplay>().ChangeHeldBlock(type);
    }
}
