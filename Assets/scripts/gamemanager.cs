using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class gamemanager : MonoBehaviour
{
  public Transform[] pointsarr;
  public IEnumerator movecoroutine;
  public bool elsecanmove = true;
  public GameObject dice;
  public GameObject diceclone;
  Animator diceani;
  dice dicescript;
  public GameObject[] pieces;
  public string[] players;
  public Transform[] holders;
  public int curplayer = 0;
  public int addpos = 0;
  public bool someoneplayed = false;
  public bool cancontinue = false;
  int noofplayersoutside = 0;
  GameObject playeroutside;
  public int[] safepoints = {0, 8, 13, 21, 26, 34, 39, 47};

  void Start(){
    dicescript = dice.GetComponent<dice>();
    diceani = dice.GetComponent<Animator>();
  }

  public void dicethrown(int noondice){
    cancontinue = false;
    noofplayersoutside = 0;
    int flag = 0;
    addpos = noondice;
    foreach(GameObject piece in pieces){
      playermovement playerscript = piece.GetComponent<playermovement>();
      if(piece.tag == players[curplayer]){
        playerscript.myturn = true;
        if(playerscript.onstart != true){
          flag = 1;
          noofplayersoutside += 1;
          if(noofplayersoutside == 1){
            playeroutside = piece;
          }
        }
      }
      else{
        playerscript.myturn = false;
      }
    }
    dicescript.isthrowable = false;
    someoneplayed = false;
    if(flag == 0){
      if(addpos == 6){
        playrandomplayer();
      }
      else{
        endturn();
      }
    }
    if(noofplayersoutside == 1 && addpos != 6){
      autoplay(playeroutside);
    }
  }
  public void playrandomplayer(){
    GameObject randplayer = GameObject.FindGameObjectsWithTag(players[curplayer])[0];
    autoplay(randplayer);
  }
  public void autoplay(GameObject playeroutside){
    int playerpos = playeroutside.GetComponent<playermovement>().currentpoint;
    moveplayer(playeroutside, playerpos);
  }

  public void moveplayer(GameObject player, int curpos){
    movecoroutine = moveplayerroutine(player, curpos);
    StartCoroutine(movecoroutine);
  }
  public void checkturn(GameObject player, playermovement playerscript){
    string playertag = player.tag;
    int playerpos = playerscript.currentpoint;
    bool res = checkifsafe(playerpos);
    if(!res){
      foreach(GameObject piece in pieces){
        if(piece.tag != playertag){
          playermovement enemyscript = piece.GetComponent<playermovement>();
          if(enemyscript.currentpoint == playerpos){
            enemyscript.gotkilled();
            playerscript.isallowedin = true;
            cancontinue = true;
          }
        }
      }
    }
    checkifcontinue();
  }
  public bool checkifsafe(int playerpos){
    bool safeflag = false;
    foreach(int point in safepoints){
      if(point == playerpos){
        safeflag = true;
        break;
      }
    }
    return safeflag;
  }
  public void checkifcontinue(){
    if(cancontinue == true){
      dicescript.isthrowable = true;
      dicescript.ani.SetBool("isactive", true);
      elsecanmove = true;
      someoneplayed = true;
    }
    else{
      endturn();
    }
  }
  public void endturn(){
    elsecanmove = true;
    curplayer += 1;
    if(curplayer >= 4){
      curplayer = 0;
    }
    dicescript.isthrowable = true;
    someoneplayed = true;
    changedicepos();
  }
  public void changedicepos(){
    dice.transform.position = holders[curplayer].position;
    diceani.SetBool("isactive", true);
    diceclone.transform.position = holders[curplayer].position;
    autothrow();
  }
  public void autothrow(){
    dicescript.autodice();
  }
  IEnumerator moveplayerroutine(GameObject player, int curpos){
    if(elsecanmove){
      elsecanmove = false;
      bool cangoin = false;
      int goatpoint = 0;
      int goinpoint = 0;
      playermovement playerscript = player.GetComponent<playermovement>();
      //check if it can go in
      if(playerscript.isallowedin == true){
        string playertag = player.tag;
        cangoin = true;
        if(playertag == "bluepiece"){
          goatpoint = 50;
          goinpoint = 52;
        }
        else if(playertag == "redpiece"){
          goatpoint = 11;
          goinpoint = 58;
        }
        else if(playertag == "greenpiece"){
          goatpoint = 24;
          goinpoint = 64;
        }
        else if(playertag == "yellowpiece"){
          goatpoint = 37;
          goinpoint = 70;
        }
      }
      if(addpos == 6){
        cancontinue = true;
      }
      if(playerscript.onstart){
        if(addpos == 6){
          playerscript.onstart = false;
          player.transform.position = pointsarr[curpos].position;
        }
      }
      else{
        while(addpos != 0){
          if(cangoin == true && curpos == goatpoint){
            curpos = goinpoint;
            addpos -= 1;
          }
          else{
            curpos += 1;
            addpos -= 1;
            if(curpos == 52 && cangoin == false){
              curpos = 0;
            }
          }
          player.transform.position = pointsarr[curpos].position;
          yield return new WaitForSeconds(0.15f);
        }
      }
      playerscript.currentpoint = curpos;
      checkturn(player, playerscript);
    }
  }
}
