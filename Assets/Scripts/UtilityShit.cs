using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Serve per evitare di andare a cercare in tutto 
/// l'albero un gameobject con un tag determinato.
/// </summary>
/// <param name="parent">Padre da cui si parte per ricercare.</param>
/// <param name="nameToFind">Tag da ricercare.</param>
/// <returns></returns>
/// 
class UtilityShit : MonoBehaviour
{
    private static UtilityShit inst;

    public static UtilityShit Get()
    {
        if(inst == null)
        {
            Debug.LogError("UtilityShit non instanziato!");    
            return null;
        }

        return inst;
    }

    void InitSingleton()
    {
        if(inst != null)
        {
            Debug.LogError("UtilityShit gia' instanziato");
        }
        else
        {
            inst = this;
        }
    }

    void Awake()
    {
        InitSingleton();
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="min">Exclusive</param>
    /// <param name="max">Inclusive</param>
    /// <returns></returns>
    bool IsInRange(int num, int min, int max)
    {
        bool isIt = false;
        if (num < min)
        {
            isIt = true;
        }
        else if (num >= max)
        {
            isIt = true;
        }

        return isIt; 
    }

    /// <summary>
    /// Non ricorsivo, si ferma al livello piu' alto di figli
    /// </summary>
    /// <returns></returns>
    public static GameObject[] GetChildrenAsGameObjects(GameObject parent)
    {
        GameObject[] children = new GameObject[parent.transform.childCount];
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            children[i] = parent.transform.GetChild(i).gameObject;
        }

        return children;
    }

    public static GameObject[] FindChildrenWithName(GameObject parent, string nameToFind)
    {
        List<GameObject> foundChildren = new List<GameObject>();
        foreach (Transform trf in parent.transform)
        {
            if (trf.gameObject.name == nameToFind)
            {
                foundChildren.Add(trf.gameObject);
            }
        }

        return foundChildren.ToArray();
    }

    public static GameObject FindChildWithName(GameObject parent, string nameToFind, bool reiterated = false)
    {
        foreach (Transform trf in parent.transform)
        {
            if (trf.childCount > 0)
            {
                GameObject obj = FindChildWithName(trf.gameObject, nameToFind, true);
                if (obj != null)
                {
                    return obj;
                }
            }

            if (trf.gameObject.name == nameToFind)
            {
                return trf.gameObject;
            }
        }
        if (!reiterated)
            Debug.LogWarning("Gameobject with name: " + nameToFind + " not found!");
        return null;
    }

    public static GameObject FindChildWithTag(GameObject parent, string tagToFind, bool reiterated = false)
    {
        foreach (Transform trf in parent.transform)
        {
            if (trf.childCount > 0)
            {
                GameObject obj = FindChildWithName(trf.gameObject, tagToFind, true);
                if (obj != null)
                {
                    return obj;
                }
            }

            if (trf.gameObject.tag == tagToFind)
            {
                return trf.gameObject;
            }
        }
        if (!reiterated)
            Debug.LogWarning("Gameobject with tag: " + tagToFind + " not found!");
        return null;
    }

    ///<summary>
    /// Permette di colorare il testo O.O :O
    ///</summary>
    public static void Log(object message, Color clr)
    {
        if (clr == null)
        {
            Debug.Log(message);
        }
            Debug.Log("<color=" + "#" + clr.ToHexString() + ">" + message + "</color>");
    }

    public static string AddColor(string original, Color toAddColor)
    {
        string result = "<color=#" + toAddColor.ToHexString() + ">" + original + "</color>";
        return result;
    }
}
