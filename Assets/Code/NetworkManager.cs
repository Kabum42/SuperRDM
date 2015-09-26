using UnityEngine;
using System.Collections;

public static class NetworkManager {

    private const string typeName = "OldLegends";
    private const string gameName = "RoomName";
    public static HostData[] hostList;

    public static void StartServer()
    {
        Network.InitializeServer(4, 25000, !Network.HavePublicAddress());
        MasterServer.RegisterHost(typeName, gameName);
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
