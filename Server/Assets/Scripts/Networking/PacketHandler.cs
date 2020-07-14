using Assets.Scripts;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Sent from client to server.</summary>
public enum ClientPackets
{
    welcomeReceived = 1,
    playerInput,
    playerInteract
}

public class PacketHandler
{
    public readonly static Dictionary<ClientPackets, Action<Guid, Packet>> PacketHandlers = new Dictionary<ClientPackets, Action<Guid, Packet>>
    {
        { ClientPackets.welcomeReceived, WelcomeReceived },
        { ClientPackets.playerInput, PlayerInput },
        { ClientPackets.playerInteract, PlayerInteract }
    };

    public static void WelcomeReceived(Guid _fromClient, Packet _packet)
    {
        Guid _clientIdCheck = _packet.ReadGuid();
        string _username = _packet.ReadString();

        Debug.Log($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully.");
        if (_fromClient != _clientIdCheck)
        {
            Debug.Log($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
        }
        Server.clients[_fromClient].SendIntoGame(_username);
    }

    public static void PlayerInput(Guid _fromClient, Packet _packet)
    {
        bool[] _inputs = new bool[_packet.ReadInt()];
        for (int i = 0; i < _inputs.Length; i++)
        {
            _inputs[i] = _packet.ReadBool();
        }
        Quaternion _rotation = _packet.ReadQuaternion();
        byte mWheelScroll = _packet.ReadByte();

        GameManager.Get.Players[_fromClient].SetInput(_inputs, _rotation, mWheelScroll);
    }

    public static void PlayerInteract(Guid _fromClient, Packet _packet)
    {
        bool leftClick = _packet.ReadBool();
        Vector3Int interactionPoint = _packet.ReadVector3().ToIntVec();

        GameManager.Get.Players[_fromClient].Interact(interactionPoint, leftClick);
    }
}
