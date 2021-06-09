using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    public float health;
    public void takeDamage(float damageValue){
        this.health = this.health - damageValue;
    }
}

