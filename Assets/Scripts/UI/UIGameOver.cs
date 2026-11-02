using UnityEngine;
using UnityEngine.SceneManagement;

public class UIGameOver : MonoBehaviour
{
    [Header("References"), SerializeField]
    TetrisLogic tl;
    [SerializeField]
    GameObject father; 

    public void Awake()
    {
        father.SetActive(false);
    }

    public void HidePanel()
    {
        father.SetActive(false);
    }

    public void ShowPanel()
    {
        father.SetActive(true);
    }

    public void OnRetryButtonPressed()
    {
        tl.RestartMatch();
    }

    public void OnExitButtonPressed()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
