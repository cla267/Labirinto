using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SetReady : MonoBehaviourPunCallbacks
{
    bool isReady = false;
    public static int readyPlayers = 0;
    bool started = false;

    void Start()
    {
        
    }

    void Update()
    {
        if(MazeGenerator.isGenerated == true && isReady == false && photonView.IsMine)
        {
            photonView.RPC("SetPlayerReady", RpcTarget.AllBuffered);
            isReady = true;
        }

        print(readyPlayers);

        print(PhotonNetwork.CurrentRoom.PlayerCount);

        if(readyPlayers != PhotonNetwork.CurrentRoom.PlayerCount && started == false)
        {
            GetComponent<moovement>().enabled = false;
        }else
        {
            GetComponent<moovement>().enabled = true;
            started = true;
        }
    }

    [PunRPC]
    void SetPlayerReady()
    {
        readyPlayers += 1;
    }
}
