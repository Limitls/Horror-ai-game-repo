using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance = null;
    [SerializeField] private AudioClip switchCameraSFX;
    public Camera[] AllCameras;
    private bool isInCameraMode;

    public bool IsInCameraMode { get { return isInCameraMode; } }

    private void Awake()
    {
        instance = this;

        for (int i = 0; i < AllCameras.Length; i++)
        {
            AllCameras[i].gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if(isInCameraMode && Input.GetKeyDown(KeyCode.Tab))
        {
            for (int i = 0; i < AllCameras.Length; i++)
            {
                AllCameras[i].gameObject.SetActive(false);
            }
            ComputerManager.instance.ActivateTerminal();
            isInCameraMode = false;
        }
    }

    public void ActivateCamera(int cameraNumber)
    {
        SoundManager.instance.PlaySound(switchCameraSFX);
        isInCameraMode = true;
        ComputerManager.instance.DisableTerminalUI();
        for (int i = 0; i < AllCameras.Length; i++)
        {
            AllCameras[i].gameObject.SetActive(false);
        }
        AllCameras[cameraNumber - 1].gameObject.SetActive(true);
    }

    public void DisableAllCameras()
    {
        for (int i = 0; i < AllCameras.Length; i++)
        {
            AllCameras[i].gameObject.SetActive(false);
        }
        isInCameraMode = false;
    }
}