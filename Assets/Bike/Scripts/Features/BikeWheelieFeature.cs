using UnityEngine;

namespace ReVR.Vehicles.Bike
{
    [AddComponentMenu("ReVR/Vehicles/Bike/Wheelie")]
    public class BikeWheelieFeature : BikeFeature
    {
        [FlexiableHeader("Wheelie", 18)]
        [Tooltip("Maximum wheelie angle of the bike.")]
        [SerializeField] private float m_MaxWheelieAngle = 30.0f;

        [Tooltip("Animation speed of wheelie, that will be used lerp from current rotation to target rotation.")]
        [SerializeField] private float m_WheelieAnimationSpeed = 3.0f;

        [Tooltip("<b>Adjusts the wheelie angle multiplier. X-axis represents normalized speed <i>(i.e, 0 means no speed, and 1 means max speed)</i>, Y-axis represents wheelie angle multiplier.</b> Example: Curve from (0, 0) to (0.1, 1) means nearly no wheelie at low speed, (0.1, 1) to (1, 1) means maximum wheelie at high speed. You will have to adjust it with multiple points to fine tune the wheelie.")]
        [SerializeField] private AnimationCurve m_WheelieCurve = new(new(0.0f, 0.0f, 10.0f, 10.0f, 0.0f, 0.3333333f), new(0.1f, 1.0f, 0.003699792f, 0.003699792f, 0.2721024f, 0.02350461f), new(1.0f, 1.0f, 0.0f, 0.0f, 0.3333333f, 0.0f));

        [Tooltip("The transform of the bike that is used to perform wheelie.")]
        [SerializeField, RequiredField] private Transform m_WheelieTransform;


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
            WheelieAnimation();
        }

        public override void OnPhysicsUpdate()
        {

        }

        private void WheelieAnimation()
        {
            bool isInputsSatisfied = m_Blackboard.clutchInput && m_Blackboard.accelerateInput > 0.0f;

            if (isInputsSatisfied && m_Blackboard.isRearWheelGrounded && m_Blackboard.localVelocity.magnitude > 0.1f)
                m_Blackboard.isDoingWheelie = true;
            if (!isInputsSatisfied && m_Blackboard.isFrontWheelGrounded)
                m_Blackboard.isDoingWheelie = false;

            const float acceptableAngleForWheelie = 20.0f;
            float surfaceAngle = Vector3.Angle(m_Blackboard.groundNormal, Vector3.up);
            float surfaceAngleFront = Vector3.Angle(m_Blackboard.frontWheelGroundNormal, Vector3.up);
            float surfaceAngleRear = Vector3.Angle(m_Blackboard.rearWheelGroundNormal, Vector3.up);

            bool snapWheelie = false;

            if (surfaceAngle > acceptableAngleForWheelie || surfaceAngleFront > acceptableAngleForWheelie || surfaceAngleRear > acceptableAngleForWheelie)
            {
                if (m_Blackboard.isDoingWheelie)
                    snapWheelie = true;
                m_Blackboard.isDoingWheelie = false;
            }

            if (m_Blackboard.isDoingWheelie)
            {
                if (isInputsSatisfied)
                {
                    float speedRatio = Mathf.Clamp01(m_Blackboard.localVelocity.z / m_Blackboard.maxSpeed.value);

                    float targetAngle = -m_MaxWheelieAngle * m_WheelieCurve.Evaluate(speedRatio);
                    float currentAngle = m_WheelieTransform.localRotation.eulerAngles.x;
                    float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, m_WheelieAnimationSpeed * Time.deltaTime);
                    m_WheelieTransform.localRotation = Quaternion.Euler(newAngle, 0.0f, 0.0f);
                }
                else
                {
                    float targetAngle = 0.0f;
                    float currentAngle = m_WheelieTransform.localRotation.eulerAngles.x;
                    float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, m_WheelieAnimationSpeed * Time.deltaTime);
                    m_WheelieTransform.localRotation = Quaternion.Euler(newAngle, 0.0f, 0.0f);
                }
            }
            else
            {
                float targetAngle = 0.0f;
                float currentAngle = m_WheelieTransform.localRotation.eulerAngles.x;
                float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, m_WheelieAnimationSpeed * Time.deltaTime);
                if (snapWheelie)
                    newAngle = Mathf.LerpAngle(currentAngle, targetAngle, m_Blackboard.alignRotatorSpeedInGround.value * Time.deltaTime);

                m_WheelieTransform.localRotation = Quaternion.Euler(newAngle, 0.0f, 0.0f);
            }
        }

    }
}
