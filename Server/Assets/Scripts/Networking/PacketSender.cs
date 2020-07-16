using Assets.Scripts;
using Assets.Scripts.Minecraft.Player;
using Assets.Scripts.Minecraft.WorldManage;
using System;
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

public class PacketSender
{
    // TODO remove
    public static void TestPacket(Guid _toClient)
    {
        byte[] data = new byte[Settings.ChunkVolume - 30];
        for (int i = 0; i < data.Length; i++)
            data[i] = (byte)(i % 2);
        Debug.Log("Sending data: " + data.Length);
        using (Packet _packet = new Packet(ServerPackets.testPacket))
        {
            _packet.Write(data);

            SendTCPData(_toClient, _packet);
        }
    }

    #region Packets
    /// <summary>Sends a welcome message to the given client.</summary>
    /// <param name="_toClient">The client to send the packet to.</param>
    /// <param name="_msg">The message to send.</param>
    public static void Welcome(Guid _toClient, string _msg)
    {
        using (Packet _packet = new Packet(ServerPackets.welcome))
        {
            _packet.Write(_msg);
            _packet.Write(_toClient);

            SendTCPData(_toClient, _packet);
        }
    }

    /// <summary>Tells a client to spawn a player.</summary>
    /// <param name="_toClient">The client that should spawn the player.</param>
    /// <param name="_player">The player to spawn.</param>
    public static void SpawnPlayer(Guid _toClient, PlayerHandler _player)
    {
        using (Packet _packet = new Packet(ServerPackets.spawnPlayer))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.username);
            _packet.Write(_player.transform.position);
            _packet.Write(_player.transform.rotation);

            SendTCPData(_toClient, _packet);
        }
    }

    /// <summary>Sends a player's updated position to all clients.</summary>
    /// <param name="_player">The player whose position to update.</param>
    public static void PlayerTransform(PlayerMovement _player)
    {
        using (Packet _packet = new Packet(ServerPackets.playerTransform))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.transform.position);
            _packet.Write(_player.transform.rotation);

            _packet.Write(_player.isFlying);

            SendUDPDataToAll(_packet);
        }
    }

    public static void PlayerDisconnected(Guid _playerId)
    {
        using (Packet _packet = new Packet(ServerPackets.playerDisconnected))
        {
            _packet.Write(_playerId);

            SendTCPDataToAll(_packet);
        }
    }

    public static void PlayerPickup(int droppedBlockID)
    {
        using (Packet _packet = new Packet(ServerPackets.playerPickup))
        {
            _packet.Write(droppedBlockID);

            SendTCPDataToAll(_packet);
        }
    }

    public static void BlockDropped(int droppedBlockID, BlockType type, Vector3 pos)
    {
        using (Packet _packet = new Packet(ServerPackets.blockDropped))
        {
            _packet.Write(droppedBlockID);
            _packet.Write((byte)type);
            _packet.Write(pos);

            SendTCPDataToAll(_packet);
        }
    }
    public static void BlockDropped(Guid id, int droppedBlockID, BlockType type, Vector3 pos)
    {
        using (Packet _packet = new Packet(ServerPackets.blockDropped))
        {
            _packet.Write(droppedBlockID);
            _packet.Write((byte)type);
            _packet.Write(pos);

            SendTCPData(id, _packet);
        }
    }

    public static void HeldItemChanged(Guid id, BlockType type)
    {
        using (Packet _packet = new Packet(ServerPackets.heldItemChanged))
        {
            _packet.Write((byte)type);

            SendTCPData(id, _packet);
        }
    }

    public static void ChunkSend(Guid id, Chunk c)
    {
        foreach (var section in c.sections)
            using (Packet _packet = new Packet(ServerPackets.chunkSend))
            {
                _packet.Write(section.Encode());

                SendTCPData(id, _packet);
            }
    }

    public static void ChunkUpdate(Guid id, Vector3 pos, BlockType type)
    {
        using (Packet _packet = new Packet(ServerPackets.chunkUpdate))
        {
            _packet.Write(pos);
            _packet.Write((byte)type);

            SendTCPData(id, _packet);
        }
    }

    #endregion

    #region Functionality
    /// <summary>Sends a packet to a client via TCP.</summary>
    /// <param name="_toClient">The client to send the packet the packet to.</param>
    /// <param name="_packet">The packet to send to the client.</param>
    private static void SendTCPData(Guid _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].tcp.SendData(_packet);
    }

    /// <summary>Sends a packet to a client via UDP.</summary>
    /// <param name="_toClient">The client to send the packet the packet to.</param>
    /// <param name="_packet">The packet to send to the client.</param>
    private static void SendUDPData(Guid _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].udp.SendData(_packet);
    }

    /// <summary>Sends a packet to all clients via TCP.</summary>
    /// <param name="_packet">The packet to send.</param>
    private static void SendTCPDataToAll(Packet _packet)
    {
        _packet.WriteLength();

        foreach (var client in Server.clients.Values)
            client.tcp.SendData(_packet);
    }
    /// <summary>Sends a packet to all clients except one via TCP.</summary>
    /// <param name="_exceptClient">The client to NOT send the data to.</param>
    /// <param name="_packet">The packet to send.</param>
    private static void SendTCPDataToAll(Guid _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        foreach (var client in Server.clients)
            if (client.Key != _exceptClient)
                client.Value.tcp.SendData(_packet);
    }

    /// <summary>Sends a packet to all clients via UDP.</summary>
    /// <param name="_packet">The packet to send.</param>
    private static void SendUDPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        foreach (var client in Server.clients.Values)
            client.udp.SendData(_packet);
    }
    /// <summary>Sends a packet to all clients except one via UDP.</summary>
    /// <param name="_exceptClient">The client to NOT send the data to.</param>
    /// <param name="_packet">The packet to send.</param>
    private static void SendUDPDataToAll(Guid _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        foreach (var client in Server.clients)
            if (client.Key != _exceptClient)
                client.Value.udp.SendData(_packet);
    }
    #endregion
}
