using UnityEngine;
using UnityEngine.Events;

namespace ReVR.Vehicles.Bike
{
    [AddComponentMenu("ReVR/Vehicles/Bike/Aligner And Rotator")]
    public class BikeAlignerAndRotatorFeature : BikeFeature
    {
        [FlexiableHeader("Rotator Alignment", 18)]
        [Tooltip("Speed of aligning the rotator when the bike is on the ground. How fast bike align with the ground surface.")]
        [SerializeField] private float m_AlignRotatorSpeedInGround = 20.0f;

        [Tooltip("Speed of aligning the rotator when the bike is in the air. How fast bike align with the world up axis.")]
        [SerializeField] private float m_AlignRotatorSpeedInAir = 2.0f;

        [Tooltip("The main transform used for aligning the bike rotation with the ground.")]
        [SerializeField, RequiredField] private SharedTransform m_Rotator;


        [FlexiableHeader("Events", 18)]
        [Tooltip("Event triggered when the bike lands on the ground.")]
        [SerializeField] private UnityEvent m_OnGrounded;

        [Tooltip("Event triggered when the bike takes off from the ground.")]
        [SerializeField] private UnityEvent m_OnTakeOff;


        private BikeBlackboard m_Blackboard;
        private float m_ProjectedForwardOffsetAngle;


        public override void OnAwake(BikeBlackboard blackboard)
        {
            m_Blackboard = blackboard;
            m_Blackboard.rotator = m_Rotator;
        }

        public override void OnStart()
        {
            m_Blackboard.wasGrounded = m_Blackboard.isGrounded;

            // Initialize projected bike directions and angles
            m_Blackboard.projectedForward = m_Rotator.value.forward;
            m_Blackboard.projectedUp = m_Rotator.value.up;

            Vector3 wheelOffset = m_Blackboard.frontWheel.value.position - m_Blackboard.rearWheel.value.position;
            m_ProjectedForwardOffsetAngle = Vector3.Angle(m_Rotator.value.forward, wheelOffset.normalized);
        }

        public override void OnUpdate()
        {
            CalculateSurfaceParameters();
            AlignRotator(m_Rotator.value, m_Blackboard.projectedForward, m_Blackboard.projectedUp);
        }

        public override void OnPhysicsUpdate()
        {

        }

        private void CalculateSurfaceParameters()
        {
            m_Blackboard.isGrounded = m_Blackboard.isFrontWheelGrounded || m_Blackboard.isRearWheelGrounded;

            if (m_Blackboard.isFrontWheelGrounded && m_Blackboard.isRearWheelGrounded)
            {
                m_Blackboard.groundNormal = (m_Blackboard.frontWheelGroundNormal + m_Blackboard.rearWheelGroundNormal).normalized;
            }
            else if (m_Blackboard.isFrontWheelGrounded)
            {
                m_Blackboard.groundNormal = m_Blackboard.frontWheelGroundNormal;
            }
            else if (m_Blackboard.isRearWheelGrounded)
            {
                m_Blackboard.groundNormal = m_Blackboard.rearWheelGroundNormal;
            }
            else
            {
                m_Blackboard.groundNormal = Vector3.up;
            }

            if (m_Blackboard.isFrontWheelGrounded || m_Blackboard.isRearWheelGrounded)
            {
                Vector3 wheelDir = (m_Blackboard.frontWheel.value.position - m_Blackboard.rearWheel.value.position).normalized;
                m_Blackboard.projectedForward = Vector3.ProjectOnPlane(wheelDir, m_Rotator.value.right).normalized;
                Vector3 sidePlane = Vector3.ProjectOnPlane(m_Rotator.value.right, Vector3.up).normalized;
                Vector3 sideProjectedGroundNormal = Vector3.ProjectOnPlane(m_Blackboard.groundNormal, sidePlane).normalized;
                m_Blackboard.projectedUp = Vector3.ProjectOnPlane(sideProjectedGroundNormal, m_Blackboard.projectedForward).normalized;
            }

            if (m_Blackboard.isRearWheelGrounded && m_Blackboard.isDoingWheelie)
            {
                m_Blackboard.projectedForward = Vector3.ProjectOnPlane(m_Rotator.value.forward, Vector3.up).normalized;
                m_Blackboard.projectedUp = Vector3.up;
            }
            else if (m_Blackboard.isFrontWheelGrounded && m_Blackboard.isDoingStoppie)
            {
                m_Blackboard.projectedForward = Vector3.ProjectOnPlane(m_Rotator.value.forward, Vector3.up).normalized;
                m_Blackboard.projectedUp = Vector3.up;
            }
            else if (!m_Blackboard.isFrontWheelGrounded && !m_Blackboard.isRearWheelGrounded)
            {
                m_Blackboard.projectedForward = Vector3.ProjectOnPlane(m_Rotator.value.forward, m_Blackboard.groundNormal).normalized;
                m_Blackboard.projectedUp = m_Blackboard.groundNormal;
            }

            if (m_Blackboard.isGrounded != m_Blackboard.wasGrounded)
            {
                if (m_Blackboard.isGrounded)
                {
                    m_OnGrounded?.Invoke();
                }
                else
                {
                    m_OnTakeOff?.Invoke();
                }
            }

            m_Blackboard.wasGrounded = m_Blackboard.isGrounded;
        }

        private void AlignRotator(Transform rotator, Vector3 projectedForward, Vector3 projectedUp)
        {
            Vector3 newPorjectedForward = RotateVector(projectedForward, -rotator.right, m_ProjectedForwardOffsetAngle);
            Quaternion targetRotation = Quaternion.LookRotation(newPorjectedForward, projectedUp);
            float rotationSpeed = m_Blackboard.isGrounded ? m_AlignRotatorSpeedInGround : m_AlignRotatorSpeedInAir;

            // Snap to ground quickly
            if (m_Blackboard.totalCompression > m_Blackboard.maxCompression.value)
                rotationSpeed = 50.0f;

            rotator.rotation = Quaternion.Slerp(rotator.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        private Vector3 RotateVector(Vector3 vector, Vector3 axis, float angleInDegree)
        {
            return Quaternion.AngleAxis(angleInDegree, axis) * vector;
        }

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            if (Application.isPlaying && m_Rotator.value != null && m_Blackboard != null)
            {
                Vector3 up = m_Blackboard.projectedUp;
                Vector3 forward = m_Blackboard.projectedForward;
                Vector3 right = Vector3.Cross(up, forward).normalized;

                Vector3 position = m_Rotator.value.position;

                if (m_Blackboard.isFrontWheelGrounded && m_Blackboard.isRearWheelGrounded)
                    position = (m_Blackboard.frontWheelHit.point + m_Blackboard.rearWheelHit.point) * 0.5f;

                const float axisLength = 0.3f;

                Gizmos.color = Color.green;
                Gizmos.DrawRay(position, up * axisLength);

                Gizmos.color = Color.blue;
                Gizmos.DrawRay(position, forward * axisLength);

                Gizmos.color = Color.red;
                Gizmos.DrawRay(position, right * axisLength);
            }
        }

#endif

    }
}
