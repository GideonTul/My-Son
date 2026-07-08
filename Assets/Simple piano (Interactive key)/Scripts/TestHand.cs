using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace JIYUMA.Piano
{
    [RequireComponent(typeof(Rigidbody))]
    public class TestHand : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float moveSpeed = 3f;

        private Rigidbody rb;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        void FixedUpdate()
        {
            Move();
        }

        void Move()
        {
            Vector3 move = Vector3.zero;

#if ENABLE_INPUT_SYSTEM
            Keyboard keyboard = Keyboard.current;

            if (keyboard != null)
            {
                float h = 0f;
                float v = 0f;
                float up = 0f;

                if (keyboard.aKey.isPressed) h -= 1f;
                if (keyboard.dKey.isPressed) h += 1f;

                if (keyboard.sKey.isPressed) v -= 1f;
                if (keyboard.wKey.isPressed) v += 1f;

                if (keyboard.qKey.isPressed) up -= 1f;
                if (keyboard.eKey.isPressed) up += 1f;

                Vector3 horizontalMove = (transform.right * h + transform.forward * v).normalized;
                Vector3 verticalMove = Vector3.up * up;

                move = horizontalMove + verticalMove;
            }
#else
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            float up = 0f;
            if (Input.GetKey(KeyCode.Q)) up = -1f;
            if (Input.GetKey(KeyCode.E)) up = 1f;

            Vector3 horizontalMove = (transform.right * h + transform.forward * v).normalized;
            Vector3 verticalMove = Vector3.up * up;

            move = horizontalMove + verticalMove;
#endif

            Vector3 targetPos = rb.position + move * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(targetPos);
        }
    }
}