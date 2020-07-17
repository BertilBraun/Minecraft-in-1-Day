using Assets;
using UnityEngine;

/// <summary>Sent from client to server.</summary>
public enum ClientPackets
{
    welcomeReceived = 1,
    playerInput,
    playerInteract
}

public class PacketSender
{
    #region Packets
    /// <summary>Lets the server know that the welcome message was received.</summary>
    public static void WelcomeReceived()
    {
        using (Packet packet = new Packet(ClientPackets.welcomeReceived))
        {
            packet.Write(Client.Get.myId);
            packet.Write(Settings.username);

            SendTCPData(packet);
        }
    }

    /// <summary>Sends player input to the server.</summary>
    /// <param name="inputs"></param>
    public static void PlayerInput(InputData data)
    {
        using (Packet packet = new Packet(ClientPackets.playerInput))
        {
            packet.Write(data.inputs.Length);
            foreach (bool input in data.inputs)
            {
                packet.Write(input);
            }
            packet.Write(data.rotation);
            packet.Write(data.mWheel);

            SendUDPData(packet);
        }
    }

    public static void PlayerInteract(Vector3 point, bool leftClick)
    {
        using (Packet packet = new Packet(ClientPackets.playerInteract))
        {
            packet.Write(leftClick);
            packet.Write(point);

            SendTCPData(packet);
        }
    }
    #endregion

    #region Functionality

    /// <summary>Sends a packet to the server via TCP.</summary>
    /// <param name="packet">The packet to send to the sever.</param>
    private static void SendTCPData(Packet packet)
    {
        packet.WriteLength();
        Client.Get.tcp.SendData(packet);
    }

    /// <summary>Sends a packet to the server via UDP.</summary>
    /// <param name="packet">The packet to send to the sever.</param>
    private static void SendUDPData(Packet packet)
    {
        packet.WriteLength();
        Client.Get.udp.SendData(packet);
    }

    #endregion
}
