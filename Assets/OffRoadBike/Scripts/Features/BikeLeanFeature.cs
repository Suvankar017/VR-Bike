using UnityEngine;

namespace ReVR.Vehicles.Bike
{
    [AddComponentMenu("ReVR/Vehicles/Bike/Lean")]
    public class BikeLeanFeature : BikeFeature
    {
        [FlexiableHeader("Lean", 18)]
        [Tooltip("Maximum lean angle for the bike.")]
        [SerializeField] private float m_MaxLeanAngle = 45.0f;

        [Tooltip("Whether the bike can lean while in the air (not grounded).")]
        [SerializeField] private bool m_CanLeanInAir = true;

        [Tooltip("Speed of the leaning animation.")]
        [SerializeField] private float m_LeaningAnimationSpeed = 4.0f;

        [Tooltip("Adjusts the lean angle based on speed. X-axis represents speed, Y-axis represents lean angle multiplier. Example: Curve from (0, 0.2) to (1, 1) means minimal lean at low speed, increasing to maximum lean at high speed.")]
        [SerializeField] private AnimationCurve m_LeanCurve = new(new(0, 0.2f), new(1, 1));

        [Tooltip("Maximum lean local position for the bike.")]
        [SerializeField] private Vector3 m_MaxLeanPosition = Vector3.right;

        [Tooltip("The transform used for leaning the bike during turns.")]
        [SerializeField, RequiredField] private SharedTransform m_LeanTransform;


        private BikeBlackboard m_Blackboard;
        private Vector3 m_StartingLocalPosition;
        private float m_LeanSmoother;


        public override void OnAwake(BikeBlackboard blackboard)
        {
            m_Blackboard = blackboard;
            m_Blackboard.leanTransform = m_LeanTransform;

            m_StartingLocalPosition = m_LeanTransform.value.localPosition;
        }

        public override void OnStart()
        {

        }

        public override void OnUpdate()
        {
            float steeringDirection = m_Blackboard.steerInput;
            if (!Mathf.Approximately(steeringDirection, 0.0f))
            {
                if (m_Blackboard.isGrounded || m_CanLeanInAir)
                {
                    LeaningAnimation(steeringDirection);
                }
            }
            else
            {
                LeaningAnimation(0.0f);
            }
        }

        public override void OnPhysicsUpdate()
        {

        }

        private void LeaningAnimation(float steeringDirection)
        {
            m_LeanSmoother = Mathf.MoveTowards(m_LeanSmoother, steeringDirection, 10.0f * Time.deltaTime);

            float leanCurveMultiplier = m_LeanCurve.Evaluate(m_Blackboard.localVelocity.magnitude / m_Blackboard.maxSpeed.value);

            float leanAngle = -m_MaxLeanAngle * leanCurveMultiplier * Mathf.Sign(m_LeanSmoother);
            Quaternion targetRotation = Quaternion.Euler(0.0f, 0.0f, leanAngle * Mathf.Abs(m_LeanSmoother));

            m_LeanTransform.value.localRotation = Quaternion.Slerp(m_LeanTransform.value.localRotation, targetRotation, m_LeaningAnimationSpeed * Time.deltaTime);

            m_Blackboard.currentLeanAngle = leanAngle * Mathf.Abs(m_LeanSmoother);

            if (!Mathf.Approximately(m_MaxLeanAngle, 0.0f))
            {
                float currentAngle = m_LeanTransform.value.localRotation.eulerAngles.z;
                float signedLeanRatio = Mathf.Sign(180.0f - currentAngle) * (180.0f - Mathf.Abs(180.0f - currentAngle)) / m_MaxLeanAngle;
                Vector3 localOffset = m_MaxLeanPosition * signedLeanRatio;

                m_LeanTransform.value.localPosition = m_StartingLocalPosition + localOffset;
            }
        }
    }
}
