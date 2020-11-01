﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class ScavengingInventory : Inventory
{
     [SerializeField] Slider progressSlider;
     [SerializeField] Text progressText;
     [SerializeField] ItemAssets itemAssets;

    //Management of scavenging time
    private int scavengeTick;
    private int scavengeTickMax;
    private bool isScavenging;

    public void Scavenge(int ticksToScavenge) {
        scavengeTickMax = ticksToScavenge;
        scavengeTick = 0;
        isScavenging = true;
        ResetProgress();
        UnSuscribe(); 
        TimeTickSystem.OnTick += TimeTickSystem_OnTick;  //Suscribe to time tick system
    }

    public void StopScavenge() {
        ResetProgress();
        UnSuscribe(); //unsubscribe from the tick system
    }

    private void ResetProgress() {
        progressText.text = "0%";
        progressSlider.value = 0;
    }

    public void loadItems(Container c) {
        if(c==null) return;
        //Reset the slots
        foreach(ItemSlot slot in itemSlots) {
            slot.item = null;
            slot.amount = 0;
        }
        Container.ContainerItem[] itemsToLoad = c.GetItems(); //Get the items to load in the scavenging panel
         int i = 0;
        //Populate slots with items
        for(; i<itemsToLoad.Length && i < itemSlots.Length; i++) {
            if(itemsToLoad[i].item!=null && itemsToLoad[i].amount>0) {
                itemSlots[i].item = itemsToLoad[i].item.GetCopy(); //Instantiate a new item based on the scriptobject
                itemSlots[i].amount = itemsToLoad[i].amount; //Setting the amount
            }
        }
        //Populate slots with empty
        for(; i < itemSlots.Length; i++) {
            itemSlots[i].item = null;
            itemSlots[i].amount = 0; //Setting the amount to 0
        }
    }

    public void storeItems(Container c) {
        if(c==null) return;
        //Create the structure to store the items in the structure
        Container.ContainerItem[] itemsToStore = new Container.ContainerItem[itemSlots.Length];
        for(int i = 0; i < itemSlots.Length; i++) {
            itemsToStore[i] = new Container.ContainerItem { item = itemSlots[i].item, amount = itemSlots[i].amount};
        }
        c.SetItems(itemsToStore);
    }

    private void TimeTickSystem_OnTick(object sender, TimeTickSystem.OnTickEventArgs e) {
        if(isScavenging) {
            scavengeTick += 1;
            if(scavengeTick>=scavengeTickMax) { //Scavenging finished
                isScavenging = false;
                UnSuscribe(); //unsubscribe from the tick system
                Item scavengedItem = itemAssets.GetRandomItem();
                if(scavengedItem!=null) {
                    AddItem(scavengedItem); //Add item to the scavenging inventory/panel
                }
            }
            progressText.text = (scavengeTick * 100f / scavengeTickMax).ToString() + "%";
            progressSlider.value = (scavengeTick * 1f / scavengeTickMax);
        }
    }

    private void UnSuscribe() {
         TimeTickSystem.OnTick -= TimeTickSystem_OnTick;
    }

    private void OnDestroy() {
        UnSuscribe();
    }
}