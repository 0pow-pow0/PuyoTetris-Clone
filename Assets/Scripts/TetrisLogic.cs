using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using EditorAttributes;
using System.Linq;

public enum GridMapValues
{
    INVALID = '0',
    EMPTY = ' ',
    WALL = 'X',
    TETRAMINO = 'O'
};

// public enum TetraminoCollisionType
// {
//     NO_COLLISION = 0,
//     X_COLLISION,
//     Y_COLLISION,
//     XY_COLLISION,
// }

public class TetrisLogic : MonoBehaviour
{

    GridMapValues[,] gridMap;
    
    //* -- Estremamente importante per eliminare le righe
    //* -- o per effettuare qualunque manipolazione alla griglia
    //* TODO: commenta meglio
    GameObject[,] blocksGridMapRef;



    // -------------------------------------------
    // ! Grid Logic
    // -------------------------------------------
    #region Grid Logic


    /* --------------- Grid Settings -------------- */

    [field: Header("Grid Logic"), SerializeField] 
    public float gridSquareSize { get; private set; } // In unita' di Unity xD
    [field: SerializeField]

    public Transform gridOrigin { get; private set; } 

    [field: SerializeField]
    public int gridWidthInSquares { get; private set; }   // Calcolata in quadrati
    
    [field: SerializeField]
    public int gridHeightInSquares { get; private set; }  // Calcolata in quadrati

    [field: SerializeField]
    private GameObject tetraminoSpawnPoint;

    [field: SerializeField]
    public GameObject tetraminosPiecesWrapper { get; private set; }
    
    /* --------------------- - -------------------- */


    /* --------------- Grid Methods --------------- */

    /// <summary>
    /// 
    /// </summary>
    /// <param name="squareX">Calcolato come gli indici degli array</param>
    /// <param name="squareY">Calcolato come gli indici degli array</param>
    /// <returns></returns>
    /*
    * TODO: Cambia questo se vuoi modificare la logica di spostamento,
    * ad esempio se vuoi che calcoli il movimento in pixel su schermo
    */
    Vector3 GridSquareBottomLeftToWorldCoords(int squareX, int squareY)
    {

        if (squareX < 0)
            UtilityShit.Log("Indice X griglia minore di 0!", Color.red);

        if (squareY < 0)
            UtilityShit.Log("Indice Y griglia minore di 0!", Color.red);

        if (squareX >= gridWidthInSquares)
            UtilityShit.Log("Indice X griglia maggiore della larghezza!", Color.red);

        if (squareY >= gridHeightInSquares)
            UtilityShit.Log("Indice Y griglia maggiore della lunghezza!", Color.red);


        /*
        * TODO: Fai in modo che sia centrato con qualunque grandezza
        */
        Vector3 resultPos =
            new Vector3
            (
                gridOrigin.transform.position.x + (squareX * gridSquareSize), //+ 0.5f,
                                                                                // La grid e' invertita
                gridOrigin.transform.position.y - (squareY * gridSquareSize),// + 0.5f,
                gridOrigin.transform.position.z
            );

        //Debug.Log("Conversion result: " + resultPos);
        return resultPos;
    }

    Vector3 GridSquareCenterToWorldCoords(int squareX, int squareY)
    {

        if (squareX < 0)
            UtilityShit.Log("Indice X griglia minore di 0!", Color.red);

        if (squareY < 0)
            UtilityShit.Log("Indice Y griglia minore di 0!", Color.red);

        if (squareX >= gridWidthInSquares)
            UtilityShit.Log("Indice X griglia maggiore della larghezza!", Color.red);

        if (squareY >= gridHeightInSquares)
            UtilityShit.Log("Indice Y griglia maggiore della lunghezza!", Color.red);


        /*
        * TODO: Fai in modo che sia centrato con qualunque grandezza
        */
        Vector3 resultPos =
            new Vector3
            (
                gridOrigin.transform.position.x + ((squareX * gridSquareSize)) + gridSquareSize/2, //+ 0.5f,
                                                                                // La grid e' invertita
                gridOrigin.transform.position.y - ((squareY * gridSquareSize)) - gridSquareSize/2,// + 0.5f,
                gridOrigin.transform.position.z
            );

        //Debug.Log("Conversion result: " + resultPos);
        return resultPos;
    }

    /**
    * ! Non tiene conto di eventuali "overflow" nella griglia.
    * ! Questo significa che se il calcolo restituisce un blocco oltre
    * ! la larghezza della griglia non da errore 
    */
    int WorldCoordsToGridX(float x)
    {
        /*
        //* -- Questa funziona non funziona proprio benissimo lol
        //* -- Se il numero e' negativo arrotonda per eccesso SENZA MOTIVO
        return Mathf.FloorToInt
        (
            X
        );*/
        /*
        * TODO:
        * Qui i casi speciali dell'asse Y non si applicano,
        * ma per debug purposes, potrebbe essere utile aggiungere robe?
        */



        //* -- E' NECESSARIO tenerla in conto, altrimenti se si sposta la griglia
        //* -- si rompe completamente il gioco.
        //* -- In sostanza, stiamo rendendo la griglia "l'origine del mondo"
        float accordingToGridOriginX = x - gridOrigin.transform.position.x;
        //* In questo caso non dobbiamo applicare nessuna condizione
        //* seguiamo la stessa direzione dell'asse del mondo.
        
        // ? --- che te posso dire, se no mi dice che e' dentro
        // ? --- solo in questo caso
        if(accordingToGridOriginX < 0 && 
            accordingToGridOriginX > -1)
        {
            return -1;
        }

        return (int)accordingToGridOriginX;
    }

