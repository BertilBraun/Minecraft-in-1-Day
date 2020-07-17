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

    public static void WelcomeReceived(Guid fromClient, Packet packet)
    {
        Guid clientIdCheck = packet.ReadGuid();
        string username = packet.ReadString();

        Debug.Log($"{Server.clients[fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully.");
        if (fromClient != clientIdCheck)
        {
            Debug.Log($"Player \"{username}\" (ID: {fromClient}) has assumed the wrong client ID ({clientIdCheck})!");
        }
        Server.clients[fromClient].SendIntoGame(username);
    }

    public static void PlayerInput(Guid fromClient, Packet packet)
    {
        bool[] inputs = new bool[packet.ReadInt()];
        for (int i = 0; i < inputs.Length; i++)
        {
            inputs[i] = packet.ReadBool();
        }
        Quaternion rotation = packet.ReadQuaternion();
        int mWheelScroll = packet.ReadInt();

        GameManager.Get.Players[fromClient].SetInput(inputs, rotation, mWheelScroll);
    }

    public static void PlayerInteract(Guid fromClient, Packet packet)
    {
        bool leftClick = packet.ReadBool();
        Vector3Int interactionPoint = packet.ReadVector3().ToIntVec();

        GameManager.Get.Players[fromClient].Interact(interactionPoint, leftClick);
    }
}
