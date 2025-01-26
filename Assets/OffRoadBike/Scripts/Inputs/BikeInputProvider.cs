using UnityEngine;

namespace ReVR.Vehicles.Bike
{
    [AddComponentMenu("ReVR/Vehicles/Bike/Input Provider")]
    public class BikeInputProvider : MonoBehaviour
    {
        public enum InputProviderType
        {
            Keyboard,
            InputAsset,
            Inspector
        }


        [SerializeField] private InputProviderType m_ProviderType;
        [SerializeField, RequiredField] private BikeController m_Controller;


        [FlexiableHeader("Keyboard", 18)]
        [SerializeField] private KeyCode m_AccelerateKey = KeyCode.W;
        [SerializeField, Range(0.0f, 1.0f)] private float m_AccelerateMultiplier = 1.0f;
        [SerializeField] private KeyCode m_ReverseKey = KeyCode.S;
        [SerializeField, Range(0.0f, 1.0f)] private float m_ReverseMultiplier = 1.0f;
        [SerializeField] private string m_SteerAxisName = "Horizontal";
        [SerializeField, Range(0.0f, 1.0f)] private float m_SteerMultiplier = 1.0f;
        [SerializeField] private KeyCode m_HandBrakeKey = KeyCode.Space;
        [SerializeField] private KeyCode m_ClutchKey = KeyCode.LeftShift;
        [SerializeField] private KeyCode m_GearUpKey = KeyCode.UpArrow;
        [SerializeField] private KeyCode m_GearDownKey = KeyCode.DownArrow;
        [SerializeField, Range(0.0f, 1.0f)] private float m_GearHalfMultiplier = 0.5f;
        [SerializeField, Range(0.0f, 1.0f)] private float m_GearFullMultiplier = 1.0f;

        [FlexiableHeader("Input Asset", 18)]
        [SerializeField, RequiredField] private BikeInputSO m_BikeInputSO;

        [FlexiableHeader("Inspector", 18)]
        [SerializeField, Range(0.0f, 1.0f)] private float m_Accelerate;
        [SerializeField, Range(0.0f, 1.0f)] private float m_Reverse;
        [SerializeField, Range(-1.0f, 1.0f)] private float m_Steer;
        [SerializeField] private bool m_HandBrake = false;
        [SerializeField] private bool m_Clutch = false;
        [SerializeField, Range(-1.0f, 1.0f)] private float m_Gear;


        private BikeInput m_InputData;
        private float m_NextGearFullUpTime;
        private float m_NextGearFullDownTime;


        private void Update()
        {
            if (m_Controller == null)
                return;

            switch (m_ProviderType)
            {
                case InputProviderType.Keyboard:
                    HandleKeyboardInput();
                    break;

                case InputProviderType.InputAsset:
                    HandleInputAsset();
                    break;

                case InputProviderType.Inspector:
                    HandleInspectorInput();
                    break;
            }
        }

        private void HandleKeyboardInput()
        {
            float accelerate = Input.GetKey(m_AccelerateKey) ? m_AccelerateMultiplier : 0.0f;
            float reverse = Input.GetKey(m_ReverseKey) ? m_ReverseMultiplier : 0.0f;
            float steer = Input.GetAxisRaw(m_SteerAxisName) * m_SteerMultiplier;
            bool handBrake = Input.GetKey(m_HandBrakeKey);
            bool clutch = Input.GetKey(m_ClutchKey);

            float gear = 0.0f;
            const float gearFullTimeDifference = 0.25f;

            if (Input.GetKey(m_GearUpKey))
            {
                gear = m_GearHalfMultiplier;

                if (Input.GetKeyDown(m_GearUpKey))
                    m_NextGearFullUpTime = Time.unscaledTime + gearFullTimeDifference;

                if (Time.unscaledTime > m_NextGearFullUpTime)
                    gear = m_GearFullMultiplier;
            }

            if (Input.GetKey(m_GearDownKey))
            {
                gear = -m_GearHalfMultiplier;

                if (Input.GetKeyDown(m_GearDownKey))
                    m_NextGearFullDownTime = Time.unscaledTime + gearFullTimeDifference;

                if (Time.unscaledTime > m_NextGearFullDownTime)
                    gear = -m_GearFullMultiplier;
            }

            m_InputData.accelerate = accelerate;
            m_InputData.reverse = reverse;
            m_InputData.steer = steer;
            m_InputData.handBrake = handBrake;
            m_InputData.clutch = clutch;
            m_InputData.gear = gear;

            m_Controller.SetInput(m_InputData);
        }

        private void HandleInputAsset()
        {
            if (m_BikeInputSO == null)
                return;

            m_Controller.SetInput(m_BikeInputSO.input);
        }

        private void HandleInspectorInput()
        {
            m_InputData.accelerate = m_Accelerate;
            m_InputData.reverse = m_Reverse;
            m_InputData.steer = m_Steer;
            m_InputData.handBrake = m_HandBrake;
            m_InputData.clutch = m_Clutch;
            m_InputData.gear = m_Gear;

            m_Controller.SetInput(m_InputData);
        }
    }
}
