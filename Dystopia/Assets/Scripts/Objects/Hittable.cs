﻿using UnityEngine;

public class Hittable : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private bool canTakeDamage = true;
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    //private Material matWhite;
    //private Material matDefault;
    private Color colorWhite = new Color(1,1,1,1);
    private Color colorDefault;
    SpriteRenderer spriteRenderer;

    private void Awake() {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        //matWhite = Resources.Load("FlashMaterial", typeof(Material)) as Material;
        //matDefault = spriteRenderer.material;
        colorDefault = spriteRenderer.color;
    }

    public void TakeDamage(int damage) {
        if(canTakeDamage) {
            SoundManager.PlaySound(SoundManager.Sound.WoodenItemHitted);
            currentHealth -= damage;
            //spriteRenderer.material = matWhite;
            spriteRenderer.color = colorWhite;
            if(currentHealth<=0) {
                currentHealth = 0;
                DestroyObject();
            }
            //Invoke("ResetMaterial", .1f);
            Invoke("ResetColor", .1f);
        } else {
            SoundManager.PlaySound(SoundManager.Sound.MetalItemHitted);
        }
    }

    private void ResetMaterial() {
        //spriteRenderer.material = matDefault;
    }

    private void ResetColor() {
        spriteRenderer.color = colorDefault;
    }

    private void DestroyObject(){
        Debug.Log("Destroy object " + this.name);
        //TODO: Destroy object and leave destroyed parts on the ground
        Destroy(this.gameObject);
    }

}