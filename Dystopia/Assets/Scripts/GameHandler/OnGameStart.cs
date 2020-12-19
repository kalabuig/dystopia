﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGameStart : MonoBehaviour
{
    [SerializeField] private GameObject pfMap;

    private void Start() {
        //Check if we are starting the scene to load a game or to create a new game
        if(PersistentData.instance.newGame == false) {
            Debug.Log("loading game...");
            GameObject.Find("SaveLoadGameHandler")?.GetComponent<SaveLoadGame>()?.Load();
        } else {
            Debug.Log("Creating new game...");
            if(pfMap!=null) {
                GameObject mapGO = Instantiate(pfMap, Vector3.zero, Quaternion.identity);
                mapGO.name = "Map";
                mapGO.GetComponent<MapWithoutSectorsHandler>().ActivateRefresh();
            }
        }
    }
}