    /**
    * ! Non tiene conto di eventuali "overflow" nella griglia.
    * ! Questo significa che se il calcolo restituisce un blocco oltre
    * ! la lunghezza della griglia non da errore 
    */
    int WorldCoordsToGridY(float y)
    {
        /*
        //* -- Questa funziona non funziona proprio benissimo lol
        //* -- Se il numero e' negativo arrotonda per eccesso SENZA MOTIVO
        return -Mathf.FloorToInt
        (
            y
        );*/
        /*
        * TODO: 
        * 1. fai una conversione dei valori che calcoli 
        *    se si trovi fuori dalla griglia, poi logga se vero.
        * 2. fai in modo che dipenda dall'origine della griglia.
        *    Al momento spostare la griglia sminchia tutto.
        */
        /*
        * IMPORTANTE: e' molto importante ricordare che i pezzi si
        * muovono dall'alto verso il basso, questo significa che nelle
        * coordinate del mondo SCENDIAMO, quando, pero', nella griglia
        * stiamo AUMENTANDO di un blocco verso il basso. 
        * Questo perche' l'origine della griglia e' in alto a sinitra.
        */
        //* -- E' NECESSARIO tenerla in conto, altrimenti se si sposta la griglia
        //* -- si rompe completamente il gioco.
        //* -- In sostanza, stiamo rendendo la griglia "l'origine del mondo" 
        float accordingToGridOriginY = 
            y - gridOrigin.transform.position.y;
        //Debug.Log("ReqY: " + y + " " + accordingToGridOriginY);
      
        //* -- SCENDIAMO nel mondo, quindi, AUMENTIAMO nella griglia
        if (y <= gridOrigin.transform.position.y)
        {  
            //Debug.Log("Y: " + Math.Abs(((int)accordingToGridOriginY)));
            //* -- Adesso calcoliamo la posizione nella griglia
            //* -- CONSIDERANDO la griglia come "origine" del calcolo
            return Math.Abs(((int)accordingToGridOriginY));
        }
        else//else if (y > gridOrigin.transform.position.y)
        {
            //Debug.Log("Y: " + -((int)accordingToGridOriginY));

            return -((int)accordingToGridOriginY);
        }



        //Debug.Log("La logica dice che fino a qui non puoi arrivare" +
        //"se ci sei arrivato, oh merda come cazzo hai fatto");
        //return 0;
    }

    string GetGridMapValuesAsString()
    {
        string toPrint = "";
        for (int y = 0; y < gridHeightInSquares; y++)
        {
            for (int x = 0; x < gridWidthInSquares; x++)
            {
                toPrint += gridMap[y, x];
            }
            toPrint += "\n";
        }
        toPrint += "\n";

        return toPrint;
    }

    string GetGridRefsAsString()
    {
        string toPrint = "";
        for (int y = 0; y < gridHeightInSquares; y++)
        {
            for (int x = 0; x < gridWidthInSquares; x++)
            {
                if(blocksGridMapRef[y, x] != null)
                    toPrint += "O";
                else
                    toPrint += "X";
            }
            toPrint += "\n";
        }
        toPrint += "\n";

        return toPrint;
    }

    /**
    * * Inizializza gridMap con valori vuoti e
    * * blocksGridMapRef a valori nulli
    */
    void InitGridMapAndBlockGridMap()
    {
        for (int y = 0; y < gridHeightInSquares; y++)
        {
            for (int x = 0; x < gridWidthInSquares; x++)
            {
                gridMap[y, x] = GridMapValues.EMPTY;
                blocksGridMapRef[y, x] = null;
            }
        }
    }

    /// <summary>
    /// Abbassa quelle righe che possiedono spazi vuoti consecutivi
    /// TODO Si puo' ottimizzare partendo da una data riga
    /// </summary>
    void TranslateDown(int startLineY)
    {
        for(int rowX = 0; rowX < gridWidthInSquares; rowX++)
        {
            for(int y = startLineY; y >= 1; y--)
            {   
                int higherY = y - 1;

                GameObject nextGO = null;
                if(blocksGridMapRef[higherY, rowX] != null)
                {
                    nextGO =
                        blocksGridMapRef[higherY, rowX];
                }
                
                if(nextGO == null)
                    continue;
                
                GridMapValues temp = gridMap[y, rowX];
                // ? --- Attuale = quello subito sopra
                gridMap[y, rowX] = gridMap[higherY, rowX];
                gridMap[higherY, rowX] = temp;
            
                // ? --- Spostiamolo sotto
                nextGO.transform.position =
                    GridSquareCenterToWorldCoords(rowX, y);
                //Debug.Log("Y: " + y + " higherY: " + higherY + " rowX: " + rowX);
                //Debug.Log("Pos: " + nextGO.transform.position);

                GameObject actual = blocksGridMapRef[y,rowX];
                if(actual != null)
                {
                    actual.transform.position =
                        GridSquareCenterToWorldCoords(rowX,higherY);
                }

                blocksGridMapRef[y, rowX] = 
                    blocksGridMapRef[higherY, rowX];
                blocksGridMapRef[higherY, rowX] = 
                    actual;
                

            }
        
        }
            Debug.Log(GetGridRefsAsString());
    }
    
    void TranslateDownMultiple(List<int> indicesToDel)
    {
        // ? --- Necessario per evitare bug bisogna partire dalla
        // ? --- linea piu' alta. 
        indicesToDel.Sort();
        foreach(int indices in indicesToDel)
        {
            TranslateDown(indices);
        }
    }

