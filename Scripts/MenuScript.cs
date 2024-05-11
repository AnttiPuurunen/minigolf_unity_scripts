using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    [SerializeField] GameObject uICanvas;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject cMFreelookCamera;
    [SerializeField] GameObject resultsPanel;

    public static bool isPaused = false;

    private void Start()
    {
        Time.timeScale = 1.0f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !resultsPanel.activeInHierarchy)
        {
            // TODO: Physics update is not paused, if the menu isn't called before hitting the ball
            isPaused = !isPaused;
            GamePause();
        }
    }

    private void GamePause()
    {
       if (isPaused)
        {
            uICanvas.SetActive(false);
            pauseMenu.SetActive(true);
            cMFreelookCamera.SetActive(false);
            Time.timeScale = 0.0f;
        }
       else
        {
            pauseMenu.SetActive(false);
            uICanvas.SetActive(true);
            cMFreelookCamera.SetActive(true);
            Time.timeScale = 1.0f;
        }
    }

    public void OnContinueClick()
    {
        isPaused = !isPaused;
        GamePause();
    }

    public void LoadLevel(int i)
    {
        SceneManager.LoadScene(i);
        isPaused = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}