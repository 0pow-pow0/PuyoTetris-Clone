using System.Collections.Generic;
using System.ComponentModel;
using Unity.Burst;
using Unity.VisualScripting;
using UnityEngine;

public class FileManager : MonoBehaviour
{
    /* -------------------------------------------- */
    /*                Singleton Shit                */
    /* -------------------------------------------- */
    private static FileManager inst;

    public static FileManager Get()
    {
        if (inst == null)
        {
            Debug.LogError("File Manager non instanziato!");
            return null;
        }
        return inst;
    }

    /* -------------------------------------------- */
    /*              Container Variabili             */
    /* -------------------------------------------- */

    private Dictionary<TetraminoTypes, Tetramino> tetraminos;

    public Tetramino GetTetramino(TetraminoTypes tetraminoType)
    {
        if (tetraminos.ContainsKey(tetraminoType))
        {
            return tetraminos[tetraminoType].GetComponent<Tetramino>();
        }
        Debug.LogError("Tetramino asset not found!" + " Key: " + tetraminoType);
        return null;
    }

    void Awake()
    {
        /* ------------- Setting singleton ------------ */
        if(inst != null)
        {
            Debug.LogError("No vabbe' mbare instanzia due File Manager");
        }

        inst = GetComponent<FileManager>();


        /* ---------------- Tetraminos ---------------- */
        tetraminos = new Dictionary<TetraminoTypes, Tetramino>();

        tetraminos.Add
        (
            TetraminoTypes.L,
            Resources.Load<GameObject>
                ("Prefabs/Entities/Tetraminos/TetraminoL").GetComponent<Tetramino>()
        );

        tetraminos.Add
        (
            TetraminoTypes.I,

            Resources.Load<GameObject>
                ("Prefabs/Entities/Tetraminos/TetraminoI").GetComponent<Tetramino>()
        );
    }

}
