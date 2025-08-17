using UnityEngine;

public class ShotgunInteractable : MonoBehaviour,IInteractable
{
    [SerializeField] private string interactMessage;
    [SerializeField] private AudioClip pickUpSFX;
    public void OnInteractEnter()
    {
        UIManager.instance.ShowInteractionText(interactMessage);
    }

    public void OnInteracted()
    {
        SoundManager.instance.PlaySound(pickUpSFX);
        GameManager.insance.ShootStalker();
        gameObject.SetActive(false);
    }

    public void OnInteractExit()
    {
        UIManager.instance.DisableInteraction();
    }
}
