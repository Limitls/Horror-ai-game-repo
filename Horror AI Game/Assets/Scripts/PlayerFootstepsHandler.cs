using EvolveGames;
using UnityEngine;

public class PlayerFootstepsHandler : MonoBehaviour
{
   [Header("Footstep Settings")]
    public AudioClip[] footstepClips;
    public float walkStepInterval = 0.5f; 
    public float sprintStepInterval = 0.3f; 
    public float sprintSpeedThreshold = 6f; 

    private float stepTimer = 0f;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (GameManager.insance.IsGameOver) return;

        if (ComputerManager.instance.IsOperatingTerminal) return;
        if (CameraManager.instance.IsInCameraMode) return;

        if (PlayerController.instance.Moving)
        {
            float interval = PlayerController.instance.isRunning ? sprintStepInterval : walkStepInterval;

            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0f)
            {
                PlayFootstep();
                stepTimer = interval;
            }
        }
        else
        {
            stepTimer = 0f; 
        }
    }

    private void PlayFootstep()
    {
        if (footstepClips.Length == 0 || audioSource == null)
            return;

        AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
        audioSource.PlayOneShot(clip,0.2f);
    }
}