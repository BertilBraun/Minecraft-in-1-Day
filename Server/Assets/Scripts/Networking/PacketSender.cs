using Assets.Scripts;
using Assets.Scripts.Minecraft.Player;
using Assets.Scripts.Minecraft.WorldManage;
using System;
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

public class PacketSender
{
    // TODO remove
    public static void TestPacket(Guid toClient)
    {
        byte[] data = new byte[Settings.ChunkVolume - 30];
        for (int i = 0; i < data.Length; i++)
            data[i] = (byte)(i % 2);
        Debug.Log("Sending data: " + data.Length);
        using (Packet packet = new Packet(ServerPackets.testPacket))
        {
            packet.Write(data);

            SendTCPData(toClient, packet);
        }
    }

    #region Packets
    /// <summary>Sends a welcome message to the given client.</summary>
    /// <param name="toClient">The client to send the packet to.</param>
    /// <param name="msg">The message to send.</param>
    public static void Welcome(Guid toClient, string msg)
    {
        using (Packet packet = new Packet(ServerPackets.welcome))
        {
            packet.Write(msg);
            packet.Write(toClient);

            SendTCPData(toClient, packet);
        }
    }

    public static void SendServerTick(int ServerTick)
    {
        using (Packet packet = new Packet(ServerPackets.serverTick))
        {
            packet.Write(ServerTick);
            packet.Write(DateTime.UtcNow.ToBinary());

            SendTCPDataToAll(packet);
        }
    }

    /// <summary>Tells a client to spawn a player.</summary>
    /// <param name="toClient">The client that should spawn the player.</param>
    /// <param name="player">The player to spawn.</param>
    public static void SpawnPlayer(Guid toClient, PlayerHandler player)
    {
        using (Packet packet = new Packet(ServerPackets.spawnPlayer))
        {
            packet.Write(player.id);
            packet.Write(player.username);
            packet.Write(player.transform.position);
            packet.Write(player.transform.rotation);

            SendTCPData(toClient, packet);
        }
    }

    /// <summary>Sends a player's updated position to all clients.</summary>
    /// <param name="player">The player whose position to update.</param>
    public static void PlayerTransform(PlayerMovement player)
    {
        using (Packet packet = new Packet(ServerPackets.playerTransform))
        {
            packet.Write(player.id);
            packet.Write(player.transform.position);
            packet.Write(player.transform.rotation);

            packet.Write(player.isFlying);
            packet.Write(player.isSwimming);

            SendUDPDataToAll(packet);
        }
    }

    public static void PlayerDisconnected(Guid playerId)
    {
        using (Packet packet = new Packet(ServerPackets.playerDisconnected))
        {
            packet.Write(playerId);

            SendTCPDataToAll(packet);
        }
    }

    public static void PlayerPickup(int droppedBlockID)
    {
        using (Packet packet = new Packet(ServerPackets.playerPickup))
        {
            packet.Write(droppedBlockID);

            SendTCPDataToAll(packet);
        }
    }

    public static void BlockDropped(int droppedBlockID, BlockType type, Vector3 pos)
    {
        using (Packet packet = new Packet(ServerPackets.blockDropped))
        {
            packet.Write(droppedBlockID);
            packet.Write((byte)type);
            packet.Write(pos);

            SendTCPDataToAll(packet);
        }
    }
    public static void BlockDropped(Guid id, int droppedBlockID, BlockType type, Vector3 pos)
    {
        using (Packet packet = new Packet(ServerPackets.blockDropped))
        {
            packet.Write(droppedBlockID);
            packet.Write((byte)type);
            packet.Write(pos);

            SendTCPData(id, packet);
        }
    }

    public static void HeldItemChanged(Guid id, BlockType type)
    {
        using (Packet packet = new Packet(ServerPackets.heldItemChanged))
        {
            packet.Write((byte)type);

            SendTCPData(id, packet);
        }
    }

    public static void ChunkSend(Guid id, Chunk c)
    {
        foreach (var section in c.sections)
            using (Packet packet = new Packet(ServerPackets.chunkSend))
            {
                packet.Write(section.Encode());

                SendTCPData(id, packet);
            }
    }

    public static void ChunkUpdate(Guid id, Vector3 pos, BlockType type)
    {
        using (Packet packet = new Packet(ServerPackets.chunkUpdate))
        {
            packet.Write(pos);
            packet.Write((byte)type);

            SendTCPData(id, packet);
        }
    }

    #endregion

    #region Functionality
    /// <summary>Sends a packet to a client via TCP.</summary>
    /// <param name="toClient">The client to send the packet the packet to.</param>
    /// <param name="packet">The packet to send to the client.</param>
    private static void SendTCPData(Guid toClient, Packet packet)
    {
        packet.WriteLength();
        Server.clients[toClient].tcp.SendData(packet);
    }

    /// <summary>Sends a packet to a client via UDP.</summary>
    /// <param name="toClient">The client to send the packet the packet to.</param>
    /// <param name="packet">The packet to send to the client.</param>
    private static void SendUDPData(Guid toClient, Packet packet)
    {
        packet.WriteLength();
        Server.clients[toClient].udp.SendData(packet);
    }

    /// <summary>Sends a packet to all clients via TCP.</summary>
    /// <param name="packet">The packet to send.</param>
    private static void SendTCPDataToAll(Packet packet)
    {
        packet.WriteLength();

        foreach (var client in Server.clients.Values)
            client.tcp.SendData(packet);
    }
    /// <summary>Sends a packet to all clients except one via TCP.</summary>
    /// <param name="exceptClient">The client to NOT send the data to.</param>
    /// <param name="packet">The packet to send.</param>
    private static void SendTCPDataToAll(Guid exceptClient, Packet packet)
    {
        packet.WriteLength();
        foreach (var client in Server.clients)
            if (client.Key != exceptClient)
                client.Value.tcp.SendData(packet);
    }

    /// <summary>Sends a packet to all clients via UDP.</summary>
    /// <param name="packet">The packet to send.</param>
    private static void SendUDPDataToAll(Packet packet)
    {
        packet.WriteLength();
        foreach (var client in Server.clients.Values)
            client.udp.SendData(packet);
    }
    /// <summary>Sends a packet to all clients except one via UDP.</summary>
    /// <param name="exceptClient">The client to NOT send the data to.</param>
    /// <param name="packet">The packet to send.</param>
    private static void SendUDPDataToAll(Guid exceptClient, Packet packet)
    {
        packet.WriteLength();
        foreach (var client in Server.clients)
            if (client.Key != exceptClient)
                client.Value.udp.SendData(packet);
    }
    #endregion
}
