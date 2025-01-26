using UnityEngine;

namespace ReVR.Vehicles.Bike
{
    [AddComponentMenu("ReVR/Vehicles/Bike/Stoppie")]
    public class BikeStoppieFeature : BikeFeature
    {
        [FlexiableHeader("Stoppie", 18)]
        [Tooltip("Maximum stoppie angle of the bike.")]
        [SerializeField] private float m_MaxStoppieAngle = 30.0f;

        [Tooltip("Animation speed of stoppie, that will be used lerp from current rotation to target rotation.")]
        [SerializeField] private float m_StoppieAnimationSpeed = 3.0f;

        [Tooltip("<b>Adjusts the stoppie angle multiplier. X-axis represents normalized speed <i>(i.e, 0 means no speed, and 1 means max speed)</i>, Y-axis represents stoppie angle multiplier.</b> Example: Curve from (0, 0) to (0.1, 1) means nearly no stoppie at low speed, (0.1, 1) to (1, 1) means maximum stoppie at high speed. You will have to adjust it with multiple points to fine tune the stoppie.")]
        [SerializeField] private AnimationCurve m_StoppieCurve = new(new(0.0f, 0.0f), new(0.5f, 1.0f), new(1.0f, 1.0f));

        [Tooltip("The transform of the bike that is used to perform stoppie.")]
        [SerializeField, RequiredField] private Transform m_StoppieTransform;


        private BikeBlackboard m_Blackboard;


        public override void OnAwake(BikeBlackboard blackboard)
        {
            m_Blackboard = blackboard;
        }

        public override void OnStart()
        {

        }

        public override void OnUpdate()
        {
            StoppieAnimation();
        }

        public override void OnPhysicsUpdate()
        {

        }

        private void StoppieAnimation()
        {
            if (m_Blackboard.handBrakeInput && m_Blackboard.isFrontWheelGrounded && m_Blackboard.localVelocity.magnitude > 0.1f)
                m_Blackboard.isDoingStoppie = true;
            if (!m_Blackboard.handBrakeInput && m_Blackboard.isRearWheelGrounded)
                m_Blackboard.isDoingStoppie = false;

            const float acceptableAngleForStoppie = 20.0f;
            float surfaceAngle = Vector3.Angle(m_Blackboard.groundNormal, Vector3.up);
            float surfaceAngleFront = Vector3.Angle(m_Blackboard.frontWheelGroundNormal, Vector3.up);
            float surfaceAngleRear = Vector3.Angle(m_Blackboard.rearWheelGroundNormal, Vector3.up);

            bool snapStoppie = false;

            if (surfaceAngle > acceptableAngleForStoppie || surfaceAngleFront > acceptableAngleForStoppie || surfaceAngleRear > acceptableAngleForStoppie)
            {
                if (m_Blackboard.isDoingStoppie)
                    snapStoppie = true;
                m_Blackboard.isDoingStoppie = false;
            }

            if (m_Blackboard.isDoingStoppie)
            {
                if (m_Blackboard.handBrakeInput)
                {
                    float speedRatio = Mathf.Clamp01(m_Blackboard.localVelocity.z / m_Blackboard.maxSpeed.value);

                    float targetAngle = m_MaxStoppieAngle * m_StoppieCurve.Evaluate(speedRatio);
                    float currentAngle = m_StoppieTransform.localRotation.eulerAngles.x;
                    float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, m_StoppieAnimationSpeed * Time.deltaTime);
                    m_StoppieTransform.localRotation = Quaternion.Euler(newAngle, 0.0f, 0.0f);
                }
                else
                {
                    float targetAngle = 0.0f;
                    float currentAngle = m_StoppieTransform.localRotation.eulerAngles.x;
                    float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, m_StoppieAnimationSpeed * Time.deltaTime);
                    m_StoppieTransform.localRotation = Quaternion.Euler(newAngle, 0.0f, 0.0f);
                }
            }
            else
            {
                float targetAngle = 0.0f;
                float currentAngle = m_StoppieTransform.localRotation.eulerAngles.x;
                float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, m_StoppieAnimationSpeed * Time.deltaTime);
                if (snapStoppie)
                    newAngle = Mathf.LerpAngle(currentAngle, targetAngle, m_Blackboard.alignRotatorSpeedInGround.value * Time.deltaTime);

                m_StoppieTransform.localRotation = Quaternion.Euler(newAngle, 0.0f, 0.0f);
            }
        }
    }
}
