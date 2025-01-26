using UnityEngine;

namespace ReVR.Vehicles.Bike
{
    [AddComponentMenu("ReVR/Vehicles/Bike/Turn And Steer")]
    public class BikeTurnAndSteerFeature : BikeFeature
    {
        [FlexiableHeader("Turn", 18)]
        [Tooltip("Maximum angle the bike handle can turn.")]
        [SerializeField] private float m_MaxTurnAngle = 30.0f;

        [Tooltip("Whether to use smooth turning. Sometimes you don't want instant left to right turning. It delays how fast we can turn the bike by inputs.")]
        [SerializeField] private bool m_UseLerpTurning = true;

        [Tooltip("Speed of smooth turning if `useLerpTurning` is true.")]
        [SerializeField] private float m_TurnLerpSpeed = 4.0f;

        [Tooltip("Whether the bike can turn while in the air (not grounded).")]
        [SerializeField] private bool m_CanTurnInAir = true;

        [Tooltip("Speed of turning while in the air if `canTurnInAir` is true.")]
        [SerializeField] private float m_TurnSpeedInAir = 70.0f;


        [FlexiableHeader("Steer", 18)]
        [Tooltip("Speed of the steering animation.")]
        [SerializeField] private float m_SteeringAnimationSpeed = 4.0f;

        [Tooltip("Adjusts the steering sensitivity. X-axis represents speed, Y-axis represents steering sensitivity. Example: Curve from (0, 1) to (1, 0.3) means sensitive steering at low speed, decreasing sensitivity at high speed. You will have to adjust it with multiple points to fine tune the turning.")]
        [SerializeField] private AnimationCurve m_SteeringCurve = new(new(0, 1), new(1, 0.3f));

        [Tooltip("Steering rotation axis in local space, w.r.t steering parent.")]
        [SerializeField] private Vector3 m_SteeringRotateAxisLS;

        [Tooltip("The transform used for steering the bike.")]
        [SerializeField, RequiredField] private Transform m_Steering;

        [Tooltip("The parent transform for the bike steering.")]
        [SerializeField, RequiredField] private Transform m_SteeringParent;


        [FlexiableHeader("Drift", 18)]
        [Tooltip("Factor to adjust turning during drifts (when handbrake is applied).")]
        [SerializeField] private float m_DriftTurnFactor = 2.0f;

        [Tooltip("Factor to adjust friction during drifts (when handbrake is applied).")]
        [SerializeField] private float m_DriftFrictionFactor = 0.5f;


        [FlexiableHeader("Environmental Dynamics", 18)]
        [Tooltip("Coefficient for calculating friction force (sideways force).")]
        [SerializeField] private float m_FrictionCoefficient = 0.5f;

        [Tooltip("Adjusts friction based on speed. X-axis represents sideways speed, Y-axis represents friction multiplier. Example: Curve from (0, 1) to (1, 0.1) means high friction at low sideways speed (meaning when bike is not sliding sideways), decreasing to low friction at high sideways speed (meaning when bike is sliding sideways).")]
        [SerializeField] private AnimationCurve m_FrictionCurve = new(new(0, 1), new(0.1f, 0.5f), new(1, 0.1f));


#if UNITY_EDITOR
        [FlexiableHeader("Editor Only", 18, TextAnchor.MiddleLeft, FontStyle.Bold, "F38181")]
        [Tooltip("Model of the bike with all of its parts in its children.")]
        [SerializeField] private Transform m_BikeModel;

        [Tooltip("Start point of the steering, from which the steering direction will be calculated.")]
        [SerializeField] private Transform m_SteeringPointFrom;

        [Tooltip("End point of the steering, till which the steering direction will get calculated.")]
        [SerializeField] private Transform m_SteeringPointTo;
#endif


        private BikeBlackboard m_Blackboard;
        private Quaternion m_StartingLocalRotation;
        private float m_WheelBase;
        private float m_SteerSmoother;


        public override void OnAwake(BikeBlackboard blackboard)
        {
            m_Blackboard = blackboard;

            m_StartingLocalRotation = m_Steering.localRotation;
        }

        public override void OnStart()
        {
            m_WheelBase = Vector3.Distance(m_Blackboard.frontWheel.value.position, m_Blackboard.rearWheel.value.position);
        }

        public override void OnUpdate()
        {
            float steeringDirection = m_Blackboard.steerInput;
            SteeringAnimation(steeringDirection);
            AddTurning(steeringDirection);
        }

        public override void OnPhysicsUpdate()
        {
            ApplyFriction();
        }

