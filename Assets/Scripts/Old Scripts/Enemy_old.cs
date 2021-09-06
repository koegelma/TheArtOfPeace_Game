using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Damageable))]
public class Enemy_old : MonoBehaviour
{   
    public GameObject enemyPrefab;
    void Update(){
        checkHealth();
    }

    void checkHealth(){
        if(this.GetComponent<Damageable>().health<=0){
            Destroy(gameObject);
            }
        if(this.GetComponent<Damageable>().health<30){
            this.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
            return;
        }
        if(this.GetComponent<Damageable>().health<60){
            this.GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
            return;
        }
        if(this.GetComponent<Damageable>().health<90){
            this.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
            return;
        }
    }
}
