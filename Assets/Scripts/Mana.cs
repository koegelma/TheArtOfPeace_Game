using System.Timers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mana : MonoBehaviour
{
    public float maxMana;
    public float currentMana;

    public Mana(float maxMana){
        this.maxMana = maxMana;
        this.currentMana = maxMana;
    }
    
    void Update(){
    }
    
}
