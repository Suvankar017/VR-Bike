using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace ReVR.Vehicles.Bike
{
    [AddComponentMenu("ReVR/Vehicles/Bike/Gear/Manual Gear")]
    public class BikeManualGearFeature : BikeGearFeature
    {
        [FlexiableHeader("Gears", 18)]
        [Tooltip("Current gear of the bike (Example :  0 is neutral).")]
        [SerializeField, Min(0)] private int m_CurrentGear = 0;

        [Tooltip("Max speed of each gears.")]
        [SerializeField] private float[] m_Gears = new float[4] { 10.0f, 20.0f, 30.0f, 40.0f };

        [Tooltip("Shifting time of each gear, between this time bike cannot be accelerated.")]
        [SerializeField] private float m_GearShiftingTime = 0.2f;

        [FlexiableHeader("Threshold", 18)]
        [Tooltip("Threshold for half gear input.")]
        [SerializeField, Range(0.0f, 1.0f)] private float m_GearHalfThreshold = 0.3f;

        [Tooltip("Threshold for full gear input.")]
        [SerializeField, Range(0.0f, 1.0f)] private float m_GearFullThreshold = 0.7f;

        [FlexiableHeader("Events", 18)]
        [Tooltip("Event triggered when the gear changed.")]
        [SerializeField] private UnityEvent<int> m_OnGearChanged;


        private Coroutine m_ShiftGearCoroutine;
        private bool m_CanGearHalfUp;
        private bool m_CanGearFullUp;
        private bool m_CanGearHalfDown;
        private bool m_CanGearFullDown;
        private bool m_HasGearChanged;

        private const float k_NeutralSpeed = 0.0f;


        private void OnValidate()
        {
            m_CurrentGear = Mathf.Clamp(m_CurrentGear, 0, m_Gears.Length);
        }

        public override void OnStart()
        {
            m_CurrentGear = Mathf.Clamp(m_CurrentGear, 0, m_Gears.Length);

            m_Blackboard.currentMaxSpeed = (m_CurrentGear == 0) ? k_NeutralSpeed : m_Gears[m_CurrentGear - 1];
            m_OnGearChanged?.Invoke(m_CurrentGear);
        }

        public override void OnUpdate()
        {
            float gearUp = (m_Blackboard.gearInput > 0.0f) ? m_Blackboard.gearInput : 0.0f;
            float gearDown = (m_Blackboard.gearInput < 0.0f) ? -m_Blackboard.gearInput : 0.0f;

            int prevGear = m_CurrentGear;


            if (gearUp > m_GearFullThreshold)
            {
                if (m_CanGearFullUp)
                {
                    m_CanGearFullUp = false;
                    OnGearFullUp();
                }
            }
            else if (gearUp > m_GearHalfThreshold)
            {
                if (m_CanGearHalfUp)
                {
                    m_CanGearHalfUp = false;
                    OnGearHalfUp();
                }
            }
            else
            {
                m_CanGearFullUp = true;
                m_CanGearHalfUp = true;
            }


            if (gearDown > m_GearFullThreshold)
            {
                if (m_CanGearFullDown)
                {
                    m_CanGearFullDown = false;
                    OnGearFullDown();
                }
            }
            else if (gearDown > m_GearHalfThreshold)
            {
                if (m_CanGearHalfDown)
                {
                    m_CanGearHalfDown = false;
                    OnGearHalfDown();
                }
            }
            else
            {
                m_CanGearFullDown = true;
                m_CanGearHalfDown = true;
            }


            if (m_CurrentGear != prevGear)
            {
                // Gear has changed...
                if (m_Blackboard.clutchInput)
                {
                    m_HasGearChanged = true;
                    m_OnGearChanged?.Invoke(m_CurrentGear);

                    if (m_ShiftGearCoroutine != null)
                        StopCoroutine(m_ShiftGearCoroutine);
                    m_ShiftGearCoroutine = StartCoroutine(ShiftGear());
                }
                else
                {
                    m_CurrentGear = prevGear;
                }
            }
        }

        public override void OnPhysicsUpdate()
        {
            if (m_HasGearChanged)
            {
                m_HasGearChanged = false;
                float maxSpeed = k_NeutralSpeed;

                if (m_CurrentGear > 0)
                    maxSpeed = m_Gears[m_CurrentGear - 1];

                m_Blackboard.currentMaxSpeed = maxSpeed;
            }
        }

        private IEnumerator ShiftGear()
        {
            m_Blackboard.canAccelerate = false;

            if (!Mathf.Approximately(m_GearShiftingTime, 0.0f))
                yield return new WaitForSeconds(m_GearShiftingTime);

            m_Blackboard.canAccelerate = true;
        }

        private void OnGearHalfUp()
        {
            if (m_CurrentGear == 1)
            {
                m_CurrentGear -= 1;
            }

            m_CurrentGear = Mathf.Clamp(m_CurrentGear, 0, m_Gears.Length);
        }

        private void OnGearFullUp()
        {
            if (m_CurrentGear == 0)
            {
                m_CurrentGear += 2;
            }
            else
            {
                m_CurrentGear += 1;
            }

            m_CurrentGear = Mathf.Clamp(m_CurrentGear, 0, m_Gears.Length);
        }

        private void OnGearHalfDown()
        {
            if (m_CurrentGear == 2)
            {
                m_CurrentGear -= 2;
            }

            m_CurrentGear = Mathf.Clamp(m_CurrentGear, 0, m_Gears.Length);
        }

        private void OnGearFullDown()
        {
            if (m_CurrentGear == 0)
            {
                m_CurrentGear++;
            }
            else if (m_CurrentGear == 1)
            {

            }
            else
            {
                m_CurrentGear--;
            }

            m_CurrentGear = Mathf.Clamp(m_CurrentGear, 0, m_Gears.Length);
        }
    }
}
