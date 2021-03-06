using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance;

    // Will be set to true when all players has loaded to the game scene.
    public bool startCountDown {get; set;} = false;

    // Game starts when time left is less than 1, therefore 5.99 will result in a 5 second count down.
    float countDownTimeLeft = 5.99f;

    // Maximum amount of products per task
    const int maxProducts = 3;

    // Maximum number of tasks at the same time
    const int maxTasks = 4;

    // Delay in seconds before a new tasks spawns
    float taskDelay;

    // Time in seconds
    public int baseTime {get; set;}
    public int amountMultiplier {get; set;}

    float[] countDownTimes = new float[maxTasks];
    bool[] countDownBools = new bool[maxTasks];

    GameObject canvasManager;
    PhotonView PV;
 
    void Awake()
    {
        Instance = this;
        PV = GetComponent<PhotonView>();
        canvasManager = CanvasManager.Instance.gameObject;
        gameObject.transform.SetParent(canvasManager.transform);
    }
        
    void Start()
    {
        SetDifficulty();
        for (int i = 0; i < maxTasks; i++)
        {
            countDownTimes[i] = taskDelay;
            countDownBools[i] = false;
        }
        if (PhotonNetwork.IsMasterClient) CreateTasks();
    }

    void FixedUpdate()
    {
        if (startCountDown)
        {
            countDownTimeLeft -= Time.fixedDeltaTime;
            int n = (int) countDownTimeLeft;
            CanvasManager.Instance.countDownText.text = (n == 0) ? "Spela!" : ""+n;
            if (countDownTimeLeft <= 0.25f)
            {
                startCountDown = false;
                CanvasManager.Instance.countDownObj.SetActive(false);
            }
            else if (countDownTimeLeft < 1 && PhotonNetwork.IsMasterClient)
            {
                Hashtable hash = new Hashtable();
                hash.Add("gameStarted", true);
                PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
            }
            return;
        }

        if (!PhotonNetwork.IsMasterClient) return;

        for (int i = 0; i < maxTasks; i++)
        {
            if (countDownBools[i])
            {
                countDownTimes[i] -= Time.fixedDeltaTime;
                if (countDownTimes[i] < 0)
                {
                    countDownBools[i] = false;
                    CreateTask(i);
                }
            }
        }
    }

    void CreateTasks()
    {
        
            if(!PhotonNetwork.OfflineMode)
            {
                for (int i = 0; i < maxTasks; i++)
                {
                    CreateTask(i);
                }
            }
            else
            {
                for(int i = 1; i < 3; i++)
                {
                    CreateOfflineTask(i);
                }
            }
            
        
    }

    void CreateOfflineTask(int i)
    {
        string tag = "Task" + i;
        int productAmount = i;
        string[] requiredProducts = i == 1 ? new string[] { "Laptop" } : new string[] { "Laptop", "Ball" };
        int time = baseTime + (productAmount * amountMultiplier);
        object[] initData = { tag, i, productAmount, requiredProducts, time };
        GameObject taskObj = PhotonNetwork.InstantiateRoomObject(Path.Combine("PhotonPrefabs", "UI", "Tasks", "Task"), Vector3.zero, Quaternion.identity, 0, initData);
    }

    public void GenerateNewTask(int i)
    {
        countDownTimes[i] = taskDelay;
        countDownBools[i] = true;
    }

    void CreateTask(int i)
    {
        string tag = "Task"+i;
        int productAmount = Random.Range(1, maxProducts+1);
        string[] requiredProducts = GenerateRequiredProducts(productAmount);
        int time = baseTime + (productAmount * amountMultiplier);
        object[] initData = {tag, i, productAmount, requiredProducts, time};
        GameObject taskObj = PhotonNetwork.InstantiateRoomObject(Path.Combine("PhotonPrefabs", "UI", "Tasks", "Task"), Vector3.zero,  Quaternion.identity, 0, initData);
    }

    string[] GenerateRequiredProducts(int productAmount)
    {
        string[] requiredProducts = new string[productAmount];
        for(int i = 0; i < productAmount; i++)
        {
            requiredProducts[i] = ObjectManager.possibleProducts[Random.Range(0, ObjectManager.possibleProducts.Count)];
        }
        return requiredProducts;
    }

    void SetDifficulty()
    {
        taskDelay = (float) PhotonNetwork.CurrentRoom.CustomProperties["taskDelay"];
        baseTime = (int) PhotonNetwork.CurrentRoom.CustomProperties["baseTime"];
        amountMultiplier = (int) PhotonNetwork.CurrentRoom.CustomProperties["amountMultiplier"];
    }
}
