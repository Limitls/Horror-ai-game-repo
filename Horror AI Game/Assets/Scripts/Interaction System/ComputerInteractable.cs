using UnityEngine;

public class ComputerInteractable :MonoBehaviour, IInteractable
{
    [Header("sfx")]
    [SerializeField] private AudioClip enterComputerSFX;

    public void OnInteractEnter()
    {
        if (ComputerManager.instance.IsOperatingTerminal) return;
        if (CameraManager.instance.IsInCameraMode) return;

        UIManager.instance.ShowInteractionText("Sit");
    }

    public void OnInteracted()
    {
        if (ComputerManager.instance.IsOperatingTerminal) return;
        if (CameraManager.instance.IsInCameraMode) return;
        if (GameManager.insance.IsGameOver) return;
        if(GameManager.insance.IsPowerOn == false)
        {
            SoundManager.instance.PlaySound(enterComputerSFX);

            UIManager.instance.ShowPopUp("Power is off", 3);
            return;
        }
        UIManager.instance.DisableInteraction();
        SoundManager.instance.PlaySound(enterComputerSFX);

        ComputerManager.instance.ActivateTerminal();
    }

    public void OnInteractExit()
    {
        if (ComputerManager.instance.IsOperatingTerminal) return;
        if (CameraManager.instance.IsInCameraMode) return;
        UIManager.instance.DisableInteraction();
    }
}