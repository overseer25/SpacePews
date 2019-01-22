using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonLobby : MonoBehaviourPunCallbacks
{

    public static PhotonLobby lobby;
    public GameObject joinButton;
    public GameObject cancelButton;

    private void Awake()
    {
        lobby = this;    
    }

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    /// <summary>
    /// Callback function when the master server is connected to.
    /// </summary>
    public override void OnConnectedToMaster()
    {
        Debug.Log("Player has conencted to the Photon Master Server.");
        PhotonNetwork.AutomaticallySyncScene = true;
        joinButton.SetActive(true);
    }

    /// <summary>
    /// Dictates what happens when the user clicks the join button.
    /// </summary>
    public void OnJoinButtonClick()
    {
        Debug.Log("Join button was clicked.");
        joinButton.SetActive(false);
        cancelButton.SetActive(true);
        PhotonNetwork.JoinRandomRoom();
    }

    /// <summary>
    /// Dictates what happens when the user clicks the cancel button.
    /// </summary>
    public void OnCancelButtonClick()
    {
        cancelButton.SetActive(false);
        joinButton.SetActive(true);
        PhotonNetwork.LeaveRoom();
    }

    /// <summary>
    /// If the player fails to join a random room, create one for them.
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to connect to the Photon Master Server. Creating a new room...");
        CreateRoom();
    }

    /// <summary>
    /// Create a room.
    /// </summary>
    private void CreateRoom()
    {
        Debug.Log("Creating room.");
        int randomRoomName = Random.Range(0, 10000);
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)MultiplayerSettings.settings.maxPlayers };
        PhotonNetwork.CreateRoom("Room" + randomRoomName, roomOps);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a room.");
        base.OnJoinedRoom();
    }

    /// <summary>
    /// If the service is unable to create a room, try again.
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to create room. Trying again...");
        CreateRoom();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
