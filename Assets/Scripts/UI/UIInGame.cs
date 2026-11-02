using EditorAttributes;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;
using DG.Tweening;
using Unity.VisualScripting;

public class UIInGame : MonoBehaviour
{
    [Header("References"), SerializeField]
    TetrisLogic tl;

    #region Next Icon

    [SerializeField]
    Image nextTetraminoIcon;

    [FoldoutGroup("Tetramino Icons", 
    nameof(ZIcon), nameof(ZMirrorIcon), nameof(LIcon), nameof(LMirrorIcon),
    nameof(SquareIcon), nameof(TIcon), nameof(IIcon)), SerializeField]
    Sprite ZIcon;
    [SerializeField, HideProperty]
    Sprite ZMirrorIcon;  
    [SerializeField, HideProperty]
    Sprite LIcon;
    [SerializeField, HideProperty]
    Sprite LMirrorIcon;
    [SerializeField, HideProperty]
    Sprite SquareIcon;
    [SerializeField, HideProperty]
    Sprite TIcon;
    [SerializeField, HideProperty]
    Sprite IIcon;

    public void UpdateNextPieceIconBasedOnType(Tetramino activeTr)
    {
        TetraminoTypes trType = tl.GetNextPouchTetraminoType();

        switch(trType)
        {
            case TetraminoTypes.I:
            nextTetraminoIcon.sprite = IIcon;
            break;

            case TetraminoTypes.SQUARE:
            nextTetraminoIcon.sprite = SquareIcon;
            break;

            case TetraminoTypes.T:
            nextTetraminoIcon.sprite = TIcon;
            break;

            case TetraminoTypes.L:
            nextTetraminoIcon.sprite = LIcon;
            break;

            case TetraminoTypes.L_MIRRORED:
            nextTetraminoIcon.sprite = LMirrorIcon;
            break;

            case TetraminoTypes.Z:
            nextTetraminoIcon.sprite = ZIcon;
            break;

            case TetraminoTypes.Z_MIRRORED:
            nextTetraminoIcon.sprite = ZMirrorIcon;
            break;
        }
    }
    #endregion


    // ? --- Nessun motivo per il quale sia qui, solo perche' sono disordinato e vado di fretta
    #region Scoreboard
    [FoldoutGroup("Scoreboard", 
    nameof(scoreboardText), nameof(scoredPoints),
    nameof(scored1ConsecutiveLineValue), nameof(scored2ConsecutiveLineValue),
    nameof(scored3ConsecutiveLineValue), nameof(scored4ConsecutiveLineValue)),
    SerializeField]
    EditorAttributes.Void socreboardHolder;

    [SerializeField, HideProperty]
    TextMeshProUGUI scoreboardText;
    [SerializeField, HideProperty, ReadOnly]
    int scoredPoints;

    [SerializeField, HideProperty]
    int scored1ConsecutiveLineValue;
    [SerializeField, HideProperty]
    int scored2ConsecutiveLineValue;

    [SerializeField, HideProperty]
    int scored3ConsecutiveLineValue;

    [SerializeField, HideProperty]
    int scored4ConsecutiveLineValue;

    public void ResetScoreboard()
    {
        scoredPoints = 0;
        scoreboardText.text = 0.ToString("D8");
    }

    public void UpdateScoreboard(
        List<(GameObject block, int xIndex, int yIndex)> blocksRefs, 
        float animationMaxTime,
        int linesToDel)
    {

        switch(linesToDel)
        {
            case 1:
            AddToScore(scored1ConsecutiveLineValue, animationMaxTime);
            break;

            case 2:
            AddToScore(scored2ConsecutiveLineValue, animationMaxTime);
            break;
            
            case 3:
            AddToScore(scored3ConsecutiveLineValue, animationMaxTime);
            break;

            case 4:
            AddToScore(scored4ConsecutiveLineValue, animationMaxTime);
            break;

            default:
            AddToScore(scored4ConsecutiveLineValue, animationMaxTime);
            break;
        }
    }

    void AddToScore(int toAdd, float tweeningTime)
    {
        int finalScoreValue = scoredPoints + toAdd;
        if(finalScoreValue > 99999999)
        {
            finalScoreValue = 99999999;
        }

        DOTween.To
        (
            () => scoredPoints,
            (int a) =>
            {
                scoredPoints = a;

                scoreboardText.text = scoredPoints.ToString("D8");
            }, 
            finalScoreValue,
            tweeningTime
        );
    }

    #endregion
}

