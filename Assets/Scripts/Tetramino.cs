using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public enum TetraminoTypes
{
    L_MIRRORED,
    L,
    T,
    SQUARE,
    I,
    Z,
    Z_MIRRORED
}


/// <summary>
/// 
/// Lo script della Section si occupa di updatare il pezzo,
/// in quanto solo il pezzo "posseduto" dal player si deve muovere.
/// 
/// Quale pezzo viene posseduto, e dunque, 
/// quale viene updatato viene gestito dalla Section.
/// 
/// Ogni tetramino ha una posizione "centrale" sulla grigilia in modo
/// tale da poterla spostare.
/// Nonostante cio', Ogni blocchetto verra' singolarmente calcolato sulla griglia.
/// 
/// </summary>
/// 


/*
* Ogni tetramino creera' la sua "map" attraverso il valore di "type"
* al momento della creazione
* Le rotazioni di ogni tetramino sono hardcoded.
*/
public class Tetramino : MonoBehaviour
{

    [SerializeField] public List<GameObject> blocks;
    [SerializeField] public GameObject rotationPivot;
    [NonSerialized] public RectTransform rectTrans;

    /*
    *
    * Vale la logica degli indici, dunque si parte da 0
    *
    */
    [NonSerialized] public int gridPosY = 0;
    [NonSerialized] public int gridPosX = 0;


    [SerializeField] public TetraminoTypes type;

    private Tetramino()
    {

    }

    public void DestroyBlock(GameObject block)
    {
        if(blocks.Contains(block))
        {
            blocks.Remove(block);
            Destroy(block);
            Debug.Log("Block Destroyed!");
        }
        else
        {
            Debug.LogError("Block not found: " + block.name);
        }
    }

    void SetColorBaseOnType()
    {
        Color chosenColor = Color.white;
        switch(type)
        {
            case TetraminoTypes.I:
            chosenColor = Color.red;
            break;

            case TetraminoTypes.L_MIRRORED:
            chosenColor = Color.cyan;
            break;

            case TetraminoTypes.L:
            chosenColor = Color.yellow;
            break;

            case TetraminoTypes.Z:
            chosenColor = Color.magenta;
            break;

            case TetraminoTypes.Z_MIRRORED:
            chosenColor = Color.gray;
            break;

            case TetraminoTypes.SQUARE:
            chosenColor = Color.green;
            break;

            case TetraminoTypes.T:
            chosenColor = Color.blue;
            break;
            
        }

        foreach(GameObject blk in blocks)
        {
            blk.GetComponent<SpriteRenderer>().color = chosenColor;
        }
    }

    // ? --- Potrei semplicemente controllare il childCount del block father
    // ? --- ma oggettivamente sono troppo fico per farlo 
    // TODO: non so perche' abbia fatto questa cosa lol, sistema
    IEnumerator DestroyMeRoutine()
    {
        while(true)
        {
            bool areAllFalse = true;
            foreach(GameObject blk in blocks)
            {
                if(blk != null)
                {
                    areAllFalse = false;
                }
            }

            if(areAllFalse)
                Destroy(gameObject);

            yield return new WaitForSeconds(1f);
        }
    }

    void Awake()
    {
        //SetColorBaseOnType();
    }

    void Start()
    {
        StartCoroutine(DestroyMeRoutine());
    }
}