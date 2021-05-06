using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Avatar : MonoBehaviour
{  
    public float health;
    public float maxMana;
    public float currentMana;

    public Avatar(float health, float maxMana){
        this.health = health;
        this.maxMana = maxMana;
        this.currentMana = maxMana;
    }
    public void checkHealth(){}
    void Update(){
        if(this.currentMana<this.maxMana){
            this.currentMana++;
        }
    }
    
}

