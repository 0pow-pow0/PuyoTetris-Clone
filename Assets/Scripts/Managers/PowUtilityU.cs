using System;
using System.Collections;
using UnityEngine;

public class PowUtilityU : MonoBehaviour
{

    // -------------------------------------------
    // ! Coroutines Related 
    // -------------------------------------------
    public void DelayAction(
        Action action,
        float timeToWait)
    {
        StartCoroutine(
            DelayActionRoutine(action, timeToWait));
    }
    private IEnumerator DelayActionRoutine(
        Action action,
        float timeToWait
        )
    {
        yield return new WaitForSeconds(timeToWait);
        action.Invoke();
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="action">Il parametro conterra' 
    /// il tempo passato dall'inizio della coroutine  
    /// </param>
    /// <param name="exitCondition"></param>
    public void DoActionUntil(
        Action<float> action,
        Func<float, bool> exitCondition)
    {
        StartCoroutine(DoActionUntilRoutine(action, exitCondition));
    }
    private IEnumerator DoActionUntilRoutine
    (
        Action<float> action,
        Func<float, bool> exitCondition
    )
    {
        float passedTime = 0f;
        while(exitCondition.Invoke(passedTime) == true)
        {
            action.Invoke(passedTime);
            passedTime += Time.deltaTime;   
            yield return null;
        }
    }
    
    public void RepeatActionForFrame(
        Action action,
        int frameDuration
    )
    {
        StartCoroutine(RepeatActionForFrameRoutine(action, frameDuration));
    }



    private IEnumerator RepeatActionForFrameRoutine(
        Action action,
        int frameDuration)
    {
        int framesSinceStart = 0;
 
        while (framesSinceStart <= frameDuration)
        {
            action.Invoke();
            yield return null;
            framesSinceStart++;
        }
    }


    // -------------------------------------------
    // ! Color related
    // -------------------------------------------
    /// <summary>
    /// Ritorna l'equivalente del colore originale ma
    /// con l'alpha settato a 1
    /// </summary>
    public Color MaxFade(Color original)
    {
        return new Color(
            original.r,
            original.g,
            original.b,
            1f
        );
    }

    /// <summary>
    /// Ritorna l'equivalente del colore originale ma
    /// con l'alpha settato a 0
    /// </summary>
    public Color MinFade(Color original)
    {
        return new Color(
            original.r,
            original.g,
            original.b,
            0
        );
    }

    void Awake()
    {
        InitSingleton();
    }

    void Start()
    {

    }

    // -------------------------------------------
    // ! Singleton shit
    // -------------------------------------------
    private static PowUtilityU inst;

    public static PowUtilityU Get()
    {
        if(inst == null)
        {
            Debug.LogError("PowUtilityU non instanziato!");    
            return null;
        }

        return inst;
    }

    void InitSingleton()
    {
        if(inst != null)
        {
            Debug.LogError("PowUtilityU gia' instanziato");
        }
        else
        {
            inst = this;
        }
    }
}
