using EvolveGames;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager insance = null;

    public bool IsPowerOn = true;

    [SerializeField] private KeyInteractable room03Key;
    [SerializeField] private EnemyAI stalkerAI;
    [SerializeField] private GameObject fakeStalker;
    [SerializeField] private GameObject cutsceneStalker;

    [SerializeField] private GameObject gameOverStatic;
    [SerializeField] private GameObject gameOverHolder;
    [SerializeField] private AudioClip jumpscareSFX;
    [SerializeField] private GameObject blackScreen;
    [Header("Game states")]
    [SerializeField] private float timeToSpawnStalker = 5;
    [SerializeField] private Door doorToUnlock;

    [Header("Shoot stalker cutscene")]
    [SerializeField] private GameObject flashlight;
    [SerializeField] private Animator shotgun;
    [SerializeField] private GameObject shotfgunMuzzleFlash;
    [SerializeField] private AudioClip shotgunSound;
    // Flags
    [Header("Flags")]
    [SerializeField] bool hasCeilingFanEvenTriggered = false;
    public bool IsGameOver = false;
    public bool HasStalkerSpawned = false;
    private float timeToSpawnStalkerTimer = 0.0f;

    private void Awake()
    {
        insance = this;
        stalkerAI.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!HasStalkerSpawned)
        {
            timeToSpawnStalkerTimer += Time.deltaTime;
            if(timeToSpawnStalkerTimer > timeToSpawnStalker)
            {
                stalkerAI.gameObject.SetActive(true);
                HasStalkerSpawned = true;
                timeToSpawnStalkerTimer = 0;
                fakeStalker.SetActive(false);
                doorToUnlock.isLocked = false;
                ComputerManager.instance.SwitchToSurvivalMode();
            }
        }
    }

    public void CheckForCeilingFanEvent()
    {
        if (hasCeilingFanEvenTriggered)
            return;

        // This is hardcoded but it's a one time thing, it makes the key look like it has dropped in the table
        // Normally you shouldn't hardcode references
        room03Key.transform.SetParent(null);

        room03Key.transform.position = new Vector3(13.86f, 1.26f, 12.174f);

        FindObjectOfType<EnemyAI>().isInBottomFloor = false;
    }

    public void ShowGameOverAnimation()
    {
        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        IsGameOver = true;
        PlayerController.instance.ControllerDisabled = true;
        PlayerController.instance.gameObject.SetActive(false);
        stalkerAI.gameObject.SetActive(false);
        gameOverStatic.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        gameOverStatic.SetActive(false);
        gameOverHolder.SetActive(true);
        SoundManager.instance.PlaySound(jumpscareSFX);
        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene(0);
    }

    public void ShootStalker()
    {
        StartCoroutine(ShootStalkerSequence());
    }
    private IEnumerator ShootStalkerSequence()
    {
        yield return new WaitForSeconds(1.0f);
        IsGameOver = true;
        PlayerController.instance.ControllerDisabled = true;
        UIManager.instance.ShowStatic(1.1f);
        yield return new WaitForSeconds(0.4f);

        stalkerAI.gameObject.SetActive(false);
        PlayerController.instance.Camera.transform.localRotation = Quaternion.identity;
        PlayerController.instance.Camera.transform.localPosition = Vector3.zero;
        PlayerController.instance.GetComponent<Animator>().enabled = true;

        cutsceneStalker.gameObject.SetActive(true);
        flashlight.gameObject.SetActive(false);
        shotgun.gameObject.SetActive(true);

        yield return new WaitForSeconds(3);

        yield return null;
        shotgun.CrossFadeInFixedTime("Shoot", 0.1f);

        yield return new WaitForSeconds(0.1f);
        shotfgunMuzzleFlash.gameObject.SetActive(true);
        SoundManager.instance.PlaySound(shotgunSound);

        yield return new WaitForSeconds(0.05f);
        blackScreen.SetActive(true);

        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene(0);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}