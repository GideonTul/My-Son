using UnityEngine;

namespace JIYUMA.Piano
{
    public class PianoKeyPress : MonoBehaviour
    {
        [Header("Controller")]
        public PianoController controller;

        private Vector3 originalPos;
        private Quaternion originalRot;

        [Header("White Key Settings")]
        public float whitePressDepth = 0.0005f;
        public float whitePressAngle = 5f;

        [Header("Black Key Settings")]
        public float blackPressDepth = 0.01f;
        public float blackPressAngle = 0f;

        [Header("Pedal Settings")]
        public float pedalPressDepth = 0.011f;
        public float pedalPressAngle = 5f;

        [Header("Common Settings")]
        public float speed = 11f;

        [Header("Physics")]
        public string triggerTag = "Finger";

        private bool isPressed = false;
        private bool isBlackKey = false;
        private bool isPedal = false;

        private int physicsPressCount = 0;
        private bool clickPressed = false;

        void Start()
        {
            originalPos = transform.localPosition;
            originalRot = transform.localRotation;

            string lowerName = gameObject.name.ToLower();
            isBlackKey = lowerName.Contains("black");
            isPedal = lowerName.Contains("pedal");
        }

        //void OnMouseDown()
        //{
        //    if (!CanUseClick()) return;

        //    clickPressed = true;
        //    UpdatePressedState();
        //}

        //void OnMouseUp()
        //{
        //    if (!CanUseClick()) return;

        //    clickPressed = false;
        //    UpdatePressedState();
        //}
        public void PressExternally()
        {
            clickPressed = true;
            UpdatePressedState();
        }

        public void ReleaseExternally()
        {
            clickPressed = false;
            UpdatePressedState();
        }

        void OnTriggerEnter(Collider other)
        {
            if (!CanUseTrigger()) return;
            if (!other.CompareTag(triggerTag)) return;

            physicsPressCount++;
            UpdatePressedState();
        }

        void OnTriggerExit(Collider other)
        {
            if (!CanUseTrigger()) return;
            if (!other.CompareTag(triggerTag)) return;

            physicsPressCount = Mathf.Max(0, physicsPressCount - 1);
            UpdatePressedState();
        }

        bool CanUseClick()
        {
            return controller == null ? true : controller.enableClickInput;
        }

        bool CanUseTrigger()
        {
            return controller == null ? true : controller.enablePhysicsInput;
        }

        void UpdatePressedState()
        {
            bool clickAllowed = CanUseClick();
            bool triggerAllowed = CanUseTrigger();

            bool clickActive = clickAllowed && clickPressed;
            bool triggerActive = triggerAllowed && physicsPressCount > 0;

            isPressed = clickActive || triggerActive;
        }

        void Update()
        {
            if (!CanUseClick())
                clickPressed = false;

            if (!CanUseTrigger())
                physicsPressCount = 0;

            UpdatePressedState();

            float pressDepth;
            float pressAngle;

            if (isPedal)
            {
                pressDepth = pedalPressDepth;
                pressAngle = pedalPressAngle;
            }
            else if (isBlackKey)
            {
                pressDepth = blackPressDepth;
                pressAngle = blackPressAngle;
            }
            else
            {
                pressDepth = whitePressDepth;
                pressAngle = whitePressAngle;
            }

            Vector3 targetPos = isPressed
                ? originalPos + Vector3.down * pressDepth
                : originalPos;

            Quaternion targetRot = isPressed
                ? originalRot * Quaternion.Euler(pressAngle, 0f, 0f)
                : originalRot;

            float t = 1f - Mathf.Exp(-speed * Time.deltaTime);

            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, t);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRot, t);
        }
    }
}