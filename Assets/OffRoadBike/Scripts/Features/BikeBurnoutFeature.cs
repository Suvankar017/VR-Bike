using UnityEngine;

namespace ReVR.Vehicles.Bike
{
    [AddComponentMenu("ReVR/Vehicles/Bike/Burnout")]
    public class BikeBurnoutFeature : BikeFeature
    {
        [FlexiableHeader("Burnout", 18)]
        [Tooltip("Speed of rotation during a burnout.")]
        [SerializeField] private float m_BurnoutRotationSpeed = 60.0f;

        [Tooltip("Smoothness of the burnout transition.")]
        [SerializeField] private float m_BurnoutSmoothness = 1.0f;


        private BikeBlackboard m_Blackboard;
        private float m_RotationAmount;
        private float m_RotationDirection;
        private float m_BurnoutLerp;


        public override void OnAwake(BikeBlackboard blackboard)
        {
            m_Blackboard = blackboard;
        }

        public override void OnStart()
        {

        }

        public override void OnUpdate()
        {
            ApplyBurnoutRotation();
        }

        public override void OnPhysicsUpdate()
        {
            HandleBurnoutAndRotation();
        }

        private void ApplyBurnoutRotation()
        {
            float timeFactor = Time.deltaTime / Time.fixedDeltaTime;

            if (m_Blackboard.isBurnoutRotating)
            {
                m_Blackboard.RotatorTransform.Rotate(Vector3.up, m_RotationAmount * m_RotationDirection * m_BurnoutLerp * timeFactor, Space.World);
            }
        }

        private void HandleBurnoutAndRotation()
        {
            bool isAccelerating = m_Blackboard.accelerateInput > 0.0f;
            bool isReversing = m_Blackboard.reverseInput > 0.0f;
            bool isSteering = !Mathf.Approximately(m_Blackboard.steerInput, 0.0f);

            m_Blackboard.isDoingBurnout = isAccelerating && isReversing && m_Blackboard.isFrontWheelGrounded && m_Blackboard.isRearWheelGrounded;
            m_Blackboard.isBurnoutRotating = isSteering && isAccelerating && isReversing && m_Blackboard.localVelocity.magnitude < 1.0f;

            if (m_Blackboard.isDoingBurnout)
            {
                ApplyBrake();
                m_Blackboard.isApplyingBrake = true;

                if (m_Blackboard.isBurnoutRotating)
                {
                    m_RotationDirection = m_Blackboard.steerInput;
                    m_BurnoutLerp = Mathf.MoveTowards(m_BurnoutLerp, 1.0f, m_BurnoutSmoothness * Time.fixedDeltaTime);

                    m_RotationAmount = m_BurnoutRotationSpeed * Time.fixedDeltaTime;

                    float distance = Vector3.Distance(m_Blackboard.frontWheel.value.position, m_Blackboard.RotatorTransform.position);
                    float speed = m_RotationAmount * distance;

                    Vector3 speedDirection = -m_RotationDirection * m_Blackboard.RotatorTransform.right;
                    Vector3 linearVelocity = speed * m_BurnoutLerp * speedDirection;

#if UNITY_6000_0_OR_NEWER
                    m_Blackboard.Rigidbody.linearVelocity = linearVelocity;
#else
                    m_Blackboard.Rigidbody.velocity = linearVelocity;
#endif

                }
                else
                {
                    m_BurnoutLerp = 0.0f;
                    m_RotationAmount = 0.0f;
                }
            }
        }

        private void ApplyBrake()
        {
            Vector3 brakeDirection = Vector3.ProjectOnPlane(m_Blackboard.projectedForward, m_Blackboard.groundNormal);
            Vector3 brakeForce = -m_Blackboard.deceleration.value * Mathf.Sign(m_Blackboard.localVelocity.z) * brakeDirection;

            // Clamp force to max force
            float maxForce = m_Blackboard.deceleration.value * Mathf.Abs(m_Blackboard.localVelocity.z);
            brakeForce = Vector3.ClampMagnitude(brakeForce, maxForce);
            m_Blackboard.Rigidbody.AddForce(brakeForce, ForceMode.Acceleration);
        }
    }
}