    //* -- TODO: commenta
    /// <summary>
    /// Non si preoccupa di rimuovere le reference dei gameobject
    /// da blocksGridMapRef e di firare l'evento lineClear.
    /// </summary>
    /// <param name="lineYIndex"></param>
    /// <returns>Lista delle reference dei blocchi eliminati</returns>
    List<(GameObject blockRef, int xIndex, int yIndex)> DeleteLineFromCollisionMap(int lineYIndex)
    {
        if(lineYIndex >= gridHeightInSquares)
        {
            Debug.LogError("Index line out of bounds");
            return default;
        }

        Debug.Log(GetGridMapValuesAsString());
        Debug.Log(GetGridRefsAsString());
        //* -- TODO: ottimizza con un'array contenente reference 
        //* -- ai blocchi
        List<(GameObject blockRef, int xIndex, int yIndex)>
        blocksAndIndex = new();

        for(int x = 0; x < gridWidthInSquares; x++)
        {
            gridMap[lineYIndex, x] = GridMapValues.EMPTY; 
            
            if(blocksGridMapRef[lineYIndex, x].gameObject ==
                null)
            {
                Debug.LogError("Null oggetto block refs");
            }

            Debug.Log("Destroying " + "i: " + x
                + blocksGridMapRef[lineYIndex, x].gameObject.name
                + " of "
                + blocksGridMapRef[lineYIndex, x].gameObject
                .GetComponentInParent<Tetramino>().gameObject.name);
            
            // Destroy
            // (
            //     blocksGridMapRef[lineYIndex, x].gameObject
            // );

            blocksAndIndex.Add((blocksGridMapRef[lineYIndex, x], lineYIndex, x));
            // tr.DestroyBlock(
            //     blocksGridMapRef[lineYIndex, x].gameObject);

            // blocksGridMapRef[lineYIndex, x] = null;

        
        }

        return blocksAndIndex;
        //onLineClear.Invoke(blocksAndIndex, LINE_CLEAR_ANIMATION_TIME);

        // PowUtilityU.Get().DelayAction
        // (
        //     () =>
        //     {
        //         foreach(var blocksTD in blocksAndIndex)
        //         {
        //             Destroy(blocksTD.blockRef);
        //         }
                
        //         TranslateDown(lineYIndex);
        //     },  
        //     LINE_CLEAR_ANIMATION_TIME
        // );

        
    }

    void DeleteLineFromReferenceMap(GameObject blockRef, int yIndex, int xIndex)
    {
        Tetramino tr = 
            blockRef.GetComponentInParent<Tetramino>();
        
        tr.DestroyBlock(blockRef);

        blocksGridMapRef[yIndex, xIndex] = null;
    }
 

    void DeleteLinesFromReferenceMap(List<(GameObject blockRef, int xIndex, int yIndex)> blocksRefs)
    {
        foreach(var bRef in blocksRefs)
        {
            DeleteLineFromReferenceMap
            (
                bRef.blockRef,
                bRef.yIndex,
                bRef.xIndex
            );
        }

        Debug.Log("Linee Eliminate");
    }

    /**
    * CheckGridLineFill
    * * Controlla se una linea della griglia e' riempita di tetramini
    */
    bool CheckGridLineFill(int lineYIndex)
    {
        /* -------------- Error handling -------------- */
        if (lineYIndex < 0)
        {
            Debug.Log("Valore parametro lineYIndex inferiore a 0 | " + lineYIndex);
            return false;
        }

        else if (lineYIndex > gridHeightInSquares)
        {
            Debug.Log("Valore parametro lineYIndex superiore" +
                "a lunghezza griglia | " + lineYIndex);
            return false;
        }
       


        bool isFill = true;
        for(int x = 0; x < gridWidthInSquares; x++)
        {
            if(GetGridMapValueAt(lineYIndex, x) 
                != GridMapValues.TETRAMINO)
            {
                isFill = false;
            }
        }
        
        return isFill;
    }


    #endregion
    
    // -------------------------------------------
    // ! Gameplay Stats
    // -------------------------------------------
    
    #region Gameplay Stats
    [Header("Gameplay Stats"), SerializeField, EditorAttributes.ReadOnly]
    Tetramino activePiece;
    Tetramino[] placedPieces;

    // ? --- Quanti quadrati cade al secondo?
    //[SerializeField]
    //float tetraminoFallSpeedSquaresPerSecond = 1f;

    // ? --- Tempo di delay tra un'input e l'altro
    [SerializeField]
    float INPUT_DELAY = 0.17f;
    float inputDelayTimer = 0f;


    [SerializeField]
    float TETRAMINO_FALL_SPEED;
    // ? --- Quando premiamo s andiamo piu' veloci
    [SerializeField]
    float TETRAMINO_FALL_SPEED_ON_PRESS;
    float activeTetraminoFallSpeed;
    float tetraminoFallTimer;

    //[SerializeField] Stopwatch _tetraminoFallSpeed;

    #endregion



    // -------------------------------------------
    // ! Tetramino Logic
    // -------------------------------------------

