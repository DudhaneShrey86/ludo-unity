using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dice : MonoBehaviour
{
  public GameObject diceclone;
  public int step;
  public gamemanager gm;
  public bool isthrowable = true;
  public Sprite[] dicefaces;
  public Sprite none;
  public Animator ani;
  void Start(){
    ani = gameObject.GetComponent<Animator>();
    gm = FindObjectOfType<gamemanager>();
    ani.SetBool("isactive", true);
  }
  void OnMouseDown(){
    StartCoroutine("throwdice");
  }
  public void autodice(){
    StartCoroutine("throwdice");
  }
  IEnumerator throwdice(){
    if(isthrowable){
      ani.SetBool("isactive", false);
      diceclone.SetActive(true);
      gameObject.GetComponent<SpriteRenderer>().sprite = none;
      yield return new WaitForSeconds(0.2f);
      diceclone.SetActive(false);
      step = Random.Range(1, 7);
      gameObject.GetComponent<SpriteRenderer>().sprite = dicefaces[step-1];
      yield return new WaitForSeconds(0.2f);
      gm.dicethrown(step);
    }
    else{
      Debug.Log("Finish current turn");
    }
  }
}
