using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseControl : MonoBehaviour
{

    private bool _paused = false;
    [SerializeField] private GameObject _panel;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!_paused)
            {
                Pause();
            }
            else
            {
                UnPause();
            }
        }
    }

    private void Pause()
    {
        Time.timeScale = 0;
        _panel.SetActive(true);
    }

    public void UnPause()
    {
        Time.timeScale = 1;
        _panel.SetActive(false);
    }

    public void Quit()
    {
        SceneManager.LoadScene(0);
    }
}
