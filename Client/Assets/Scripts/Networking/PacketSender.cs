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
        using (Packet _packet = new Packet(ClientPackets.welcomeReceived))
        {
            _packet.Write(Client.Get.myId);
            _packet.Write("TestName"); // TODO Real Username

            SendTCPData(_packet);
        }
    }

    /// <summary>Sends player input to the server.</summary>
    /// <param name="_inputs"></param>
    public static void PlayerInput(bool[] _inputs, byte mWheelInput)
    {
        using (Packet _packet = new Packet(ClientPackets.playerInput))
        {
            _packet.Write(_inputs.Length);
            foreach (bool _input in _inputs)
            {
                _packet.Write(_input);
            }
            _packet.Write(GameManager.Get.players[Client.Get.myId].transform.rotation);
            _packet.Write(mWheelInput);

            SendUDPData(_packet);
        }
    }

    public static void PlayerInteract(Vector3 point, bool leftClick)
    {
        using (Packet _packet = new Packet(ClientPackets.playerInteract))
        {
            _packet.Write(leftClick);
            _packet.Write(point);

            SendTCPData(_packet);
        }
    }
    #endregion

    #region Functionality

    /// <summary>Sends a packet to the server via TCP.</summary>
    /// <param name="_packet">The packet to send to the sever.</param>
    private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.Get.tcp.SendData(_packet);
    }

    /// <summary>Sends a packet to the server via UDP.</summary>
    /// <param name="_packet">The packet to send to the sever.</param>
    private static void SendUDPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.Get.udp.SendData(_packet);
    }

    #endregion
}
