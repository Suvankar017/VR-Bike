using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace ReVR.Vehicles.Bike
{
    [AddComponentMenu("ReVR/Vehicles/Bike/Gear/Automatic Gear")]
    public class BikeAutomaticGearFeature : BikeGearFeature
    {
        [FlexiableHeader("Gears", 18)]
        [Tooltip("Current gear of the bike (Example :  0 is neutral).")]
        [SerializeField, Min(0)] private int m_CurrentGear = 0;

        [Tooltip("Max speed of each gears.")]
        [SerializeField] private float[] m_Gears = new float[] { 10.0f, 20.0f, 30.0f, 40.0f, 50.0f };

        [Tooltip("Shifting time of each gear, between this time bike cannot be accelerated.")]
        [SerializeField] private float m_GearShiftingTime = 0.2f;

        [FlexiableHeader("Events", 18)]
        [Tooltip("Event triggered when the gear changed.")]
        [SerializeField] private UnityEvent<int> m_OnGearChanged;


        private Coroutine m_ShiftGearCoroutine;


        public override void OnStart()
        {
            m_Blackboard.currentMaxSpeed = m_Blackboard.maxSpeed.value;
            m_CurrentGear = Mathf.Clamp(m_CurrentGear, 0, m_Gears.Length);

            m_OnGearChanged?.Invoke(m_CurrentGear);
        }

        public override void OnUpdate()
        {

        }

        public override void OnPhysicsUpdate()
        {
            float speed = m_Blackboard.localVelocity.magnitude;
            int nextGear = 0;

            for (int i = 0; i < m_Gears.Length; i++)
            {
                if (speed > m_Gears[i])
                    nextGear = i + 1;
                else
                    break;
            }

            if (m_CurrentGear != nextGear)
            {
                m_CurrentGear = nextGear;

                if (m_Blackboard.accelerateInput > 0.0f && m_Blackboard.localVelocity.z > 0.0f && m_Blackboard.isGrounded)
                {
                    m_OnGearChanged?.Invoke(m_CurrentGear);

                    if (m_ShiftGearCoroutine != null)
                        StopCoroutine(m_ShiftGearCoroutine);
                    m_ShiftGearCoroutine = StartCoroutine(ShiftGear());
                }
            }
        }

        private IEnumerator ShiftGear()
        {
            m_Blackboard.canAccelerate = false;

            if (!Mathf.Approximately(m_GearShiftingTime, 0.0f))
                yield return new WaitForSeconds(m_GearShiftingTime);

            m_Blackboard.canAccelerate = true;
        }
    }
}
