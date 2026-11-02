using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMainMenu : MonoBehaviour
{
    public void PlayButton()
    {
        SceneManager.LoadScene("Tetris");
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
