using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaRegeneration : MonoBehaviour
{
    public float duration;
    public float strength;
    float startTime;
    float manaTimestamp = 0f;
    float interval = 0.01f;
    void Update(){
        if(manaTimestamp <= Time.time && duration > 0){
            if(this.GetComponent<Mana>().currentMana + strength*interval > this.GetComponent<Mana>().maxMana){
                this.GetComponent<Mana>().currentMana = this.GetComponent<Mana>().maxMana;
            }
            else{
                this.GetComponent<Mana>().currentMana += strength*interval;
            }
            duration -= interval;
            manaTimestamp = Time.time + interval;
        }
        if(duration<=0){
            Destroy(this.GetComponent<ManaRegeneration>());
        }
    }

    /* public ManaRegeneration(float duration, float strength){
        this.startTime = Time.time;
        this.duration = duration;
        this.strength = strength;
    } */
    
}
