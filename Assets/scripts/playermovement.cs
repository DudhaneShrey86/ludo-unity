using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playermovement : MonoBehaviour
{
  public gamemanager gm;
  public int currentpoint = 0;
  public Vector2 initialpos;
  public int initialpoint = 0;
  public bool isallowedin = false;
  public bool onstart = true;
  public bool myturn = false;
  public bool ismoving = false;
  public bool hitsomething = false;


  void Awake(){
    initialpos = gameObject.transform.position;
  }
  void Start(){
    gm = FindObjectOfType<gamemanager>();
  }
  public void OnMouseDown(){
    if(myturn && !gm.someoneplayed){
      gm.moveplayer(gameObject, currentpoint);
    }
  }
  public void flagfromgm(){
    ismoving = false;
    checkhit();
    // if(lasthit != null){
    //   hitenemy(lasthit);
    // }
    // else{
    //   gm.checkturn(gameObject);
    // }
  }
  public void checkhit(){
    if(hitsomething == false){
      gm.checkturn(gameObject);
    }
  }
  void OnTriggerEnter2D(Collider2D col){
    hitsomething = true;
    IEnumerator hit = checkhitenum(col);
    StartCoroutine(hit);
  }

  // public void hitenemy(Collider2D col){
  //   if(wasmyturn() == true){
  //     if(gameObject.tag != col.gameObject.tag  && ismoving == false){
  //       ismoving = true;
  //       playermovement colscript = col.gameObject.GetComponent<playermovement>();
  //       colscript.currentpoint = colscript.initialpoint;
  //       colscript.onstart = true;
  //       col.gameObject.transform.position = colscript.initialpos;
  //     }
  //     gm.checkturn(gameObject);
  //   }
  //   lasthit = null;
  // }
  IEnumerator checkhitenum(Collider2D col){
    yield return new WaitForSeconds(0.2f);
    Debug.Log(ismoving);
    if(ismoving == false){
      if(wasmyturn() == true){
        if(gameObject.tag != col.gameObject.tag){
          playermovement colscript = col.gameObject.GetComponent<playermovement>();
          colscript.currentpoint = colscript.initialpoint;
          colscript.onstart = true;
          col.gameObject.transform.position = colscript.initialpos;
        }
        gm.checkturn(gameObject);
        hitsomething = false;
      }
    }
  }
  // IEnumerator checkhitenum(Collider2D col){
  //   yield return new WaitForSeconds(0.2f);
  //   if(wasmyturn() == true){
  //     if(gameObject.tag != col.gameObject.tag  && gm.endedturn == true){
  //       Debug.Log(gm.endedturn);
  //       gm.endedturn = false;
  //       playermovement colscript = col.gameObject.GetComponent<playermovement>();
  //       colscript.currentpoint = colscript.initialpoint;
  //       colscript.onstart = true;
  //       col.gameObject.transform.position = colscript.initialpos;
  //       gm.checkturn(gameObject);
  //     }
  //   }
  // }
  public bool wasmyturn(){
    if(gameObject.tag == gm.players[gm.curplayer]){
      return true;
    }
    else{
      return false;
    }
  }
}
