using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public PlayableDirector pd;
    public GameObject fade;

    public MusicTrack musicTrack;
    public GameObject mainMenu;
    public GameObject settingsMenu;
    private bool isPaused = false;

    IEnumerator PlayTimelineNextFrame()
    {
        yield return null;

        pd.Stop();
        pd.time = 0;
        pd.Evaluate();
        pd.Play();
    }

    void Start()
    {
        Debug.Log("Start called");

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isPaused)
        {
            Resume();
        }

    }

    void Awake()
    {
        Debug.Log("Menu Awake");
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
        //if (pd != null)
        //{
        //    pd.Play();
        //}

        //StartCoroutine(PlayTimelineNextFrame());
    }

    void OnDestroy()
    {
        Debug.Log("Menu Destroy");
    }

    public void Play()
    {
        SceneManager.LoadScene("LvlOne");
        fade.SetActive(false);
    }
    public void Settings()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
        isPaused = true;

    }
    public void Resume()
    {
        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
        isPaused = false;
    }
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
