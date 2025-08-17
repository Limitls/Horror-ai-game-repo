using UnityEngine;

public class RotateX : MonoBehaviour
{
    [SerializeField] private float speed = 5;

    void Update()
    {
        if (GameManager.insance.IsPowerOn == false) return;
        transform.Rotate(0, 0, speed, Space.Self);
    }
}
