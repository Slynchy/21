using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManagerScript : NetworkManager {

    public override void OnServerConnect(NetworkConnection conn)
    {
        //base.OnServerConnect(conn);
        GameMaster.numOfPlayers++;
        GameObject.Find("GameVars").GetComponent<SyncListTest>().numOfPlayers++;
    }

    public override void OnStopServer()
    {
        GameMaster.HideCards();
    }

}
