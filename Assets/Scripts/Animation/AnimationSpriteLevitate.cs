using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationSpriteLevitate : MonoBehaviour
{
    [SerializeField]
    float offsetY;
    
    [SerializeField]
    float offsetX;

    Vector3 startPos;
    void Start()
    {
        startPos = transform.position;
        transform.DOMoveY(startPos.y + offsetY, 2f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
        transform.DOMoveX(startPos.x + offsetX, 2f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    void OnDestroy()
    {
        transform.DOKill();
    }

    void Update()
    {

    }
}
