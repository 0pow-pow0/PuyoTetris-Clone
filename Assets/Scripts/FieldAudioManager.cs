using System.Collections.Generic;
using DG.Tweening;
using EditorAttributes;
using UnityEngine;

public class FieldAudioManager : MonoBehaviour
{
    [field: SerializeField]
    public AudioSource SFXSource { get; private set; } 
    
    [field: SerializeField]
    public AudioSource musicSource { get; private set; }


    #region Tetramino Related Sounds
    [field: FoldoutGroup("Tetramino Related Sounds", 
    nameof(movePieceSound), nameof(rotatePieceSound), nameof(placePieceSound)),
    SerializeField]
    EditorAttributes.Void tetraminoGroup;
    
    [field: HideProperty, SerializeField]
    public AudioClip movePieceSound { get; private set; }

    [field: SerializeField, HideProperty]
    public AudioClip rotatePieceSound { get; private set; }

    [field: SerializeField, HideProperty]
    public AudioClip placePieceSound { get; private set; } 



    public void PlayMovePieceSound(int yTranslation, int xTranslation, bool hasCollided)
    {
        // ? --- Non vogliamo che si triggheri il rumore per ogni movimento
        // ? --- automatico verso il basso.

        // ? --- TODO: purtroppo parte in spam se teniamo premuto il tasto verso il muro
        // ? --- e rimaniamo attaccati ad esso.
        if(xTranslation == 0 ||
            hasCollided)
            return;

        SFXSource.PlayOneShot(movePieceSound);
    }

    public void PlayRotatePieceSound()
    {
        SFXSource.PlayOneShot(rotatePieceSound);
    }

    public void PlayPlacePieceSound()
    {
        SFXSource.PlayOneShot(placePieceSound);
    }


    #endregion    
    
    #region Lines Related Sounds
    [FoldoutGroup("Lines Related Sounds",
    nameof(line1ClearAudio)), SerializeField]
    AudioClip line1ClearAudio;

    public void PlayLineScoreSound(
        List<(GameObject blocks, int xIndex, int yIndex)> blocksRef, 
        float animationMaxTime,
        int linesDeleted)
    {
        switch(linesDeleted)
        {
            case 1:
            SFXSource.PlayOneShot(line1ClearAudio);
            break;

            default:
            SFXSource.PlayOneShot(line1ClearAudio);
            break;
            
        }
    }

    #endregion

    #region Music Related
    [FoldoutGroup("Music Related", 
    nameof(mainBackgroundMusic))]

    [SerializeField]
    AudioClip mainBackgroundMusic;

    public void PlayMainBackgroundMusic()
    {
        musicSource.loop = true;

        musicSource.clip = mainBackgroundMusic;

        musicSource.volume = 0f;
        musicSource.Play();

        musicSource.DOFade(1f, 10f);
    }

    public void StopBackgroundMusic()
    {
        
    }

    #endregion
}
