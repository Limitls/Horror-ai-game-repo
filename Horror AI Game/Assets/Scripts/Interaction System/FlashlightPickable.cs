using UnityEngine;

public class FlashlightPickable : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactMessage;
    [SerializeField] private string pickUpMessage;
    [SerializeField] private AudioClip pickUpSFX;
    [SerializeField] private GameObject flashlight;

    public void OnInteractEnter()
    {
        UIManager.instance.ShowInteractionText(interactMessage);
    }

    public void OnInteracted()
    {
        SoundManager.instance.PlaySound(pickUpSFX);
        UIManager.instance.ShowPopUp(pickUpMessage, 3);
        flashlight.SetActive(true);
        gameObject.SetActive(false);
    }

    public void OnInteractExit()
    {
        UIManager.instance.DisableInteraction();
    }
}
