using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private AudioClip buttonUISFX;
    [SerializeField] private GameObject faderUI;

    private bool isStartingGame = false;

    private void Awake()
    {
        faderUI.SetActive(false);
    }

    void Start()
    {
        // Fixes a bug where keys would already be unlocked in a new playthrough
        Inventory.ClearKeys();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Time.timeScale = 1.0f;
    }

    public void StartGame()
    {
        if (isStartingGame) return;
        isStartingGame = true;
        LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }

    IEnumerator LoadAsynchronously(int sceneIndex)
    {
        faderUI.SetActive(true);
        SoundManager.instance.PlaySound(buttonUISFX);
        yield return new WaitForSeconds(1.0f);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