    #region Tetramino Logic
    int inputTraslX = 0;
    int inputTraslY = 0;
    void RetrieveInput()
    {

        // ? --- Non si spammera' alla velocita' della luce 
        // ? --- poiche' la funzione MovePiece e' limitata da
        // ? --- INPUT_DELAY
        //* -- Destra
        if (Keyboard.current.aKey.isPressed
            || TouchManager.thisFrameTouchInput == DetectedTouchInput.SWIPE_LEFT)
        {
            inputTraslX = -1;

            // ? --- In questo modo se spammo il tasto
            // ? --- non viene influenzato dal delay
            // ? --- TOGLIENDO la sensazione di inputlag
            inputDelayTimer = inputDelayTimer + 0.001f; // 0.001f perhce' mi cago
        }

        // ? --- Same shit qui
        // * --- Sinistra
        if (Keyboard.current.dKey.isPressed
            || TouchManager.thisFrameTouchInput == DetectedTouchInput.SWIPE_RIGHT)
        {
            inputTraslX = 1;
            //TODO: Potrei fare in modo che parta lento e poi 
            //TODO: acceleri.
            // ? --- In questo modo se spammo il tasto
            // ? --- non viene influenzato dal delay
            // ? --- TOGLIENDO la sensazione di inputlag
            inputDelayTimer = inputDelayTimer + 0.001f; // 0.001f perhce' mi cago
        }

        if(Keyboard.current.sKey.isPressed)
        {
            activeTetraminoFallSpeed = TETRAMINO_FALL_SPEED_ON_PRESS;
            Debug.Log("Faster!");
        }
        if(Keyboard.current.sKey.wasReleasedThisFrame)
        {
            activeTetraminoFallSpeed = TETRAMINO_FALL_SPEED;
            Debug.Log("Slower!");
        }

        if(TouchManager.thisFrameTouchInput == DetectedTouchInput.LONG_STILL_TOUCH)
        {
            // ? --- Si resetta in automatico ndell'update  
            activeTetraminoFallSpeed = TETRAMINO_FALL_SPEED_ON_PRESS;
        }

        // ? --- Rotazione
        if(Keyboard.current.qKey.wasPressedThisFrame
        || TouchManager.thisFrameTouchInput == DetectedTouchInput.SWIPE_UP)
        {
            RotateLeft();
        }
        if(Keyboard.current.eKey.wasPressedThisFrame
        || TouchManager.thisFrameTouchInput == DetectedTouchInput.SWIPE_DOWN)
        {
            RotateRight();
        }
    }

    void RotateLeft()
    {
        Quaternion originalRot = 
            activePiece.rotationPivot.transform.localRotation;

        activePiece.rotationPivot.transform.localRotation =
            Quaternion.Euler
            (
                activePiece.rotationPivot.transform.localEulerAngles.x,
                activePiece.rotationPivot.transform.localEulerAngles.y,
                activePiece.rotationPivot.transform.localEulerAngles.z 
                -90f
            );

        // ? --- La funzione verifica anche se esce fuori dalla mappa
        bool hasCollided = 
                CheckTetraminoCollisionOnGrid(activePiece, 0, 0).generalCollision;
        
        if(hasCollided)
        {
            activePiece.rotationPivot.transform.localRotation = 
                originalRot;
        }
        else
        {
            onPieceRotate.Invoke();
        }
    }

    void RotateRight()
    {
        Quaternion originalRot = 
            activePiece.rotationPivot.transform.localRotation;

        activePiece.rotationPivot.transform.localRotation =
            Quaternion.Euler
            (
                activePiece.rotationPivot.transform.localEulerAngles.x,
                activePiece.rotationPivot.transform.localEulerAngles.y,
                activePiece.rotationPivot.transform.localEulerAngles.z 
                +90f
            );

        // ? --- La funzione verifica anche se esce fuori dalla mappa
        bool hasCollided = 
            CheckTetraminoCollisionOnGrid(activePiece, 0, 0).generalCollision;
        
        if(hasCollided)
        {
            activePiece.rotationPivot.transform.localRotation = 
                originalRot;
        }
        else
        {
            onPieceRotate.Invoke();
        }
    }

    /// <summary>
    /// 
    /// Istanzia nuovo pezzo e setta come _activePiece
    /// 
    /// </summary>
    [SerializeField]
    List<GameObject> tetraminosTypes;
    
    // ? --- Inizializzato da CheckaAndCreateNewPouch()
    [SerializeField, EditorAttributes.ReadOnly]
    List<GameObject> piecesPouch;
    int pouchIndex = 0;
    //GameObject[] activeTetraminoPouch;
    void CreateNewPiece()
    {
        CheckAndCreateNewPouch();

        // int randNum = UnityEngine.Random.Range(0, tetraminosTypes.Count);
        // activePiece =
        //     Instantiate(tetraminosTypes[randNum])
        //     .GetComponent<Tetramino>();
        activePiece = Instantiate(piecesPouch[pouchIndex])
            .GetComponent<Tetramino>();

        activePiece.transform.SetParent(tetraminosPiecesWrapper.transform);
        activePiece.transform.position = 
            GridSquareBottomLeftToWorldCoords(0, 0);
        
        // ? --- Se abbiamo colliso con un tetramino
        // ? --- appena siamo spawnati significa che abbiamo perso

        var colls = CheckTetraminoCollisionOnGrid(activePiece, 0, 0);

        Debug.Log(colls); 
        if(colls.onTetraminoCollision)
        {
            GameOverLogic();
            return;
        }

        onPieceCreated.Invoke(activePiece);
        Debug.Log("Piece created!");
        pouchIndex++;
    }

