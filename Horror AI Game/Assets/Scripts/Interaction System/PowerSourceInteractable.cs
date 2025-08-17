using EvolveGames;
using UnityEngine;

public class PowerSourceInteractable : MonoBehaviour, IInteractable
{
    [Header("Power Settings")]
    public bool isPoweredOn = true;
    public Light[] connectedLights;

    [Header("Knob Settings")]
    public Transform knobLever;
    public float leverRotationAngle = 45f;
    public float leverRotateSpeed = 5f;

    [Header("SFX")]
    public AudioClip interactSFX;

    private Quaternion leverOnRotation;
    private Quaternion leverOffRotation;
    private bool canBeInteractedWith = true;
    private float restoreTimer;

    private void Start()
    {
        if (knobLever != null)
        {
            leverOnRotation = knobLever.localRotation;
            leverOffRotation = leverOnRotation * Quaternion.Euler(leverRotationAngle, 0, 0);
        }

        UpdateLights();
    }

    public void OnInteractEnter()
    {
        UIManager.instance.ShowInteractionText(isPoweredOn ? "Turn Off" : "Turn On");
    }

    public void OnInteracted()
    {
        if (!canBeInteractedWith)
        {
            SoundManager.instance.PlaySound(interactSFX);

            UIManager.instance.ShowPopUp("You need to wait before interacting with the power source again",3);
            return;
        }
        isPoweredOn = !isPoweredOn;
        GameManager.insance.IsPowerOn = isPoweredOn;

        canBeInteractedWith = false;
        UpdateLights();
        SoundManager.instance.PlaySound(interactSFX);
        PlayerController.instance.GetComponent<PlayerInteraction>().ResetInteractables();

        if (!isPoweredOn)
        {
            GameManager.insance.CheckForCeilingFanEvent();
        }
    }

    public void OnInteractExit()
    {
        UIManager.instance.DisableInteraction();
    }

    private void Update()
    {
        if (knobLever != null)
        {
            Quaternion targetRot = isPoweredOn ? leverOnRotation : leverOffRotation;
            knobLever.localRotation = Quaternion.Slerp(knobLever.localRotation, targetRot, Time.deltaTime * leverRotateSpeed);
        }

        if (!canBeInteractedWith)
        {
            restoreTimer += Time.deltaTime;
            if(restoreTimer > 60)
            {
                canBeInteractedWith = true;
                restoreTimer = 0;
            }
        }
    }

    private void UpdateLights()
    {
        foreach (Light light in connectedLights)
        {
            if (light != null)
                light.enabled = isPoweredOn;
        }
    }
}
