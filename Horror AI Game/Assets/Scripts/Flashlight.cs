using UnityEngine;

public class Flashlight : MonoBehaviour
{
    [SerializeField] private Light light;
    [SerializeField] private AudioClip toggleSFX;

    private void Update()
    {
        if (ComputerManager.instance.IsOperatingTerminal) return;
        if (CameraManager.instance.IsInCameraMode) return;

        if (Input.GetKeyDown(KeyCode.T))
        {
            SoundManager.instance.PlaySound(toggleSFX);
            light.enabled = !light.enabled;
        }
    }
}
