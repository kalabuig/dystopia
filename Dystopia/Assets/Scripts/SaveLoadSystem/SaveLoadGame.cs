﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SaveLoadMap), typeof(SaveLoadIsland), typeof(SaveLoadBuilding))]
public class SaveLoadGame : MonoBehaviour
{
    private SaveLoadMap saveLoadMap;

    private void Awake() {
        saveLoadMap = GetComponent<SaveLoadMap>();
    }

    public void Save() {
        //MAP
        GameObject map = GameObject.Find("Map");
        if(map!=null) {
            saveLoadMap.Save(map); //save map
        } else {
            Debug.LogError("Map object not found.");
        }
        //PLAYER
        //TODO: Save player stats, skills, inventory and equipped items
    }

    public void Load() {
        //MAP
        GameObject map = GameObject.Find("Map");
        if(map!=null) Destroy(map); //remove actual map
        saveLoadMap.Load(); //load saved map
        //PLAYER
        //TODO: Load player stats, skills, inventory and equipped items
    }
}
