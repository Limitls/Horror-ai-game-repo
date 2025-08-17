using System;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float reachRange = 3f;
    [SerializeField] private LayerMask whatIsInteractable;
    [SerializeField] private Camera fpsCamera;
    public KeyCode interactKeycode = KeyCode.E;

    public IInteractable CurrentInteractable;

    private void Update()
    {
        HandleRaycast();
    }

    private void HandleRaycast()
    {
        RaycastHit hit;
        bool hasHit = Physics.Raycast(fpsCamera.transform.position, fpsCamera.transform.forward, out hit, reachRange,whatIsInteractable);
        if (hasHit)
        {
            Debug.Log(hit.transform.name);

            if (hit.transform.GetComponent<IInteractable>() != null)
            {

                if (CurrentInteractable == null) 
                {
                    CurrentInteractable = hit.transform.GetComponent<IInteractable>();
                    CurrentInteractable.OnInteractEnter();
                }
                if (Input.GetKeyDown(interactKeycode))
                {
                    CurrentInteractable.OnInteracted();
                }
            }
        }
        else
        {
            ResetInteractables();
        }
    }

    public void ResetInteractables()
    {
        if (CurrentInteractable != null)
        {
            CurrentInteractable.OnInteractExit();
            CurrentInteractable = null;
        }
    }
}