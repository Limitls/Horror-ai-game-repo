using System;
using UnityEngine;
using UnityEngine.Assertions;

public class Keyboard : MonoBehaviour
{
    [SerializeField] AudioClip[] keyStrokeSounds;
    [SerializeField] Terminal connectedToTerminal;

    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        QualitySettings.vSyncCount = 0; 
        Application.targetFrameRate = 1000; 
        WarnIfTerminalNotConneced();
    }

    private void WarnIfTerminalNotConneced()
    {
        if (!connectedToTerminal)
        {
            Debug.LogWarning("Keyboard not connected to a terminal");
        }
    }

    private void Update()
    {
        // Don't do anything unless the terminal/computer is on
        if (!ComputerManager.instance.IsOperatingTerminal)
            return;

        // If the ai is generating a response don't let the player type anything
        if (ComputerManager.instance.isAIThinking)
            return;

        if (GameManager.insance.IsGameOver)
            return;
        bool isValidKey = Input.inputString.Length > 0;
        if (isValidKey)
        {
            PlayRandomSound();
        }
        if (connectedToTerminal)
        {
            connectedToTerminal.ReceiveFrameInput(Input.inputString);
        }
    }

    private void PlayRandomSound()
    {
        int randomIndex = UnityEngine.Random.Range(0, keyStrokeSounds.Length);
        audioSource.clip = keyStrokeSounds[randomIndex];
        audioSource.Play();
    }
}
