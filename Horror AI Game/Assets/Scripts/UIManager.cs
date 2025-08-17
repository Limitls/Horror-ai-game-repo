using System.Collections;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] private GameObject interactionUI;
    [SerializeField] private TextMeshProUGUI interactionText;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private TextMeshProUGUI popUpUILabel;
    [SerializeField] private GameObject staticUIVideo;

    private PlayerInteraction playerInteraction;

    [Header("Flags")]
    public bool StaticActive;

    private void Awake()
    {
        instance = this;

        popUpUILabel.gameObject.SetActive(false);
    }

    private void Start()
    {
        playerInteraction = FindObjectOfType<PlayerInteraction>();

        DisableInteraction();
    }

    public void ShowInteractionText(string text)
    {
        interactionUI.SetActive(true);
        interactionText.text = "[" + playerInteraction.interactKeycode.ToString() + "]"
            + text;
    }

    public void DisableInteraction()
    {
        interactionUI.SetActive(false);
    }

    public void SetCrosshairStatus(bool visible)
    {
        crosshair.SetActive(visible);
    }

    public void ShowPopUp(string message, float duration)
    {
        popUpUILabel.gameObject.SetActive(false);
        StopAllCoroutines();
        StartCoroutine(ShowPopUpSequence(message,duration));
    }

    private IEnumerator ShowPopUpSequence(string message, float duration)
    {
        yield return null;
        popUpUILabel.text = message;
        popUpUILabel.gameObject.SetActive(true);

        yield return new WaitForSeconds(duration);
        popUpUILabel.gameObject.SetActive(false);
    }

    public void ShowStatic(float duration)
    {
        if (StaticActive)
            return;

        StartCoroutine(ShowStaticSequence(duration));
    }
    private IEnumerator ShowStaticSequence(float duration)
    {
        StaticActive = true;
        staticUIVideo.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        staticUIVideo.gameObject.SetActive(false);
        StaticActive = false;
    }
}