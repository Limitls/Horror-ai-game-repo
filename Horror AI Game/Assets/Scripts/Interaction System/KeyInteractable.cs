using UnityEngine;

public class KeyInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string keyName;
    [SerializeField] private string interactMessage;
    [SerializeField] private string pickUpMessage;
    [SerializeField] private AudioClip pickUpSFX;
    public void OnInteractEnter()
    {
        UIManager.instance.ShowInteractionText(interactMessage);
    }

    public void OnInteracted()
    {
        SoundManager.instance.PlaySound(pickUpSFX);
        UIManager.instance.ShowPopUp(pickUpMessage,3);
        Inventory.AddKey(keyName);
        gameObject.SetActive(false);
    }

    public void OnInteractExit()
    {
        UIManager.instance.DisableInteraction();
    }
}