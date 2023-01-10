using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ServerController : MonoBehaviourPunCallbacks
{
    public MainMenu MainMenu;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        Connect();
    }

    public void Connect()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        MainMenu.DropDown.interactable = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
