using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class FieldAnimationManager : MonoBehaviour
{
    [Header("References"), SerializeField]
    TetrisLogic tl;
 
    public void AnimateDestroyingBlocks(
        List<(GameObject block, int xIndex, int yIndex)> blocksRefs, 
        float animationMaxTime,
        int linesToDel)
    {
        float columnConcessedTime = animationMaxTime / tl.gridWidthInSquares; 
        int iteratedColumns = 0;
        
        blocksRefs.Sort((a, b) => a.xIndex.CompareTo(b.xIndex));
        int startXindex = blocksRefs[0].xIndex;
        foreach(var bl in blocksRefs)
        {
            SpriteRenderer spr = bl.block.GetComponent<SpriteRenderer>();
            bl.block.transform.DOScale(2f, columnConcessedTime * iteratedColumns );
            spr.DOFade(0f, animationMaxTime);
            if(bl.xIndex != startXindex)
            {
                startXindex = bl.xIndex;
                iteratedColumns++;
            }
        }
    }
}