    void CheckAndCreateNewPouch()
    {
        // ? --- La pouch avra' una grandezza di 8 pezzi, in modo tale
        // ? --- che appena arriviamo al 7imo pezzo abbiamo l'8 come reference
        // ? --- per creare una nuova pouch e non rimaniamo senza reference al prossimo
        // ? --- pezzo nel caso in cui siamo al 7 index.
        if(piecesPouch == null ||
            piecesPouch.Count == 0)
        {
            var randomizedTetraminos = 
                tetraminosTypes.OrderBy(x => UnityEngine.Random.value)
                .ToList();
            piecesPouch = randomizedTetraminos;

            Debug.Log("List randTetrs: " + randomizedTetraminos);
            int randomNum = UnityEngine.Random.Range(0,tetraminosTypes.Count);
            Debug.Log("RandomNum: " + randomNum);
            // ? --- 8 Pezzo
            piecesPouch.Add(randomizedTetraminos[randomNum]);

            pouchIndex = 0;

            Debug.Log("Pouch created, size: " + piecesPouch.Count);
        }
        else
        {
            if(pouchIndex != 7)
                return;

            GameObject eightObjRef = piecesPouch[piecesPouch.Count-1];
    
            var randomizedTetraminos = 
                tetraminosTypes.OrderBy(x => UnityEngine.Random.value)
                .ToList();

            Debug.Log("Has removedPiece: " + 
            randomizedTetraminos.Remove(eightObjRef));
            

            piecesPouch.Clear();

            // ? --- Aggiungiamo l'8 come 1
            piecesPouch.Add(eightObjRef);

            // ? --- Aggiungiamo i restanti 6
            piecesPouch.AddRange(randomizedTetraminos);

            int randomNum = UnityEngine.Random.Range(0, tetraminosTypes.Count);

            piecesPouch.Add(tetraminosTypes[randomNum]);

            pouchIndex = 0;


            Debug.Log("Pouch REcreated, size: " + piecesPouch.Count);
        }
        
    }

    // ? --- Utile per la UI 
    public TetraminoTypes GetActiveTetraminoType()
    {

        return activePiece.type;
    }

