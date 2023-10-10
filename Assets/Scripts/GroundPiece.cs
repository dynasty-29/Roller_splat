using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPiece : MonoBehaviour
{
    //we want to create a code that assign color to every ground piece we have
    public bool isColored = false;
    // giving the ability to change color
    public void ChangeColor(Color color)
    {
        GetComponent<MeshRenderer>().material.color = color;
        isColored = true;

        GameManager.singleton.CheckComplete();
    }
}
