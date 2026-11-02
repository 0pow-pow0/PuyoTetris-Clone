using System.Net.Sockets;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class AnimationFadeAndScale : MonoBehaviour
{

    [SerializeField]
    float blinkingSpeed;
    [SerializeField]
    float scaleSpeed;
    [SerializeField]
    float scaleFactor;

    Vector3 startPos;
    [SerializeField]
    TextMeshProUGUI txt;
    void Start()
    {
        transform.DOScale(scaleFactor, scaleSpeed) 
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
        
        txt.DOFade(0, blinkingSpeed)
        .OnComplete(() => txt.DOFade(1f, blinkingSpeed))
        .SetLoops(-1, LoopType.Yoyo)
        .SetEase(Ease.InOutSine);

            
    }

    void OnDestroy()
    {
        txt.DOKill();
        transform.DOKill();
    }

    void Update()
    {

    }
}
