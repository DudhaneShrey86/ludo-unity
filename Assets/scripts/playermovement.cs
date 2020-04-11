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
  public bool won = false;
  public bool allwon = false;
  public bool isinside = false;
  void Awake(){
    initialpos = gameObject.transform.position;
  }
  void Start(){
    gm = FindObjectOfType<gamemanager>();
  }
  public void OnMouseDown(){
    if(myturn && !gm.someoneplayed && !won){
      gm.moveplayer(gameObject, currentpoint);
    }
  }
  public void gotkilled(){
    currentpoint = initialpoint;
    gameObject.transform.position = initialpos;
    onstart = true;
  }
  public void haswon(){
    won = true;
    Debug.Log("won");
    gm.checkifwholewon(gameObject.tag);
  }

}