        private void SteeringAnimation(float steeringDirection)
        {
            float speedRatio = Mathf.Abs(m_Blackboard.localVelocity.z) / m_Blackboard.maxSpeed.value;
            float steeringAngle = m_MaxTurnAngle * m_SteeringCurve.Evaluate(speedRatio);
            m_Blackboard.currentSteerAngle = steeringAngle;

            if (m_Blackboard.handBrakeInput)
                steeringAngle = -m_MaxTurnAngle;

            //Quaternion targetRotation = Quaternion.Euler(0.0f, steeringAngle * steeringDirection, 0.0f);
            //Quaternion targetRotation = Quaternion.Euler(steeringAngle * steeringDirection, 0.0f, -153.039f);

            Quaternion rotationAroundSteeringAxis = Quaternion.AngleAxis(steeringAngle * steeringDirection, m_SteeringRotateAxisLS);
            Quaternion targetRotation = rotationAroundSteeringAxis * m_StartingLocalRotation;

            m_Steering.localRotation = Quaternion.Slerp(m_Steering.localRotation, targetRotation, m_SteeringAnimationSpeed * Time.deltaTime);
        }

        private void AddTurning(float steeringDirection)
        {
            if (m_UseLerpTurning)
            {
                m_SteerSmoother = Mathf.MoveTowards(m_SteerSmoother, steeringDirection, m_TurnLerpSpeed * Time.deltaTime);
            }
            else
            {
                m_SteerSmoother = steeringDirection;
            }

            if (m_Blackboard.isGrounded)
            {
                float driftTurningFactorCurrent = m_Blackboard.handBrakeInput ? m_DriftTurnFactor : 1.0f;
                float steeringAngleInRadians = m_SteerSmoother * Mathf.Deg2Rad * m_Blackboard.currentSteerAngle;
                
                if (Mathf.Abs(steeringAngleInRadians) < 0.01f)
                    return;
                
                float turningRadius = m_WheelBase / Mathf.Tan(steeringAngleInRadians);
                float angularVelocity = m_Blackboard.localVelocity.z / turningRadius;
                float rotationAmount = angularVelocity * Time.deltaTime;

                m_Blackboard.RotatorTransform.Rotate(m_Blackboard.projectedUp, rotationAmount * Mathf.Rad2Deg * driftTurningFactorCurrent, Space.World);
            }
            else if (m_CanTurnInAir)
            {
                m_Blackboard.RotatorTransform.Rotate(Vector3.up, m_TurnSpeedInAir * m_SteerSmoother * Time.deltaTime, Space.World);
            }
        }

        private void ApplyFriction()
        {
            if (!m_Blackboard.isGrounded || m_Blackboard.isBurnoutRotating)
                return;

            float driftFrictionFactorCurrent = m_Blackboard.handBrakeInput ? m_DriftFrictionFactor : 1.0f;

            float sideVelocityRatio = Mathf.Abs(m_Blackboard.localVelocity.x / m_Blackboard.maxSpeed.value);
            float frictionForce = (-m_Blackboard.localVelocity.x * m_FrictionCoefficient / Time.fixedDeltaTime) * m_FrictionCurve.Evaluate(sideVelocityRatio);

            Vector3 totalFrictionForce = m_Blackboard.RotatorTransform.right * frictionForce * driftFrictionFactorCurrent;

            m_Blackboard.Rigidbody.AddForceAtPosition(totalFrictionForce, transform.position, ForceMode.Acceleration);
        }

#if UNITY_EDITOR
        [ContextMenu("Init Steering Axis"), ShowAsButtonInInspector]
        private void InitSteeringAxis()
        {
            bool isValidated = true;

            if (m_SteeringParent == null)
            {
                isValidated = false;
                Debug.LogError("[VALIDATION FAILED] Steering Parent is NULL. To initialize steering axis Steering Parent is required.");
            }

            if (m_SteeringPointFrom == null)
            {
                isValidated = false;
                Debug.LogError("[VALIDATION FAILED] Steering Point From is NULL. To initialize steering axis Steering Point From is required.");
            }

            if (m_SteeringPointTo == null)
            {
                isValidated = false;
                Debug.LogError("[VALIDATION FAILED] Steering Point To is NULL. To initialize steering axis Steering Point To is required.");
            }

            if (m_BikeModel == null)
            {
                isValidated = false;
                Debug.LogError("[VALIDATION FAILED] Bike Model is NULL. To initialize steering axis Bike Model is required.");
            }

            if (!isValidated)
                return;

            m_BikeModel.GetPositionAndRotation(out Vector3 currentPosition, out Quaternion currentRotation);
            m_BikeModel.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

            currentPosition = (currentPosition == Vector3.zero) ? Vector3.zero : currentPosition;
            currentRotation = (currentRotation == Quaternion.identity) ? Quaternion.identity : currentRotation;

            Vector3 steeringDirectionWS = (m_SteeringPointTo.position - m_SteeringPointFrom.position).normalized;

            m_SteeringRotateAxisLS = m_SteeringParent.InverseTransformDirection(steeringDirectionWS);

            m_BikeModel.SetPositionAndRotation(currentPosition, currentRotation);
        }

        private void OnDrawGizmos()
        {
            if (m_Steering != null && m_SteeringParent != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(m_Steering.position, m_SteeringParent.TransformDirection(m_SteeringRotateAxisLS) * 0.3f);
            }
        }
#endif

    }
}
