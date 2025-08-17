using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [Header("Door Settings")]
    public bool isLocked = false;
    public Transform aiInvestigationPoint;
    public Transform roomInvestigationPoint;

    [SerializeField] private string lockedMessage = "Door is locked";
    public string requiredKey = ""; 
    public float openAngle = 90f;
    public float openSpeed = 2f;
    public AudioClip doorOpenSFX, doorCloseSFX,lockedSFX,unlockedSFX;
    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool canInteract;

    public bool IsOpen { get { return isOpen; } }

    private void Start()
    {
        closedRotation = transform.rotation;
        openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, openAngle, 0));
    }

    private void Update()
    {
        Quaternion targetRotation = isOpen ? openRotation : closedRotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * openSpeed);
    }

    public void OnInteractEnter()
    {
        canInteract = true;
        UIManager.instance.ShowInteractionText(isOpen ?
            "Close Door" : "Open Door");
    }

    public void OnInteracted()
    {
        AttemptToOpenDoor();
    }

    public bool AttemptToOpenDoor(bool isPlayer = true)
    {
        if (isLocked)
        {
            if (Inventory.HasKey(requiredKey) && isPlayer) // Check if player has key
            {
                isLocked = false;
                Inventory.UseKey(requiredKey);

                if(isPlayer) UIManager.instance.ShowPopUp("Unlocked", 2);

                // SoundManager.instance.PlaySound(unlockedSFX);
                AudioSource.PlayClipAtPoint(unlockedSFX, transform.position);
            }
            else
            {
                if(isPlayer) UIManager.instance.ShowPopUp(lockedMessage, 3);
                //  SoundManager.instance.PlaySound(lockedSFX);
                AudioSource.PlayClipAtPoint(lockedSFX, transform.position);
                return false;
            }
        }
        isOpen = !isOpen;
        AudioSource.PlayClipAtPoint(isOpen ? doorOpenSFX : doorCloseSFX, transform.position);
        FindObjectOfType<PlayerInteraction>().ResetInteractables();
        return true;
    }

    public void OnInteractExit()
    {
        canInteract = false;

        Debug.Log("Stopped interacting with door");
        UIManager.instance.DisableInteraction();
    }
}
