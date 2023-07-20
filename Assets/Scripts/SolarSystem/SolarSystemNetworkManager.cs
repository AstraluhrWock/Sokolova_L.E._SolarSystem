using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class SolarSystemNetworkManager : NetworkManager
{
    [SerializeField] private string _playerName;
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private int _count;
    Dictionary<int, ShipController> _players = new Dictionary<int, ShipController>();
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        var spawnTransform = GetStartPosition();
        var player = Instantiate(playerPrefab, spawnTransform.position, spawnTransform.rotation);
        player.GetComponent<ShipController>().PlayerName = _playerName;
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }
    public override void OnStartServer()
    {
        base.OnStartServer();
        //NetworkServer.RegisterHandler(100, ReceiveName);
    }

    public class MessageLogin : MessageBase
    {
        public string login;

        public override void Deserialize(NetworkReader reader)
        {
            login = reader.ReadString();
        }

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(login);
        }
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        MessageLogin _login = new MessageLogin();
        _login.login = _inputField.text;
        conn.Send(100, _login);
    }

//    public void ReceiveName(NetworkMessage networkMessage)
//    {
//        _players[networkMessage.conn.connectionId].PlayerName = networkMessage.reader.ReadString();
//        _players[networkMessage.conn.connectionId].gameObject.name = _players[networkMessage.conn.connectionId].PlayerName;
//        Debug.Log(_players[networkMessage.conn.connectionId]);
//    }
}
