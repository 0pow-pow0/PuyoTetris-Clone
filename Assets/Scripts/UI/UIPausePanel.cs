using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIPausePanel : MonoBehaviour
{
    [Header("References"), SerializeField]
    GameObject father;
    [SerializeField]
    AudioMixer mixer;

    

    [SerializeField]
    Button pauseButton;

    void Awake()
    {
        father.SetActive(false);
    }

    public void OnPauseButtonPressed()
    {
        father.SetActive(true);
        pauseButton.interactable = false;
        Time.timeScale = 0f;
    }

    public void OnContinueButtonPressed()
    {
        father.SetActive(false);
        pauseButton.interactable = true;
        Time.timeScale = 1f;    
    }

    public void OnMusicSliderValueChanged(float sliderVal)
    {
        float resultVol = 30f * sliderVal - 30f;
        if(resultVol == -30f)
            resultVol = -80f;


        mixer.SetFloat("musicVolume", resultVol);
        Debug.Log("Music Vol: " + resultVol + " " + sliderVal);
    }

    public void OnSFXSliderValueChanged(float sliderVal)
    {
        float resultVol = 30f * sliderVal - 30f;
        if(resultVol == -30f)
            resultVol = -80f;

        mixer.SetFloat("sfxVolume", resultVol);
        Debug.Log("SFX Vol: " + resultVol + " " + sliderVal);
    }

    // Update is called once per frame
    void Update()
    {
        // ? --- Per farlo fungere anche da PC
        if(Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if(father.activeInHierarchy)
            {

            }
            else
            {

                OnPauseButtonPressed();
            }
        }
    }
}
