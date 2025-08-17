using UnityEngine;

public interface IInteractable 
{
    void OnInteractEnter();
    void OnInteracted();
    void OnInteractExit();
}