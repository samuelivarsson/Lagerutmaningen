﻿using Photon.Pun;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;

    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_InputField createNickNameInputField;
    [SerializeField] TMP_InputField findNickNameInputField;
    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] TMP_Text currPlayersInRoom;
    [SerializeField] Transform roomListContent;
    public Transform playerListContent;
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] GameObject playerListItemPrefab;
    [SerializeField] GameObject startGameButton;

    Player[] players;

    List<string> takenNames = new List<string>{};

    public const int maxPlayers = 4;

    List<string> characterList = new List<string> {"Long", "Normal", "Strong", "Weak"};
    List<int> spawnPointList = new List<int>();

    void Awake()
    {
        Instance = this;
        for (int i = 0; i < maxPlayers; i++)
        {
            spawnPointList.Add(i);
        }
    }

    void Start()
    {
        // Connecting to master server (Set to eu in PhotonServerSettings)
        if(!PhotonNetwork.IsConnected) 
        {
            Debug.Log("Connecting to the master server...");
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    void Update()
    {
        if (PhotonNetwork.InRoom) currPlayersInRoom.text = PhotonNetwork.PlayerList.Length.ToString();
    }

    public override void OnConnectedToMaster()
    {
        if(!PhotonNetwork.InLobby) 
        {
            PhotonNetwork.JoinLobby();
            Debug.Log("Connected to the " + PhotonNetwork.CloudRegion + " server!");
            PhotonNetwork.AutomaticallySyncScene = true;
        }        
    }

    public override void OnJoinedLobby()
    {
        MenuManager.Instance.OpenMenu("title");
        Debug.Log("Joined Lobby");
    }
  
    public void CreateRoom()
    {
        if(string.IsNullOrEmpty(roomNameInputField.text) || string.IsNullOrEmpty(createNickNameInputField.text))
        {
          return;
        }
        PhotonNetwork.CreateRoom(roomNameInputField.text);
        PhotonNetwork.NickName = createNickNameInputField.text;
        MenuManager.Instance.OpenMenu("loading");
    }

    public override void OnJoinedRoom() 
    {
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        MenuManager.Instance.OpenMenu("room");

        SetNick(PhotonNetwork.LocalPlayer);
        object[] initData = {PhotonNetwork.LocalPlayer.NickName};
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "UI", "MainMenu", "PlayerListItem"), Vector3.zero, Quaternion.identity, 0, initData);

        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        // When the last player has updated the room properties -> Start the game.
        string lastPlayerNick = PhotonNetwork.PlayerList[PhotonNetwork.PlayerList.Length-1].NickName;
        if (propertiesThatChanged[lastPlayerNick+"Character"] != null) PhotonNetwork.LoadLevel(1);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }
    
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room Creation Failed: " + message;
        MenuManager.Instance.OpenMenu("error");
    }

    public void StartGame()
    {
        if (PhotonNetwork.PlayerList.Length != maxPlayers) return;

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            Hashtable hash = new Hashtable();

            int randomCharacterIndex = Random.Range(0, characterList.Count);
            string character = characterList[randomCharacterIndex];
            characterList.RemoveAt(randomCharacterIndex);
            hash.Add(player.NickName+"Character", character);

            int randomSpawnPointIndex = Random.Range(0, spawnPointList.Count);
            int spawnPoint = spawnPointList[randomSpawnPointIndex];
            spawnPointList.RemoveAt(randomSpawnPointIndex);
            hash.Add(player.NickName+"SpawnPoint", spawnPoint);

            PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
        }
    }
    
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("loading");
    }

    public void JoinRoom(RoomInfo info)
    {        
        if(string.IsNullOrEmpty(findNickNameInputField.text) || PhotonNetwork.PlayerList.Length >= 4)
        {                
            return;
        }
                      
        PhotonNetwork.JoinRoom(info.Name);
        PhotonNetwork.NickName = findNickNameInputField.text;       
        MenuManager.Instance.OpenMenu("loading");
    }

    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu("title");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {    
        RoomListItem[] currentRooms = roomListContent.GetComponentsInChildren<RoomListItem>();
        for (int i = 0; i < roomList.Count; i++)
        {
            for (int j = 0; j < currentRooms.Length; j++)
            {
                if(roomList[i].Name == currentRooms[j].info.Name) 
                {
                    Destroy(currentRooms[j].gameObject);
                }
            } 
        }         

        for (int i = 0; i < roomList.Count; i++)
        {
            if(roomList[i].RemovedFromList) continue;
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
    bool IsNameTaken(string name)
    {
        foreach (Player player in PhotonNetwork.PlayerListOthers)
        {
            if(player.NickName.Equals(name)) 
            {
                return true;
            }
        }
        return false;
    }

    void SetNick(Player player) 
    {
        if(!IsNameTaken(player.NickName)) return;

        int i = 0;
        string temp = player.NickName; 
        string newName = player.NickName;        
        while (IsNameTaken(temp))
        {   
            i++;
            temp = newName + i;            
        }
        newName += i;
        player.NickName = newName;
    }
}

