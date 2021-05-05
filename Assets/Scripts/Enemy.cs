using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{  
    public float health;
    public GameObject enemyPrefab;
    public Enemy(){
        this.health = 100;
    }

    void Update(){
        checkHealth();
    }

    public void checkHealth(){
        if(this.health<=0){
            Destroy(this.enemyPrefab);
            Destroy(this);
            Debug.Log(this.name.ToString());
        }
        if(this.health<30){
            this.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
            return;
        }
        if(this.health<60){
            this.GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
            return;
        }
        if(this.health<90){
            this.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
            return;
        }
    }
}
