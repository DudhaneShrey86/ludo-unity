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
  int[] winpoints = {58, 64, 70, 76};
  GameObject playeroutside;
  public int[] safepoints = {0, 8, 13, 21, 26, 34, 39, 47};
  Vector2 normalsize

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
        piece.GetComponent<SpriteRenderer>().sortingOrder = 1;
        showeligibleplayers(piece.tag);
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
        selectplayers(players[curplayer], "normalizeplayers");
        endturn();
      }
    }
    if(noofplayersoutside == 1 && addpos != 6){
      autoplay(playeroutside);
    }
  }

  public void showeligibleplayers(string tagname){
    GameObject[] selectedplayers = GameObject.FindGameObjectsWithTag(tagname);
    playermovement playerscript;
    foreach(GameObject player in selectedplayers){
      playerscript = player.GetComponent<playermovement>();
      if(playerscript.onstart == true){
        if(addpos == 6){
          player.GetComponent<Animator>().SetBool("iseligible", true);
        }
      }
      else{
        if(winpoints[curplayer] - playerscript.currentpoint >= addpos){
          player.GetComponent<Animator>().SetBool("iseligible", true);
        }
      }
    }
  }
  public void selectplayers(string tagname,string action){
    GameObject[] selectedplayers = GameObject.FindGameObjectsWithTag(tagname);
    if(action == "normalizeplayers"){
      foreach(GameObject selectedplayer in selectedplayers){
        selectedplayer.GetComponent<SpriteRenderer>().sortingOrder = 0;
        selectedplayer.GetComponent<Animator>().SetBool("iseligible", false);
      }
    }
    if(action == "allowin"){
      foreach(GameObject selectedplayer in selectedplayers){
        selectedplayer.GetComponent<playermovement>().isallowedin = true;
      }
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
    if(winpoints[curplayer] - curpos >= addpos){
      StartCoroutine(movecoroutine);
    }
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
            if(playerscript.isallowedin == false){
              selectplayers(playertag, "allowin");
            }
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
  public void checkifwon(GameObject player){
    player.GetComponent<playermovement>().won = true;
    int flag = 1;
    GameObject[] players = GameObject.FindGameObjectsWithTag(player.tag);
    foreach(GameObject p in players){
      if(p.GetComponent<playermovement>().won == false){
        flag = 0;
        break;
      }
    }
    if(flag == 1){
      wongame(player);
    }
  }
  public void wongame(GameObject player){
    Debug.Log("Won the game");
  }
  IEnumerator moveplayerroutine(GameObject player, int curpos){
    if(elsecanmove){
      selectplayers(player.tag, "normalizeplayers");
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
          goinpoint = 53;
        }
        else if(playertag == "redpiece"){
          goatpoint = 11;
          goinpoint = 59;
        }
        else if(playertag == "greenpiece"){
          goatpoint = 24;
          goinpoint = 65;
        }
        else if(playertag == "yellowpiece"){
          goatpoint = 37;
          goinpoint = 71;
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
          if(curpos == winpoints[curplayer]){
            checkifwon(player);
            break;
          }
          if(cangoin == true && curpos == goatpoint){
            curpos = goinpoint;
            addpos -= 1;
          }
          else{
            curpos += 1;
            addpos -= 1;
            if(curpos == 52){
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
