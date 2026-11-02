using DG.Tweening;
using TMPro;
using UnityEngine;

public class UIControls : MonoBehaviour
{
    [SerializeField]
    GameObject father;
    [SerializeField]
    float screenDuration;
    float passedTime;    

    bool hasTimerEnded = false;

    void Awake()
    {
        father.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        passedTime += Time.deltaTime;

        if(passedTime >= screenDuration
        && !hasTimerEnded)
        {
            DeactivatePanel();
        }
    }

    public void DeactivatePanel()
    {
        hasTimerEnded = true;
        father.transform.DOScale(0f, 0.5f)
        .OnComplete(
            () =>
            {
                father.transform.localScale = new Vector3(1f,1f,1f);
                father.SetActive(false);
            }
        );
    }
}
