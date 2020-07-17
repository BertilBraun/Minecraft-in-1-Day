using Assets;
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
    serverTick,
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
        { ServerPackets.serverTick, ServerTick },
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
    public static void TestPacket(Packet packet)
    {
        byte[] data = packet.ReadBytes(packet.UnreadLength());
        Debug.Log(data.Length);
        for (int i = 0; i < data.Length; i++)
            if (data[i] != i % 2)
                Debug.Log("Error on data transfer");
        Debug.Log("Otherwise no Error occured");
    }

    public static void Welcome(Packet packet)
    {
        string msg = packet.ReadString();
        Guid myId = packet.ReadGuid();

        Debug.Log($"Message from server: {msg}");
        Client.Get.myId = myId;
        PacketSender.WelcomeReceived();

        // Now that we have the client's id, connect UDP
        Client.Get.udp.Connect(((IPEndPoint)Client.Get.tcp.socket.Client.LocalEndPoint).Port);
    }

    private static void ServerTick(Packet packet)
    {
        int ServerTick = packet.ReadInt();
        DateTime ServerSendTime = DateTime.FromBinary(packet.ReadLong());

        GameManager.Get.ServerTick = ServerTick;
        GameManager.Get.ServerPing = DateTime.UtcNow - ServerSendTime;
    }

    public static void SpawnPlayer(Packet packet)
    {
        Guid id = packet.ReadGuid();
        string username = packet.ReadString();
        Vector3 position = packet.ReadVector3();
        Quaternion rotation = packet.ReadQuaternion();

        GameManager.Get.SpawnPlayer(id, username, position, rotation);
    }

    public static void PlayerTransform(Packet packet)
    {
        Guid id = packet.ReadGuid();
        Vector3 position = packet.ReadVector3();
        Quaternion rotation = packet.ReadQuaternion();

        bool isFlying = packet.ReadBool();
        bool isSwimming = packet.ReadBool();

        PlayerManager player = GameManager.Get.players[id];

        if (player != GameManager.Get.localPlayerManager)
        {
            player.transform.rotation = rotation;
            player.transform.position = position;
        }
        else
        {
            if ((player.transform.position - position).sqrMagnitude > Settings.DistanceToSyncPlayerPositionFrom)
                player.transform.position = position;
            PlayerMovementPrediction prediction = player.GetComponent<PlayerMovementPrediction>();
            prediction.isFlying = isFlying;
            prediction.isSwimming = isSwimming;
        }
    }

    public static void PlayerDisconnected(Packet packet)
    {
        Guid id = packet.ReadGuid();

        GameObject.Destroy(GameManager.Get.players[id].gameObject);
        GameManager.Get.players.Remove(id);
    }

    public static void PlayerPickup(Packet packet)
    {
        int id = packet.ReadInt();

        GameManager.Get.droppedBlocks[id].Destroy();
    }

    private static void BlockDropped(Packet packet)
    {
        int id = packet.ReadInt();
        BlockType type = (BlockType)packet.ReadByte();
        Vector3 pos = packet.ReadVector3();

        GameManager.Get.CreateDroppedBlock(id, type, pos);
    }

    private static void ChunkUpdate(Packet packet)
    {
        Vector3Int pos = packet.ReadVector3().ToIntVec();
        BlockType type = (BlockType)packet.ReadByte();

        World.Get.SetBlock(pos.x, pos.y, pos.z, type);
        // TODO Update Chunk Mesh
        // TODO Should be only place to call SetBlock
    }

    private static Dictionary<Vector2Int, Chunk> loadingChunks = new Dictionary<Vector2Int, Chunk>();
    private static void ChunkRecieve(Packet packet)
    {
        Vector3Int pos = new Vector3Int(packet.ReadInt(), packet.ReadInt(), packet.ReadInt());
        bool encoded = packet.ReadBool();
        int count = packet.ReadInt();
        byte[] data = packet.ReadBytes(count);

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

    private static void HeldItemChanged(Packet packet)
    {
        BlockType type = (BlockType)packet.ReadByte();

        GameManager.Get.localPlayer.GetComponentInChildren<HeldItemDisplay>().ChangeHeldBlock(type);
    }
}
