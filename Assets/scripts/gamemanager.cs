using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class gamemanager : MonoBehaviour
{
  public Transform[] pointsarr;
  public IEnumerator movecoroutine;
  public IEnumerator aiplays;
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
  public bool sixflag = false;
  int noofplayersoutside = 0;
  public int[] winpoints = {58, 64, 70, 76};
  GameObject playeroutside;
  public int[] safepoints = {0, 8, 13, 21, 26, 34, 39, 47};
  public GameObject playertomove;
  public bool gotplayer;
  public bool wonthegame;
  public List<string> playerranks = new List<string>();
  string playerlast;
  public GameObject winpanel;

  void Start(){
    dicescript = dice.GetComponent<dice>();
    diceani = dice.GetComponent<Animator>();
  }

  public void dicethrown(int noondice){
    playertomove = null;
    cancontinue = false;
    sixflag = false;
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
    /////////////////aiplays////////////////
    // if(curplayer != 0){
    //   aiplays = aiplayscoroutine(flag, noofplayersoutside, playeroutside);
    //   StartCoroutine(aiplays);
    // }
    // else{
    //   playessentials(flag, noofplayersoutside, playeroutside);
    // }
    aiplays = aiplayscoroutine(flag, noofplayersoutside, playeroutside);
    StartCoroutine(aiplays);
  }
  public void playessentials(int flag,int noofplayersoutside,GameObject playeroutside){
    GameObject[] allplayers = GameObject.FindGameObjectsWithTag(players[curplayer]);
    if(addpos == 6){
      if(flag == 0){
        playrandomplayer();
      }
      if(noofplayersoutside == 4){
        if(shouldskip(allplayers)){
          skipturn();
        }
      }
    }
    else{
      if(flag == 0){
        skipturn();
      }
      if(shouldskip(allplayers)){
        skipturn();
      }
    }
  }
  public bool shouldskip(GameObject[] allplayers){
    bool shouldskipturn = true;
    foreach(GameObject player in allplayers){
      if(player.GetComponent<playermovement>().currentpoint + addpos <= winpoints[curplayer]){
        shouldskipturn = false;
        break;
      }
    }
    return shouldskipturn;
  }
  IEnumerator aiplayscoroutine(int flag, int noofplayersoutside, GameObject playeroutside){
    yield return new WaitForSeconds(0.01f);
    GameObject[] allplayers = GameObject.FindGameObjectsWithTag(players[curplayer]);
    if(addpos == 6){
      if(flag == 0){
        playrandomplayer();
      }
      else{
        if(noofplayersoutside <= 2){
          bringplayeroutside(allplayers);
        }
        else if(noofplayersoutside <= 3){
          if(!checkifwinsorgoesinside(allplayers)){
            if(!killssomeone(allplayers)){
              if(!goestosafepoint(allplayers)){
                bringplayeroutside(allplayers);
              }
            }
          }
        }
        else{
          if(!checkifwinsorgoesinside(allplayers)){
            if(!killssomeone(allplayers)){
              if(!goestosafepoint(allplayers)){
                randomplayorskip(allplayers);
              }
            }
          }
        }
      }
    }
    else{
      if(flag == 0){
        skipturn();
      }
      else{
        if(!checkifwinsorgoesinside(allplayers)){
          if(!killssomeone(allplayers)){
            if(!goestosafepoint(allplayers)){
              randomplayorskip(allplayers);
            }
          }
        }
      }
    }
  }
  public void bringplayeroutside(GameObject[] allplayers){
    gotplayer = false;
    foreach(GameObject player in allplayers){
      if(player.GetComponent<playermovement>().onstart == true){
        playertomove = player;
        gotplayer = true;
        break;
      }
    }
    if(gotplayer){
      autoplay(playertomove);
    }
    else{
      skipturn();
    }
  }
  ////might have to bring return true outside
  public bool checkifwinsorgoesinside(GameObject[] allplayers){
    playermovement playerscript;
    gotplayer = false;
    foreach(GameObject player in allplayers){
      playerscript = player.GetComponent<playermovement>();
      if(playerscript.won != true){
        if(playerscript.currentpoint + addpos == winpoints[curplayer]){
          playertomove = player;
          gotplayer = true;
          break;
        }
        else if((playerscript.currentpoint + addpos >= winpoints[curplayer]-5) && (playerscript.currentpoint + addpos < winpoints[curplayer])){
          playertomove = player;
          gotplayer = true;
          break;
        }
      }
    }
    if(gotplayer == true){
      autoplay(playertomove);
    }
    return gotplayer;
  }
  public bool killssomeone(GameObject[] allplayers){
    playermovement playerscript;
    string playertag = players[curplayer];
    gotplayer = false;
    int futurepos;
    playermovement enemyscript;
    foreach(GameObject player in allplayers){
      playerscript = player.GetComponent<playermovement>();
      futurepos = playerscript.currentpoint + addpos;
      if(!checkifsafe(futurepos)){
        if(playerscript.onstart == false && playerscript.won == false){
          foreach(GameObject enemy in pieces){
            enemyscript = enemy.GetComponent<playermovement>();
            if(enemy.tag != playertag){
              if(enemyscript.currentpoint == futurepos && enemyscript.isinside == false){
                playertomove = player;
                gotplayer = true;
                break;
              }
            }
          }
        }
      }
      if(gotplayer){
        break;
      }
    }
    if(gotplayer){
      autoplay(playertomove);
    }
    return gotplayer;
  }
  public bool goestosafepoint(GameObject[] allplayers){
    gotplayer = false;
    int futurepos;
    playermovement playerscript;
    foreach(GameObject player in allplayers){
      playerscript = player.GetComponent<playermovement>();
      futurepos = playerscript.currentpoint + addpos;
      if(playerscript.won == false){
        if(checkifsafe(futurepos) && playerscript.onstart == false){
          playertomove = player;
          gotplayer = true;
          break;
        }
      }
    }
    if(gotplayer){
      autoplay(playertomove);
    }
    return gotplayer;
  }
  public void randomplayorskip(GameObject[] allplayers){
    bool gotplayer = false;
    playermovement playerscript;
    foreach(GameObject player in allplayers){
      playerscript = player.GetComponent<playermovement>();
      if(playerscript.won == false){
        if(playerscript.currentpoint + addpos < winpoints[curplayer] && playerscript.onstart == false){
          playertomove = player;
          gotplayer = true;
          break;
        }
      }
    }
    if(gotplayer){
      autoplay(playertomove);
    }
    else{
      skipturn();
    }
  }
  public void skipturn(){
    selectplayers(players[curplayer], "normalizeplayers");
    endturn();
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

  //this playes player automatically
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
    if(sixflag){
      cancontinue = true;
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
      // if(curplayer != 0){
      //   autothrow();
      // }
      autothrow();
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
    GameObject sampleplayer = GameObject.FindGameObjectsWithTag(players[curplayer])[0];
    if(sampleplayer.GetComponent<playermovement>().allwon == true){
      endturn();
    }
    // if(curplayer == 3){
    //   curplayer = 0;
    // }
    // else{
    //   curplayer += 1;
    // }
    dicescript.isthrowable = true;
    someoneplayed = true;
    changedicepos();
  }
  public void changedicepos(){
    dice.transform.position = holders[curplayer].position;
    diceani.SetBool("isactive", true);
    diceclone.transform.position = holders[curplayer].position;
    // if(curplayer != 0){
    //   autothrow();
    // }
    autothrow();
  }
  public void autothrow(){
    dicescript.autodice();
  }
  public void checkifwholewon(string playertag){
    GameObject[] allplayers = GameObject.FindGameObjectsWithTag(playertag);
    wonthegame = true;
    foreach(GameObject player in allplayers){
      if(player.GetComponent<playermovement>().won == false){
        wonthegame = false;
        break;
      }
    }
    if(wonthegame){
      playerwon(playertag, allplayers);
    }
  }
  public void playerwon(string ptag, GameObject[] allplayers){
    playerranks.Add(ptag);
    GameObject[] currentplayers = GameObject.FindGameObjectsWithTag(ptag);
    foreach(GameObject player in currentplayers){
      player.GetComponent<playermovement>().allwon = true;
    }
    if(playerranks.Count == 3){
      addlastplayer(allplayers);
    }
  }
  public void addlastplayer(GameObject[] allplayers){
    foreach(GameObject player in allplayers){
      if(player.GetComponent<playermovement>().won == false){
        playerlast = player.tag;
        break;
      }
    }
    playerranks.Add(playerlast);
    finishgame();
  }
  public void finishgame(){
    stopgame();
    winpanel.SetActive(true);
  }
  public void stopgame(){
    
  }

  IEnumerator moveplayerroutine(GameObject player, int curpos){
    stopgame();
    if(elsecanmove){
      selectplayers(player.tag, "normalizeplayers");
      elsecanmove = false;
      wonthegame = false;
      sixflag = false;
      bool cangoin = false;
      int goatpoint = 0;
      int goinpoint = 0;
      bool haswon = false;
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
        sixflag = true;
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
            playerscript.isinside = true;
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
          if(curpos >= winpoints[curplayer]){
            haswon = true;
            break;
          }
          yield return new WaitForSeconds(0.1f);
        }
        if(haswon){
          playerscript.haswon();
        }
      }
      playerscript.currentpoint = curpos;
      checkturn(player, playerscript);
    }
  }
}
