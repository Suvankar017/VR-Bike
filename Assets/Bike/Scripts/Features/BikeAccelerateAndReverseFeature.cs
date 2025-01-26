using UnityEngine;

namespace ReVR.Vehicles.Bike
{
    [AddComponentMenu("ReVR/Vehicles/Bike/Accelerate And Reverse")]
    public class BikeAccelerateAndReverseFeature : BikeFeature
    {
        [FlexiableHeader("Motion Dynamics", 18)]
        [Tooltip("Maximum speed the bike can reach in the forward direction.")]
        [SerializeField] private SharedFloat m_MaxSpeed = 60.0f;

        [Tooltip("Acceleration rate of the bike.")]
        [SerializeField] private float m_Acceleration = 15.0f;

        [Tooltip("Adjusts the acceleration. X-axis represents speed, Y-axis represents acceleration multiplier. Example: Curve from (0, 1) to (1, 0.5) means high acceleration at low speed, decreasing to half acceleration at max speed.")]
        [SerializeField] private AnimationCurve m_AccelerationCurve = new(new(0, 1), new(0.5f, 0.8f), new(1, 0.5f));

        [Tooltip("Maximum speed the bike can reach in reverse.")]
        [SerializeField] private float m_ReverseMaxSpeed = 3.0f;

        [Tooltip("Acceleration rate when the bike is in reverse.")]
        [SerializeField] private float m_ReverseAcceleration = 5.0f;

        [Tooltip("Adjusts the reverse acceleration. X-axis represents reverse speed, Y-axis represents acceleration multiplier.")]
        [SerializeField] private AnimationCurve m_ReverseAccelerationCurve = new(new(0, 1), new(1, 0.2f));

        [Tooltip("Rate at which the bike decelerates when opposite button(to the bike move direction) is pressed. Basically brake amount. How fast the bike will reach 0 speed.")]
        [SerializeField] private SharedFloat m_Deceleration = 20.0f;

        [Tooltip("Deceleration rate when the hand brake is applied.")]
        [SerializeField] private float m_HandBrakeDeceleration = 5.0f;


        [FlexiableHeader("Environmental Dynamics", 18)]
        [Tooltip("Resistance applied to the bike to simulate rolling friction. It determines how fast bike will come to rest (0 speed) if acceleration/deceleration input is not applied.")]
        [SerializeField] private float m_RollingResistance = 1.0f;


        private BikeBlackboard m_Blackboard;


        public override void OnAwake(BikeBlackboard blackboard)
        {
            m_Blackboard = blackboard;

            m_Blackboard.maxSpeed = m_MaxSpeed;
            m_Blackboard.deceleration = m_Deceleration;
            m_Blackboard.currentMaxSpeed = m_MaxSpeed.value;
        }

        public override void OnStart()
        {

        }

        public override void OnUpdate()
        {

        }

        public override void OnPhysicsUpdate()
        {
            HandleAccelerationAndReverse();
        }

        private void HandleAccelerationAndReverse()
        {
            if (!m_Blackboard.isGrounded || !m_Blackboard.canAccelerate || m_Blackboard.isDoingBurnout)
                return;

            float currentSpeed = m_Blackboard.localVelocity.z;
            float targetAcceleration = 0.0f;

            if (m_Blackboard.handBrakeInput)
            {
                targetAcceleration = (currentSpeed >= 0.0f) ? -m_HandBrakeDeceleration : m_HandBrakeDeceleration;
                m_Blackboard.isApplyingHandBrake = true;
            }
            else
            {
                m_Blackboard.isApplyingHandBrake = false;

                bool isAccelerating = m_Blackboard.accelerateInput > 0.0f;
                bool isReversing = m_Blackboard.reverseInput > 0.0f;

                bool isMaxSpeedNotZero = !Mathf.Approximately(m_Blackboard.currentMaxSpeed, 0.0f);
                bool isMaxSpeedDecreasing = m_Blackboard.localVelocity.magnitude > (m_Blackboard.currentMaxSpeed + 0.01f);

                if (isAccelerating && !m_Blackboard.isDoingStoppie && isMaxSpeedNotZero && !isMaxSpeedDecreasing && !m_Blackboard.clutchInput)
                {
                    float minAccel = Mathf.Min(m_Acceleration * m_Blackboard.accelerateInput * m_AccelerationCurve.Evaluate(m_Blackboard.localVelocity.magnitude / m_Blackboard.currentMaxSpeed), (m_Blackboard.currentMaxSpeed - currentSpeed) / Time.fixedDeltaTime);
                    targetAcceleration = (currentSpeed >= 0.0f) ? minAccel : m_Deceleration.value;

                    m_Blackboard.isApplyingBrake = currentSpeed < 0.0f;
                }
                else if (isReversing && !m_Blackboard.isDoingStoppie)
                {
                    float maxAccel = Mathf.Max(-m_ReverseAcceleration * m_Blackboard.reverseInput * m_ReverseAccelerationCurve.Evaluate(m_Blackboard.localVelocity.magnitude / m_ReverseMaxSpeed), -(m_ReverseMaxSpeed + currentSpeed) / Time.fixedDeltaTime);
                    targetAcceleration = (currentSpeed >= 0.0f) ? -m_Deceleration.value : maxAccel;

                    m_Blackboard.isApplyingBrake = currentSpeed >= 0.0f;
                }
                else
                {
                    ApplyRollingResistance();
                    m_Blackboard.isApplyingBrake = false;
                }
            }

            if (m_Blackboard.isApplyingHandBrake)
            {
                float clampedAcceleration = Mathf.Abs(currentSpeed / Time.fixedDeltaTime);
                targetAcceleration = Mathf.Clamp(targetAcceleration, -clampedAcceleration, clampedAcceleration);
            }

            Vector3 accelerationDirection = Vector3.ProjectOnPlane(m_Blackboard.projectedForward, m_Blackboard.groundNormal);
            Vector3 accelerationForce = accelerationDirection * targetAcceleration;

            m_Blackboard.Rigidbody.AddForce(accelerationForce, ForceMode.Acceleration);
        }

        private void ApplyRollingResistance()
        {
            Vector3 rollingResistanceDirection = Vector3.ProjectOnPlane(m_Blackboard.projectedForward, m_Blackboard.groundNormal);
            Vector3 rollingResistanceForce = -m_RollingResistance * Mathf.Sign(m_Blackboard.localVelocity.z) * rollingResistanceDirection;

            // Clamp force to max force
            float maxForce = m_RollingResistance * Mathf.Abs(m_Blackboard.localVelocity.z);
            rollingResistanceForce = Vector3.ClampMagnitude(rollingResistanceForce, maxForce);
            m_Blackboard.Rigidbody.AddForce(rollingResistanceForce, ForceMode.Acceleration);
        }

    }
}
