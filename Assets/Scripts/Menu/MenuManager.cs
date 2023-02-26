using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class MenuManager : MonoBehaviourPunCallbacks
{
    public GameObject canvas;

    [Header("Lobby")]
    public InputField roomNameInput;
    public Button createRoomButton;

    [Header("Room")]
    public InputField mazeSizeInput;
    public Text roomText;

    public GameObject[] panels;

    bool inRoom;

    void Start()
    {
        LoadPanel("Loading");
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return) && PhotonNetwork.IsMasterClient && int.TryParse(mazeSizeInput.text, out int number) && number <= 50 && number >= 3)
        {
            photonView.RPC("StartGame", RpcTarget.AllBuffered, number);
        }

        if(PhotonNetwork.IsMasterClient == false && inRoom)
        {
            mazeSizeInput.gameObject.SetActive(false);
            roomText.text = "Waiting for master...";
        }

        createRoomButton.enabled = CheckInput(roomNameInput);
    }

    void LoadPanel(string panelName)
    {
        for (int i = 0; i < panels.Length; i++)
        {
            if(panels[i].name != panelName)
            {
                panels[i].SetActive(false);
            } else
            {
                panels[i].SetActive(true);
            }
        }
    }

    public void CreateRoom()
    {
        LoadPanel("Loading");
        PhotonNetwork.JoinOrCreateRoom(roomNameInput.text, null, null);
    }

    [PunRPC]
    void StartGame(int number)
    {
        MazeGenerator.mazeSize = number;
        PhotonNetwork.LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
    }

    bool CheckInput(InputField inputField, bool excludeSpace = true)
    {
        if(excludeSpace == true){
            if(inputField.text != "" && inputField.text.Contains(" ") == false) return true;
            else return false;
        } else{
            if(inputField.text != "") return true;
            else return false;
        }
    }
    
#region PUN CALLBACKS
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        LoadPanel("Main Menu");
    }
    public override void OnJoinedRoom()
    {
        LoadPanel("Room");
        inRoom = true;
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
#endregion
}