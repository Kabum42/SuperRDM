using UnityEngine;
using System.Collections;

public static class NetworkManager {

    private const string typeName = "OldLegends";
    //private const string gameName = "RoomName";
    public static HostData[] hostList;

    public static void StartServer(string roomName)
    {
        Network.InitializeServer(4, 25000, !Network.HavePublicAddress());
        MasterServer.RegisterHost(typeName, roomName);
    }

    public static void RefreshHostList()
    {
        MasterServer.RequestHostList(typeName);
    }

    public static void JoinServer(HostData hostData)
    {
        Network.Connect(hostData);
    }

}
