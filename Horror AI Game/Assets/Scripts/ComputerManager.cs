using EvolveGames;
using player2_sdk;
using System.Collections;
using UnityEngine;

public class ComputerManager : MonoBehaviour
{
    public static ComputerManager instance = null;

    [SerializeField] private GameObject terminalUI;
    [SerializeField] private GameObject camerasUI;
    [SerializeField] private Camera terminalCamera;
    [SerializeField] private AudioSource computerAudioSource;
    [SerializeField] private GameObject monitorRendertexture;
    [SerializeField] private GameObject player2PopUp;

    [Header("UI")]
    [SerializeField] private GameObject terminalInputUI;
    private Player2Npc player2Npc = null;

    [Header("Flags")]
    public bool isAIThinking = false;
    public bool IsOperatingTerminal = false;
    private bool hasBeenBooted;
    private bool isTryingToConnect;
    private float connectionTimer = 0.0f;

    const float CONNECTION_TIME_OUT_TIME = 10;

    private void Awake()
    {
        instance = this;
        player2Npc = GetComponent<Player2Npc>();
        computerAudioSource.Stop();
        terminalInputUI.SetActive(false);
    }

    private IEnumerator AttemptToConnectWithPlayer2Client()
    {
        Terminal.WriteLine("Attempting to connect...");
        isTryingToConnect = true;

        player2Npc.AttemptToConnect();
        yield return null;
        yield return new WaitUntil(() => player2Npc.IsConnecting == false);
        isTryingToConnect = false;
        if (player2Npc.HasConnectedWithTheClient)
        {
            Terminal.WriteLine("Succesfully connected with Player2 Client");

            yield return null;
            yield return new WaitForSeconds(1.0f);
        }
        else
        {
            ShowAIPopUp();
        }
    }

    private void Update()
    {
        if (isTryingToConnect)
        {
            connectionTimer += Time.deltaTime;
            if(connectionTimer > CONNECTION_TIME_OUT_TIME)
            {
                ShowAIPopUp();
            }
        } 

        if (Input.GetKeyDown(KeyCode.Tab) && IsOperatingTerminal && !CameraManager.instance.IsInCameraMode)
        {
            terminalCamera.gameObject.SetActive(false);
            CameraManager.instance.DisableAllCameras();
            PlayerController.instance.ControllerDisabled = false;
            PlayerController.instance.GetComponent<PlayerInteraction>().ResetInteractables();
            terminalCamera.gameObject.SetActive(false);
            UIManager.instance.SetCrosshairStatus(true);
            terminalInputUI.SetActive(false);
            DisableTerminalUI();
            camerasUI.SetActive(false);
            computerAudioSource.Stop();
            monitorRendertexture.SetActive(false);
        }
    }

    private void ShowAIPopUp()
    {
        isTryingToConnect = false;
        isAIThinking = false;
        Terminal.WriteLine("Player2 Client required, download or restart the client");
        player2PopUp.gameObject.SetActive(true);
        GameManager.insance.IsGameOver = true;
        Time.timeScale = 0.0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        connectionTimer = 0;
    }

    public void SwitchToSurvivalMode()
    {
        player2Npc.OnChatMessageSubmitted("##DEV##,you now know that a stalker exists, warn the player" +
        "just say that you've detected someone, don't add any details, just say that there's someone outside the house , make a prompt, you know know about all the puzzles in the game");
    }

    private void OnUserInput(string message)
    {
        if (message == string.Empty) return;
        // Check for commands
        if (message.ToLower() == "help")
        {
            Terminal.WriteLine("Available commands:\n" +
                "<Camera>\n" +
                "<Tips>");
            Terminal.WriteLine("----------------------------------------");
            return;
        }
        if (message.ToLower() == "camera")
        {
            Terminal.WriteLine("<CAMERA1>Outside Camera\n" +
                "<CAMERA2>Living Room Camera" +
                "\n<CAMERA3>Upstairs Camera");
            Terminal.WriteLine("----------------------------------------");
            return;
        }
        if(message.ToLower() == "camera1")
        {
            CameraManager.instance.ActivateCamera(1);
            return;
        }
        if (message.ToLower() == "camera2")
        {
            CameraManager.instance.ActivateCamera(2);
            return;
        }
        if (message.ToLower() == "camera3")
        {
            CameraManager.instance.ActivateCamera(3);
            return;
        }
        isAIThinking = true;
        player2Npc.OnChatMessageSubmitted(message);
        StartCoroutine(OnAIThinkingIndicator());
    }

    private IEnumerator OnAIThinkingIndicator()
    {
        Terminal.WriteLine("Thinking...");

        yield return null;
        while (isAIThinking)
        {
            yield return null;
        }
    }

    public void OnAIMessage(string message)
    {
        isAIThinking = false;

        // if the message is empty, don't do anything
        if (message == string.Empty) return;

        Terminal.ClearScreen();
        Terminal.WriteLine(message);
        Terminal.WriteLine("----------------------------------------");
    }

    public void ActivateTerminal()
    {
        monitorRendertexture.SetActive(true);

        computerAudioSource.Play();
        terminalInputUI.SetActive(true);

        UIManager.instance.SetCrosshairStatus(false);
        PlayerController.instance.ControllerDisabled = true;
        IsOperatingTerminal = true;

        terminalUI.SetActive(true);
        camerasUI.SetActive(false);
        terminalCamera.gameObject.SetActive(true);
        if (!hasBeenBooted)
        {
            StartCoroutine(AttemptToConnectWithPlayer2Client());
            hasBeenBooted = true;
        }
    }

    public void DisableTerminalUI()
    {
        IsOperatingTerminal = false;
        terminalUI.SetActive(false);
        camerasUI.SetActive(true);
    }
}
