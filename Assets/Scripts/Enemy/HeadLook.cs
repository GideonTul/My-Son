using UnityEngine;

public class HeadLook : MonoBehaviour
{
    [SerializeField] private Transform head;
    [SerializeField] private float turnSpeed = 5f;

    private Transform target;
    private bool shouldLook;

    void Update()
    {
        if (!shouldLook || target == null)
            return;

        Vector3 direction = target.position - head.position;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        head.rotation = Quaternion.Slerp(
            head.rotation,
            targetRotation,
            turnSpeed * Time.deltaTime);
    }

    public void LookAt(Transform player)
    {
        target = player;
        shouldLook = true;
    }

    public void StopLooking()
    {
        shouldLook = false;
    }
}