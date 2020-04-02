using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tempai : MonoBehaviour
{
  public int currentpoint = 0;
  void Start(){
    currentpoint = 25;
    FindObjectOfType<gamemanager>().moveplayer(gameObject, currentpoint);
  }
}