    // ? --- Utile per la UI 
    public TetraminoTypes GetNextPouchTetraminoType()
    {
        int nextIndex = pouchIndex + 1;
        if(nextIndex >= piecesPouch.Count ||
            nextIndex < 0)
        {
            Debug.LogError("Invalid index: " + nextIndex);
            return 0;
        }

        return piecesPouch[nextIndex].GetComponent<Tetramino>().type;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="traslX">Numero di quadrati di cui spostarsi nell'asse X</param>
    /// <param name="traslY">Stessa cosa ma nell'asse Y</param>
    void MoveActivePiece(int squareTraslX, int squareTraslY)
    {
        if (squareTraslX > 1 || squareTraslX < -1 || squareTraslY > 1 || squareTraslY < -1 )
        {
            UtilityShit.Log("Traslazione troppo alta, errori probabili", Color.red);
        }



        int newGridPosX = activePiece.gridPosX + squareTraslX;
        int newGridPosY = activePiece.gridPosY + squareTraslY;
        bool hasCollided = false;


        /* ------------- Check collisioni ------------- */
        //* -- Trasla solo se non ci sono collisioni
        // ? --- Devo dividere gli le funzioni in due, una per ogni asse diverso
        // ? --- in modo da identificare esattamente quale asse crea problemi ed evitare il 
        // ? --- movimento solo con quella, cosa che non ptorei fare 
        // ? --- facilmente se fossero unite le chiamate
        //* -- Asse x
        if (!CheckTetraminoCollisionOnGrid(activePiece, squareTraslX, 0).generalCollision)
        {
            activePiece.gridPosX = newGridPosX;
        }
        //* -- Collisione avvenuta 
        else
        {
            UtilityShit.Log("Collided!", Color.red);
            hasCollided = true;
        }

        //* -- Asse y
        if (!CheckTetraminoCollisionOnGrid(activePiece, 0, squareTraslY).generalCollision)
        {
            activePiece.gridPosY = newGridPosY;
        }

        //* -- Collisione avvenuta 
        else
        {
            UtilityShit.Log("Collided!", Color.red);
            hasCollided = true;
        }



        activePiece.transform.position =
            GridSquareBottomLeftToWorldCoords
            (
                activePiece.gridPosX,
                activePiece.gridPosY
            );

        /*
        _activePiece.rectTrans.anchoredPosition =
        new Vector2(
            _activePiece.gridPosX * _gridSquareSize,
            _activePiece.gridPosY * _gridSquareSize
        );
        
        */
        onPieceMove.Invoke(squareTraslY, squareTraslX, hasCollided);

    }

    /// <summary>
    /// Per calcolare le collisioni occorre prevedere il movimento.
    /// 
    /// In questo caso non faremo altro che scomporre gli assi del movimento per
    /// comprendere quale movimento ha causato la collisione.
    /// </summary>
    /// <param name="tetr"></param>
    /// <param name="traslX"></param>
    /// <param name="traslY"></param>
    /// <returns> Vero se
    (bool generalCollision, bool onTetraminoCollision) 
    CheckTetraminoCollisionOnGrid(Tetramino tetr, int traslX, int traslY)
    {
        bool genColl = false;
        bool onTetrCollision = false; 

        if (traslX > 1 || traslX < -1 || traslY > 1 || traslY < -1)
        {
            UtilityShit.Log("Traslazione troppo alta, errori probabili", Color.red);
        }

        /* --------- Prevedi traslazione pezzo -------- */
        //* -- Serve per vedere se il movimento puo' essere effettuato
        foreach (GameObject gO in tetr.blocks)
        {
            /*
            * TODO: orgin negativa crea problemi
            */

            //* -- Indici blocco tetramino sulla griglia
            int blockX = WorldCoordsToGridX(gO.transform.position.x);
            int blockY = WorldCoordsToGridY(gO.transform.position.y);
            //Debug.Log(gO.gameObject.transform.position);
            //Debug.Log("BlockX: " + blockX + " " + " BlockY: " + blockY);
            int gridX = blockX + traslX;

            /*
            WorldCoordsToGridX
            (
                traslatedWorldPosition.x +
                gO.transform.localPosition.x -
                _gridOrigin.transform.position.x
            );*/

            int gridY = blockY + traslY;

            /*
            WorldCoordsToGridY
            (
                traslatedWorldPosition.y +
                gO.transform.localPosition.y -
                _gridOrigin.transform.position.y
            );*/


            /*
            Debug.Log\
                (
                    "GridY: " + gridY + " GridX: " + gridX +

                    "\nTrasl: " + traslX + " " + traslY +

                    "\nBlock name: " +
                    gO.name +

                    "\nBlock local pos:" +
                    gO.transform.position.y +
                    " " +
                    gO.transform.position.x +

                    "\nGrid Origin: " + _gridOrigin.transform.position
                );
                */


            /* --------------------- - -------------------- */
            /*   Se va out of bounds conta come collisione  */
            if (gridX < 0 || gridX >= gridWidthInSquares)
            {
                UtilityShit.Log("Out of bounds collision!", Color.yellow);
                return (true, false);
            }

            if (/*gridY < 0 Previene movimento spawn  ||*/
                    gridY > gridHeightInSquares - 1)
            {
                UtilityShit.Log("Out of bounds collision!", Color.yellow);
                return (true, false);
            }

            /*
            * TODO: Valuta se crea bug
            */
            if (gridY < 0)
                gridY = 0;

            if (gridMap[gridY, gridX] == GridMapValues.TETRAMINO)
            {
                UtilityShit.Log("Collision with Tetramino", Color.yellow);
                genColl = true;
                onTetrCollision = true;
            }
        }

        return (genColl, onTetrCollision);
    }

    /// <summary>
    /// 
    /// Piazza il tetramino ammesso che non ci siano errori di sorta
    /// (Compenetrazioni con MURI o altri TETRAMINI)
    /// 
    /// </summary>
    void PlaceTetraminoOnGrid(Tetramino tetr)
    {
        foreach (GameObject blockGO in tetr.blocks)
        {
            int squareGridY = WorldCoordsToGridY(blockGO.transform.position.y);
            int squareGridX = WorldCoordsToGridX(blockGO.transform.position.x);

            GridMapValues collisionValueAt =
                GetGridMapValueAt
                (
                    squareGridY,
                    squareGridX
                );

            Debug.Log
            (
                "SquareGrid: " + squareGridY + " " + squareGridX +
                "\nCollision Value: " + collisionValueAt
            );

            /* -------------- Error handling -------------- */
            switch(collisionValueAt)
            {



                /* ---- Punto griglia occupato da TETRAMINO --- */
                case GridMapValues.TETRAMINO:
                    //     Debug.LogError
                    // (
                    //     "Trying to put tetramino on TETRAMINO" +
                    //     "\nIndex values: " + squareGridY + " " + squareGridX
                    // );
                    break;



                /* ------ Punto griglia occupato da MURO ------ */
                case GridMapValues.WALL:

                    // Debug.LogError
                    //     (
                    //         "Trying to put tetramino on WALL" +
                    //         "\nIndex values: " + squareGridY + " " + squareGridX
                    //     );

                    break;



                /* ----------- Punto griglia valido ----------- */
                case GridMapValues.EMPTY:

                    UtilityShit.Log("Placed!", Color.grey);
                    Debug.Log(gridMap.Length);
                    
                    Debug.Log
                    (
                        "square: " + squareGridY + " " + squareGridX
                    );
                    
                    gridMap[squareGridY, squareGridX] = GridMapValues.TETRAMINO;
                    blocksGridMapRef[squareGridY, squareGridX] = blockGO;
                    Debug.Log("Placed: " + blockGO.name );
                    /* --------------- Setta griglia -------------- */
                    /*foreach (GameObject toPlaceBlockGO in tetr)
                    {
                        //* -- Non serve effettuare check,
                        //* -- se siamo qui significa che siamo
                        //* -- in punti validi della griglia
                        int toPlaceBlockGOGridX =
                            WorldCoordsToGridX(toPlaceBlockGO.transform.position.x);
                        int toPlaceBlockGOGridY =
                            WorldCoordsToGridY(toPlaceBlockGO.transform.position.y);

                        gridMap[toPlaceBlockGOGridX, toPlaceBlockGOGridY] 
                            = GridMapValues.TETRAMINO;
                    }*/
                    break;                
            }
        }
    }

    /// <summary>
    /// 
    /// Controlla se le GAMEPLAY CONDITIONS richiedono di posizionare
    /// il tetramino attivo
    /// 
    /// Ogni _fallTetraminoSquaresPerSecond viene verificato se il tetramino
    /// soddisfa le condizioni di "posizionamento".
    /// 
    /// </summary>
    bool VerifyAndExecuteTetraminoPlaceConditions(Tetramino tetr)
    {
        bool hasBeenPlaced = false;
        

        foreach (GameObject blockGO in tetr.blocks)
        {
            //* -- Posizione del blocco nella griglia
            int blockGridY = WorldCoordsToGridY(blockGO.transform.position.y);
            int blockGridX = WorldCoordsToGridX(blockGO.transform.position.x);

            /* --------- Ultima riga della griglia -------- */
            if (blockGridY > gridHeightInSquares - 2/*Ricorda che sono indici*/)
            {
                //* -- Piazziamo pezzo
                PlaceTetraminoOnGrid(tetr);

                UtilityShit.Log
                (
                    "Piazzato: Hai raggiunto la fine della griglia!",
                    Color.green
                );

                hasBeenPlaced = true;
                break;
            }

            /* ------- Collisione con altro tetramino ------ */
            if (
                    GetGridMapValueAt
                    (
                        blockGridY + 1,/*non si compenetra, quindi ovviamnete si va di predict*/
                        blockGridX
                    )
                    == GridMapValues.TETRAMINO
                )
            {
                PlaceTetraminoOnGrid(tetr);

                UtilityShit.Log
                (
                    "Piazzato: Hai toccato un altro blocchetto!",
                    Color.green
                );

                hasBeenPlaced = true;
            }
        }


        //* -- Se abbiamo colliso, il pezzo attivo,
        //* -- NON deve essere piu' usato
        //* TODO: Registra i pezzi piazzati
        List<int> foundIndicesToDel = new();
        bool fullLineFound = false;
        if(hasBeenPlaced)
        {
            //* -- Se abbiamo piazzato il pezzo dobbiamo
            //* -- controllare se la linea e' piena
            int preaviousBlockGridY = -999;

            int performance_GridChecks = 0;

            List<int> linesToCheck = new();
            //* -- Dobbiamo controllare se ogni blocco posizionato
            //* -- ha fatto in modo di riempire la griglia
            foreach(GameObject blockGO in tetr.blocks)
            {
                //* -- Posizione del blocco nella griglia
                int blockGridY =
                    WorldCoordsToGridY(blockGO.transform.position.y);

                //* -- Con questo if evitiamo di controllare 
                //* -- la stessa riga piu' volte
                if (blockGridY == preaviousBlockGridY)
                {
                    continue;
                }
                if(!linesToCheck.Contains(blockGridY))
                {
                    linesToCheck.Add(blockGridY);    
                }
            }
            /// TODO UNA LUUUUUNGA SPIEGAZIONE
            Debug.Log("Lin: " + linesToCheck);
            int scoredLines = 0;
            foreach(int indexToCheck in linesToCheck)
            {
                if (CheckGridLineFill(indexToCheck))
                {
                    scoredLines++;
                    fullLineFound = true;
                    UtilityShit.Log("Found full line!", Color.cyan);
                    preaviousBlockGridY = indexToCheck;
                    foundIndicesToDel.Add(indexToCheck);
                    //DeleteLine(indexToCheck);
                    performance_GridChecks++; 
                    Debug.Log("line found!!!!");  
                }
            }

            
            List<(GameObject blocksRef, int xIndex, int yIndex)>
                blocksRef = new();
            if(foundIndicesToDel.Count > 0)
            {
                
                foreach(int index in foundIndicesToDel)
                {
                    blocksRef
                        .AddRange(DeleteLineFromCollisionMap(index));    
                }
            }


            // ? --- Togliamo il controllo al player, dopodiche'
            // ? --- firiamo l'evento di lineClear PRIMA di eliminare
            // ? --- le reference che coinvolgono la linea.
            // ? --- In questo modo possiamo animare e fare il cazzo
            // ? --- che vogliamo con i blocchi che verranno eliminati.
            if(foundIndicesToDel.Count > 0)
            {
                activePiece = null;
                onLineClear.Invoke
                (
                    blocksRef,
                    LINE_CLEAR_ANIMATION_TIME,
                    foundIndicesToDel.Count
                );

                PowUtilityU.Get().DelayAction
                (
                    () =>
                    {
                        DeleteLinesFromReferenceMap(blocksRef);

                        TranslateDownMultiple(foundIndicesToDel);
                        CreateNewPiece();
                    },
                    LINE_CLEAR_ANIMATION_TIME
                );                
            }


            // ? --- Se non ci sono linee da eliminare, 
            // ? --- semplicemente crea un nuovo pezzo
            if(foundIndicesToDel.Count == 0)
            {
                CreateNewPiece();
                onPieceJustPlaced.Invoke();
            }

            // if(fullLineFound)
            // {
            //     onPieceJustPlacedAndScored
            //         .Invoke(scoredLines);
            // }
            // else
            // {
            //     onPieceJustPlaced.Invoke();
            // }
            Debug.Log("Performed grid fill checks: " + performance_GridChecks);
        }




        return hasBeenPlaced;
    }

    GridMapValues GetGridMapValueAt(int gridSquareY, int gridSquareX)
    {
        /* -------------- Error handling -------------- */
        if (gridSquareX < 0)
        {
            UtilityShit.Log("Negative square X index!", Color.red);
            return GridMapValues.EMPTY;
        }
        if (gridSquareY < 0)
        {
            UtilityShit.Log("Negative square X index!", Color.red);
            return GridMapValues.INVALID;
        }
        if (gridSquareX >= gridWidthInSquares)
        {
            UtilityShit.Log("Out of bounds square X index!", Color.red);
            return GridMapValues.INVALID;
        }
        if (gridSquareY >= gridHeightInSquares)
        {
            UtilityShit.Log("Out of bounds square Y index!", Color.red);
            return GridMapValues.INVALID;
        }

        return gridMap[gridSquareY, gridSquareX];
    }

    GameObject GetGridBlockRefAt(int gridSquareY, int gridSquareX)
    {
        if (gridSquareX < 0)
        {
            UtilityShit.Log("Negative square X index!", Color.red);
            return null;
        }
        if (gridSquareY < 0)
        {
            UtilityShit.Log("Negative square X index!", Color.red);
            return null;
        }
        if (gridSquareX >= gridWidthInSquares)
        {
            UtilityShit.Log("Out of bounds square X index!", Color.red);
            return null;
        }
        if (gridSquareY >= gridHeightInSquares)
        {
            UtilityShit.Log("Out of bounds square Y index!", Color.red);
            return null;
        }

        return blocksGridMapRef[gridSquareY, gridSquareX];
    }

    #endregion
    #region Gameplay Events
    [FoldoutGroup("Gameplay Events", nameof(onPieceMove), nameof(onPieceRotate), 
    nameof(onPieceJustPlaced), nameof(onPieceJustPlacedAndScored),
    nameof(LINE_CLEAR_ANIMATION_TIME), nameof(onLineClear),
    nameof(onPieceCreated))]
    [SerializeField] private EditorAttributes.Void eventsHolder;

    [field: SerializeField, HideProperty]
    
    // ? --- Passano il vettore di movimento, utile se si vuole triggherare un evento
    // ? --- solo se ci si muove verso una data direzione.
    // ? --- INT: yTranslation
    // ? --- INT: xTranslation
    // ? --- BOOL: hasCollided: Utile per il suono, cosi' non spamma se teniamo premuto e andiamo contro un muro
    public UnityEvent<int, int, bool> onPieceMove { get; private set; } = new();
    [field: SerializeField, HideProperty]
    public UnityEvent onPieceRotate { get; private set; } = new(); 
    [field: SerializeField, HideProperty]
    public UnityEvent onPieceJustPlaced { get; private set; } = new();
    
    // ? --- Se il pezzo e' stato appena posizionato
    // ? --- e ha tolto una linea
    // ? --- INT: quante righe hanno fatto score
    [field: SerializeField, HideProperty]
    public UnityEvent<int> onPieceJustPlacedAndScored { get; private set; } = new();

    [field: SerializeField, HideProperty] 
    public float LINE_CLEAR_ANIMATION_TIME { get; private set; }
    // ? --- GameObject[]: Blocchi involved
    // ? --- float: tempo MASSIMO animazione (blocca il gioco)
    [field: SerializeField, HideProperty]

    // ? --- Ha come riferimento tuple con riferimento al blocco che si sta
    // ? --- distruggendo e gli X e Y index di riferimento dentro blocksGridMapRef.
    // ? --- float: durata massima concessa animazione
    // ? --- INT: numero di linee distrutto
    public UnityEvent<List<(GameObject blocks, int xIndex, int yIndex)>, float, int>
        onLineClear { get; private set;} = new();

    // ? --- TETRAMINO: Reference al pezzo creato
    [field: SerializeField, HideProperty]
    public UnityEvent<Tetramino> onPieceCreated { get; private set; } = new(); 
    #endregion

    #region Match Events
    [FoldoutGroup("Match Events", 
    nameof(onMatchStart), nameof(onMatchGameOver))]
    [SerializeField] private EditorAttributes.Void matchEventsHolder;

    [field: SerializeField, HideProperty]
    public UnityEvent onMatchStart { get; private set; } = new();
    
    [field: SerializeField, HideProperty]
    public UnityEvent onMatchGameOver { get; private set; } = new();

    #endregion


    bool hasGameOver = false;
    void GameOverLogic()
    {
        hasGameOver = true;
        Debug.LogWarning("You have been slain!");
        onMatchGameOver.Invoke();
    }

    public void RestartMatch()
    {
        // ? --- XDDDD
        MatchBegin();
    }
    void MatchBegin()
    {
        hasGameOver = false;

        activeTetraminoFallSpeed = TETRAMINO_FALL_SPEED;

        blocksGridMapRef =
            new GameObject[gridHeightInSquares, gridHeightInSquares];
        gridMap =
            new GridMapValues[gridHeightInSquares, gridWidthInSquares]; 

        InitGridMapAndBlockGridMap();

        piecesPouch.Clear();
        pouchIndex = 0;


        if(tetraminosPiecesWrapper.transform.childCount != 0)
        {
            foreach(Transform trans in tetraminosPiecesWrapper.transform)
            {
                Destroy(trans.gameObject);
            }
        }

        CreateNewPiece();


        onMatchStart.Invoke();
    }

    void LogicUpdate()
    {
        
        //* -- Resetta assi dello scorso input
        inputTraslX = 0; inputTraslY = 0; 
        activeTetraminoFallSpeed = TETRAMINO_FALL_SPEED;

        if(activePiece == null ||
            Time.timeScale == 0f ) // ? --- XDDDDD
            return; 

 
        RetrieveInput();

        if (inputDelayTimer >= INPUT_DELAY)
        {

            if(inputTraslX != 0)
            {
                MoveActivePiece(inputTraslX, 0);
                //UtilityShit.Log("Moving by input!", Color.blue);
            }

            inputDelayTimer = 0f;
        }



        if (tetraminoFallTimer >= activeTetraminoFallSpeed)
        {
            //UtilityShit.Log("Moving Tetramino!", Color.cyan);

            //* -- Se posizioniamo il blocco, dopo, non dobbiamo muovere
            if (!VerifyAndExecuteTetraminoPlaceConditions(activePiece))
            {
                MoveActivePiece(0, 1);
            }
            tetraminoFallTimer = 0;
        }

        inputDelayTimer += Time.deltaTime;
        tetraminoFallTimer += Time.deltaTime;
    }





    void Awake()
    {
        activeTetraminoFallSpeed = TETRAMINO_FALL_SPEED;
        //_gridSquareSize = _gridImage.mainTexture.width / _gridWidthInSquares;
        blocksGridMapRef =
            new GameObject[gridHeightInSquares, gridHeightInSquares];
        gridMap =
            new GridMapValues[gridHeightInSquares, gridWidthInSquares];
        InitGridMapAndBlockGridMap();

        CreateNewPiece();
    }

    GameObject debPiv;
    void Start()
    {
        debPiv = GameObject.Find("DebugPivot");
        onMatchStart.Invoke();
    }


    void Update()
    {
        if(hasGameOver)
            return;
        //WorldCoordsToGridY(debPiv.transform.position.y);
        LogicUpdate();
    }
}