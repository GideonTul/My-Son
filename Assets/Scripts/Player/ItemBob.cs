using UnityEngine;
using StarterAssets;

public class ItemBob : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FirstPersonController player;
    [SerializeField] private StarterAssetsInputs input;

    [Header("Walking")]
    [SerializeField] private float walkBobSpeed = 7f;
    [SerializeField] private float sprintBobSpeed = 11f;
    [SerializeField] private float walkBobAmount = 0.015f;
    [SerializeField] private float sprintBobAmount = 0.025f;
    [SerializeField] private float horizontalAmount = 0.008f;

    [Header("Smoothing")]
    [SerializeField] private float returnSpeed = 10f;

    private Vector3 startPos;
    private float timer;

    private void Start()
    {
        startPos = transform.localPosition;
    }

    private void Update()
    {
        bool moving = input.move.sqrMagnitude > 0.01f && player.Grounded;

        if (moving)
        {
            float speed = input.sprint ? sprintBobSpeed : walkBobSpeed;
            float amount = input.sprint ? sprintBobAmount : walkBobAmount;

            timer += Time.deltaTime * speed;

            Vector3 offset = new Vector3(
                Mathf.Cos(timer * 0.5f) * horizontalAmount,
                Mathf.Sin(timer) * amount,
                0f
            );

            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                startPos + offset,
                Time.deltaTime * 15f
            );
        }
        else
        {
            timer = 0f;

            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                startPos,
                Time.deltaTime * returnSpeed
            );
        }
    }
}