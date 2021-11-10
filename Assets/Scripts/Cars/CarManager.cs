// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using System;
// using UnityEngine.Events;


// public class CarManager : MonoBehaviour
// {
//     public static CarManager Instance { private set; get; }
//     public PlayerSignEvent CarSpawn_Event;
//     public GameObject[] CarPrefab;
//     public int CarNum;
//     public Transform[] spawnpoints;
//     public GameObject[] Nodes;
//     public BotManager botManager;

//     void Awake()
//     {
//         botManager = FindObjectOfType<BotManager>();
//         Instance = this;
//         if (CarSpawn_Event == null)
//             CarSpawn_Event = new PlayerSignEvent();
//         CarSpawn_Event.AddListener(spawnCar);
//     }

//     void Start()
//     {

//     }



//     public int i = 0;
//     void spawnCar(PlayerSign sign)
//     {
//         if (CarNum == 0)
//             botManager.FinishCheck();
//         if (CarNum != 0)
//         {
//             if (i == 3) i = 0;
//             GameObject car = Instantiate(CarPrefab[UnityEngine.Random.Range(0, CarPrefab.Length - 1)], spawnpoints[i].position, Quaternion.identity);
//             car.GetComponent<CarController>().path = Nodes[i].transform;
//             car.GetComponent<CarController>().playerSign = sign;
//             i++;
//             CarNum--;
//             Debug.Log("Car Spawned");

//         }
//     } 

// }
// [Serializable] public class PlayerSignEvent : UnityEvent<PlayerSign> { }
