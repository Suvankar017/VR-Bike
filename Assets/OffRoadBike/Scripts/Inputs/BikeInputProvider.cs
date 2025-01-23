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
        [SerializeField] private KeyCode m_ReverseKey = KeyCode.S;
        [SerializeField] private string m_SteerAxisName = "Horizontal";
        [SerializeField] private KeyCode m_HandBrakeKey = KeyCode.Space;
        [SerializeField] private KeyCode m_ClutchKey = KeyCode.LeftShift;
        [SerializeField] private KeyCode m_GearUpKey = KeyCode.UpArrow;
        [SerializeField] private KeyCode m_GearDownKey = KeyCode.DownArrow;

        [FlexiableHeader("Input Asset", 18)]
        [SerializeField, RequiredField] private BikeInputSO m_BikeInputSO;

        [FlexiableHeader("Inspector", 18)]
        [SerializeField, Range(0.0f, 1.0f)] private float m_Accelerate;
        [SerializeField, Range(0.0f, 1.0f)] private float m_Reverse;
        [SerializeField, Range(-1.0f, 1.0f)] private float m_Steer;
        [SerializeField] private bool m_HandBrake = false;
        [SerializeField] private bool m_Clutch = false;
        [SerializeField, Range(0.0f, 1.0f)] private float m_GearUp;
        [SerializeField, Range(0.0f, 1.0f)] private float m_GearDown;


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
            float accelerate = Input.GetKey(m_AccelerateKey) ? 1.0f : 0.0f;
            float reverse = Input.GetKey(m_ReverseKey) ? 1.0f : 0.0f;
            float steer = Input.GetAxisRaw(m_SteerAxisName);
            bool handBrake = Input.GetKey(m_HandBrakeKey);
            bool clutch = Input.GetKey(m_ClutchKey);

            float gearUp = 0.0f;
            float gearDown = 0.0f;
            const float gearFullTimeDifference = 0.25f;

            if (Input.GetKey(m_GearUpKey))
            {
                gearUp = 0.5f;

                if (Input.GetKeyDown(m_GearUpKey))
                    m_NextGearFullUpTime = Time.unscaledTime + gearFullTimeDifference;

                if (Time.unscaledTime > m_NextGearFullUpTime)
                    gearUp = 1.0f;
            }

            if (Input.GetKey(m_GearDownKey))
            {
                gearDown = 0.5f;

                if (Input.GetKeyDown(m_GearDownKey))
                    m_NextGearFullDownTime = Time.unscaledTime + gearFullTimeDifference;

                if (Time.unscaledTime > m_NextGearFullDownTime)
                    gearDown = 1.0f;
            }

            m_InputData.accelerate = accelerate;
            m_InputData.reverse = reverse;
            m_InputData.steer = steer;
            m_InputData.handBrake = handBrake;
            m_InputData.clutch = clutch;

            float gear = 0.0f;

            if (gearUp > 0.0f)
                gear = gearUp;
            else if (gearDown > 0.0f)
                gear = -gearDown;

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

            float gear = 0.0f;

            if (m_GearUp > 0.0f)
                gear = m_GearUp;
            else if (m_GearDown > 0.0f)
                gear = -m_GearDown;

            m_InputData.gear = gear;

            m_Controller.SetInput(m_InputData);
        }
    }
}
