using UnityEngine;

namespace ReVR.Vehicles.Bike
{
    [AddComponentMenu("ReVR/Vehicles/Bike/Suspension")]
    public class BikeSuspensionFeature : BikeFeature
    {
        [System.Serializable]
        private struct WheelInfo
        {
            [Tooltip("Radius of the wheel.")]
            [Min(0.0f)] public float radius;

            [Tooltip("Angle between world up and wheel movement direction.")]
            [Min(0.0f)] public float angle;

            [Tooltip("Starting position of the wheel in local space, w.r.t suspension's parent.")]
            public Vector3 startingPositionLS;

            [Tooltip("Wheel rotation axis in local space, w.r.t suspension's parent.")]
            public Vector3 rotateAxisLS;

            [Tooltip("Transform of the wheel.")]
            [RequiredField] public SharedTransform transform;
        }

        [System.Serializable]
        private struct SuspensionInfo
        {
            [Tooltip("Max compression length of the suspension from its rest length.")]
            [Min(0.0f)] public float maxCompressionLength;

            [Tooltip("Movement direction of the suspension in local space, w.r.t suspension's parent.")]
            public Vector3 directionLS;

            [Tooltip("Transform of the suspension.")]
            [RequiredField] public Transform transform;

            [Tooltip("Parent transform of the suspension.")]
            [RequiredField] public Transform parent;
        }


        [FlexiableHeader("Wheel", 18)]
        [Tooltip("Front wheel informations.")]
        [SerializeField] private WheelInfo m_FrontWheel;

        [Tooltip("Rear wheel informations.")]
        [SerializeField] private WheelInfo m_RearWheel;


        [FlexiableHeader("Suspension", 18)]
        [Tooltip("The force exerted by the suspension springs. Higher values result in a stiffer suspension.")]
        [SerializeField] private float m_SpringForce = 100.0f;

        [Tooltip("The force exerted by the dampers to absorb shocks. Higher values result in less oscillation.")]
        [SerializeField] private float m_DamperForce = 10.0f;

        [Tooltip("A factor to adjust how well the bike sticks to the ground.")]
        [SerializeField] private float m_GroundStickFactor = 0.2f;

        [Tooltip("Maximum allowable compression for the suspension. Prevents the bike from bottoming out.")]
        [SerializeField] private SharedFloat m_MaxCompression = 0.5f;
        [Space]
        [Tooltip("Front suspension informations.")]
        [SerializeField] private SuspensionInfo m_FrontSuspension;

        [Tooltip("Rear suspension informations.")]
        [SerializeField] private SuspensionInfo m_RearSuspension;


        [FlexiableHeader("Mics", 18)]
        [Tooltip("Layers on which the bike can be drive and suspension can be applied.")]
        [SerializeField] private LayerMask m_DrivableLayerMask;


#if UNITY_EDITOR
        [FlexiableHeader("Editor Only", 18, TextAnchor.MiddleLeft, FontStyle.Bold, "F38181")]
        [Tooltip("Model of the bike with all of its parts in its children.")]
        [SerializeField] private Transform m_BikeModel;

        [Tooltip("Start point of the front suspension, from which the front suspension direction will be calculated.")]
        [SerializeField] private Transform m_FrontSuspensionPointFrom;

        [Tooltip("End point of the front suspension, till which the front suspension direction will get calculated.")]
        [SerializeField] private Transform m_FrontSuspensionPointTo;

        [Tooltip("Starting point of the rear suspension, from which the rear suspension direction will be calculated.")]
        [SerializeField] private Transform m_RearSuspensionPointFrom;

        [Tooltip("End point of the rear suspension, till which the rear suspension direction will get calculated.")]
        [SerializeField] private Transform m_RearSuspensionPointTo;
#endif


        private BikeBlackboard m_Blackboard;
        private float m_CompressionForSnap;
        private float m_SnapDistance;
        private float m_MaxFrontRayDistance;
        private float m_MaxRearRayDistance;
        private bool m_IsSnappedToGround;
        private Vector3 m_FrontSuspensionStartingLocalPosition;
        private Quaternion m_RearSuspensionStartingLocalRotation;


        public override void OnAwake(BikeBlackboard blackboard)
        {
            m_Blackboard = blackboard;

            m_Blackboard.frontWheel = m_FrontWheel.transform;
            m_Blackboard.rearWheel = m_RearWheel.transform;
            m_Blackboard.maxCompression = m_MaxCompression;

            m_FrontSuspensionStartingLocalPosition = m_FrontSuspension.transform.localPosition;
            m_RearSuspensionStartingLocalRotation = m_RearSuspension.transform.localRotation;

            // Calculate max raycast distance
            m_MaxFrontRayDistance = m_FrontWheel.radius / Mathf.Cos(m_FrontWheel.angle * Mathf.Deg2Rad) + m_FrontWheel.radius;
            m_MaxRearRayDistance = m_RearWheel.radius / Mathf.Cos(m_RearWheel.angle * Mathf.Deg2Rad) + m_RearWheel.radius;
        }

        public override void OnStart()
        {
            
        }

        public override void OnUpdate()
        {
            // Update wheel (ground contact and tire animation)
            PlaceFrontWheelOnGround();
            PlaceRearWheelOnGround();

            TireAnimation();
        }

        public override void OnPhysicsUpdate()
        {
            // Calculate suspension and Handle suspension
            CalculateSuspensionParameters();
            AddSuspension();
        }


        private void PlaceFrontWheelOnGround()
        {
            float wheelRadius = m_FrontWheel.radius;
            Vector3 wheelPositionWS = m_FrontSuspension.parent.TransformPoint(m_FrontWheel.startingPositionLS);
            Vector3 suspensionDirWS = m_FrontSuspension.parent.TransformDirection(m_FrontSuspension.directionLS);
            Ray ray = new(wheelPositionWS - suspensionDirWS * wheelRadius, suspensionDirWS);

            if (Physics.Raycast(ray, out RaycastHit hit, m_MaxFrontRayDistance, m_DrivableLayerMask, QueryTriggerInteraction.Ignore))
            {
                float height = wheelRadius / Mathf.Cos(m_FrontWheel.angle * Mathf.Deg2Rad);
                float localOffset = height + wheelRadius - hit.distance;

                localOffset = Mathf.Clamp(localOffset, 0.0f, m_FrontSuspension.maxCompressionLength);

                m_FrontSuspension.transform.localPosition = m_FrontSuspensionStartingLocalPosition - m_FrontSuspension.directionLS * localOffset;

                m_Blackboard.isFrontWheelGrounded = true;
                m_Blackboard.frontWheelGroundNormal = hit.normal;
            }
            else
            {
                m_FrontSuspension.transform.localPosition = Vector3.Lerp(m_FrontSuspension.transform.localPosition, m_FrontSuspensionStartingLocalPosition, 20.0f * Time.deltaTime);

                m_Blackboard.isFrontWheelGrounded = false;
                m_Blackboard.frontWheelGroundNormal = Vector3.up;
            }

            m_Blackboard.frontWheelHit = hit;
        }

        private void PlaceRearWheelOnGround()
        {
            float wheelRadius = m_RearWheel.radius;
            Vector3 wheelPositionWS = m_RearSuspension.parent.TransformPoint(m_RearWheel.startingPositionLS);
            Vector3 suspensionDirWS = m_RearSuspension.parent.TransformDirection(m_RearSuspension.directionLS);
            Ray ray = new(wheelPositionWS - suspensionDirWS * wheelRadius, suspensionDirWS);

            if (Physics.Raycast(ray, out RaycastHit hit, m_MaxRearRayDistance, m_DrivableLayerMask, QueryTriggerInteraction.Ignore))
            {
                float height = wheelRadius / Mathf.Cos(m_RearWheel.angle * Mathf.Deg2Rad);
                float localOffset = height + wheelRadius - hit.distance;

                localOffset = Mathf.Clamp(localOffset, 0.0f, m_RearSuspension.maxCompressionLength);

                Vector3 localPosition = m_RearWheel.startingPositionLS - m_RearSuspension.directionLS * localOffset;
                Vector3 fromDirection = (m_RearWheel.startingPositionLS - m_RearSuspension.transform.localPosition).normalized;
                Vector3 toDirection = (localPosition - m_RearSuspension.transform.localPosition).normalized;

                m_RearSuspension.transform.localRotation = m_RearSuspensionStartingLocalRotation * Quaternion.FromToRotation(fromDirection, toDirection);

                m_Blackboard.isRearWheelGrounded = true;
                m_Blackboard.rearWheelGroundNormal = hit.normal;
            }
            else
            {
                m_RearSuspension.transform.localRotation = Quaternion.Slerp(m_RearSuspension.transform.localRotation, m_RearSuspensionStartingLocalRotation, 20.0f * Time.deltaTime);

                m_Blackboard.isRearWheelGrounded = false;
                m_Blackboard.rearWheelGroundNormal = Vector3.up;
            }

            m_Blackboard.rearWheelHit = hit;
        }

        private void TireAnimation()
        {
            // Front wheel
            float frontRotation = m_Blackboard.isApplyingBrake ? 0.0f : (m_Blackboard.localVelocity.z / m_FrontWheel.radius) * Mathf.Rad2Deg * Time.deltaTime;
            Vector3 frontWheelAxis = m_FrontSuspension.transform.TransformDirection(m_FrontWheel.rotateAxisLS);
            m_FrontWheel.transform.value.RotateAround(m_FrontWheel.transform.value.position, frontWheelAxis, frontRotation);

            // Rear wheel
            float rearRotation = (m_Blackboard.isApplyingHandBrake || m_Blackboard.isApplyingBrake) ? 0.0f : (m_Blackboard.localVelocity.z / m_RearWheel.radius) * Mathf.Rad2Deg * Time.deltaTime;

            if (m_Blackboard.isDoingBurnout && Mathf.Abs(m_Blackboard.localVelocity.z) < 1.0f)
            {
                rearRotation = (m_Blackboard.maxSpeed.value / m_RearWheel.radius) * Mathf.Rad2Deg * Time.deltaTime;
            }

            Vector3 rearWheelAxis = m_RearSuspension.transform.TransformDirection(m_RearWheel.rotateAxisLS);
            m_RearWheel.transform.value.RotateAround(m_RearWheel.transform.value.position, rearWheelAxis, rearRotation);
        }

        private void CalculateSuspensionParameters()
        {
            // Suspension compression parameters
            float frontWheelCompression = Mathf.Clamp01(m_Blackboard.isFrontWheelGrounded ? (m_MaxFrontRayDistance - m_Blackboard.frontWheelHit.distance) / m_MaxFrontRayDistance : 0.0f);
            float rearWheelCompression = Mathf.Clamp01(m_Blackboard.isRearWheelGrounded ? (m_MaxRearRayDistance - m_Blackboard.rearWheelHit.distance) / m_MaxRearRayDistance : 0.0f);
            m_Blackboard.totalCompression = Mathf.Clamp01((frontWheelCompression + rearWheelCompression) * 0.5f);

            // Bike snap to ground parameters
            float frontCosTheta = Mathf.Cos(m_FrontWheel.angle);
            float rearCosTheta = Mathf.Cos(m_RearWheel.angle);
            float mard = ((m_MaxFrontRayDistance * frontCosTheta) + (m_MaxRearRayDistance * rearCosTheta)) * 0.5f;

            m_CompressionForSnap = ((frontWheelCompression * frontCosTheta) + (rearWheelCompression * rearCosTheta)) * 0.5f;
            m_SnapDistance = mard * (m_CompressionForSnap - m_MaxCompression.value);
        }

        private void AddSuspension()
        {
            Vector3 normal = Vector3.up;

            if (m_Blackboard.isFrontWheelGrounded && m_Blackboard.isRearWheelGrounded)
                normal = m_Blackboard.groundNormal;
            else if (m_Blackboard.isRearWheelGrounded)
                normal = m_Blackboard.rearWheelHit.normal;

            Vector3 frontWheelPositionWS = m_FrontSuspension.parent.TransformPoint(m_FrontWheel.startingPositionLS);
            Vector3 rearWheelPositionWS = m_RearSuspension.parent.TransformPoint(m_RearWheel.startingPositionLS);

            Vector3 sidePlaneNormal = Vector3.Cross((frontWheelPositionWS - rearWheelPositionWS).normalized, (Vector3.up * Vector3.Dot(m_Blackboard.RotatorTransform.up, transform.up)).normalized).normalized;
            Vector3 suspensionNormal = Vector3.ProjectOnPlane(normal, sidePlaneNormal);
            Vector3 springDir = suspensionNormal.normalized;

#if UNITY_6000_0_OR_NEWER
            Vector3 linearVelocity = m_Blackboard.Rigidbody.linearVelocity;
#else
            Vector3 linearVelocity = m_Blackboard.Rigidbody.velocity;
#endif

            float springVelocity = Vector3.Dot(linearVelocity, springDir);
            float acceleration = Mathf.Abs(springVelocity) / Time.fixedDeltaTime;
            float springForce = m_SpringForce * (m_Blackboard.totalCompression - m_GroundStickFactor * m_Blackboard.totalCompression);
            float damperForce = Mathf.Clamp(m_DamperForce * springVelocity, -acceleration, acceleration);

            if (m_Blackboard.totalCompression < 0.05f)
                damperForce = 0.0f;

            Vector3 suspensionForce = springDir * (springForce - damperForce);

            if (m_Blackboard.isGrounded)
            {
                // Snap to ground
                if (m_CompressionForSnap > m_MaxCompression.value)
                {
                    if (!m_IsSnappedToGround)
                    {
                        m_IsSnappedToGround = true;

                        Vector3 snapMovePosition = m_Blackboard.Rigidbody.position + (m_Blackboard.projectedUp * m_SnapDistance);
                        m_Blackboard.Rigidbody.MovePosition(snapMovePosition);
                        ReduceSurfaceNormalDownVelocity(0.8f);
                    }
                }
                else
                {
                    m_IsSnappedToGround = false;
                }

                float leanAngle = Mathf.Abs(m_Blackboard.LeanTransform.localEulerAngles.z);
                float leanFactor = Mathf.Cos(leanAngle * Mathf.Deg2Rad);
                m_Blackboard.Rigidbody.AddForce(suspensionForce * leanFactor, ForceMode.Acceleration);
            }
        }

        private void ReduceSurfaceNormalDownVelocity(float factor)
        {

#if UNITY_6000_0_OR_NEWER
            Vector3 linearVelocity = m_Blackboard.Rigidbody.linearVelocity;
#else
            Vector3 linearVelocity = m_Blackboard.Rigidbody.velocity;
#endif

            Vector3 currentVelocityInSurfaceNormal = Vector3.Project(linearVelocity, m_Blackboard.projectedUp);
            linearVelocity -= currentVelocityInSurfaceNormal * factor;

#if UNITY_6000_0_OR_NEWER
            m_Blackboard.Rigidbody.linearVelocity = linearVelocity;
#else
            m_Blackboard.Rigidbody.velocity = linearVelocity;
#endif

        }


#if UNITY_EDITOR

        [ContextMenu("Calculate Wheel And Suspension Informations")]
        private void CalculateWheelAndSuspensionInfo()
        {
            InitFrontWheelAndSuspension();
            InitRearWheelAndSuspension();
        }

        [ContextMenu("Init Front Wheel And Suspension"), ShowAsButtonInInspector]
        private void InitFrontWheelAndSuspension()
        {
            bool isValidated = true;

            if (m_FrontWheel.transform.value == null)
            {
                isValidated = false;
                Debug.LogError("[VALIDATION FAILED] Front Wheel.Transform is NULL. To initialize front wheel & front suspension information Front Wheel.Transform is required.");
            }

            if (m_FrontSuspension.transform == null)
            {
                isValidated = false;
                Debug.LogError("[VALIDATION FAILED] Front Suspension.Transform is NULL. To initialize front wheel & front suspension information Front Suspension.Transform is required.");
            }

            if (m_FrontSuspension.parent == null)
            {
                isValidated = false;
                Debug.LogError("[VALIDATION FAILED] Front Suspension.Parent is NULL. To initialize front wheel & front suspension information Front Suspension.Parent is required.");
            }

            if (m_FrontSuspensionPointFrom == null)
            {
                isValidated = false;
                Debug.LogError("[VALIDATION FAILED] Front Suspension Point From is NULL. To initialize front wheel & front suspension information Front Suspension Point From is required.");
            }

            if (m_FrontSuspensionPointTo == null)
            {
                isValidated = false;
                Debug.LogError("[VALIDATION FAILED] Front Suspension Point To is NULL. To initialize front wheel & front suspension information Front Suspension Point To is required.");
            }

            if (m_BikeModel == null)
            {
                isValidated = false;
                Debug.LogError("[VALIDATION FAILED] Bike Model is NULL. To initialize front wheel & front suspension information Bike Model is required.");
            }

            if (!isValidated)
                return;

            m_BikeModel.GetPositionAndRotation(out Vector3 currentPosition, out Quaternion currentRotation);
            m_BikeModel.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

            currentPosition = (currentPosition == Vector3.zero) ? Vector3.zero : currentPosition;
            currentRotation = (currentRotation == Quaternion.identity) ? Quaternion.identity : currentRotation;

            Vector3 suspensionDirWS = (m_FrontSuspensionPointTo.position - m_FrontSuspensionPointFrom.position).normalized;

            m_FrontWheel.angle = Vector3.Angle(Vector3.up, Mathf.Sign(Vector3.Dot(Vector3.up, suspensionDirWS)) * suspensionDirWS);
            m_FrontWheel.startingPositionLS = m_FrontSuspension.parent.InverseTransformPoint(m_FrontWheel.transform.value.position);
            m_FrontSuspension.directionLS = m_FrontSuspension.parent.InverseTransformDirection(suspensionDirWS);
            m_FrontWheel.rotateAxisLS = m_FrontSuspension.transform.InverseTransformDirection(Vector3.right);

            m_BikeModel.SetPositionAndRotation(currentPosition, currentRotation);
        }

        [ContextMenu("Init Rear Wheel And Suspension"), ShowAsButtonInInspector]
        private void InitRearWheelAndSuspension()
        {
            bool isValidated = true;

            if (m_RearWheel.transform.value == null)
            {
                isValidated = false;
                Debug.LogError("[VALIDATION FAILED] Rear Wheel.Transform is NULL. To initialize rear wheel & rear suspension information Rear Wheel.Transform is required.");
            }

            if (m_RearSuspension.transform == null)
            {
                isValidated = false;
                Debug.LogError("[VALIDATION FAILED] Rear Suspension.Transform is NULL. To initialize rear wheel & rear suspension information Rear Suspension.Transform is required.");
            }

            if (m_RearSuspension.parent == null)
            {
                isValidated = false;
                Debug.LogError("[VALIDATION FAILED] Rear Suspension.Parent is NULL. To initialize rear wheel & rear suspension information Rear Suspension.Parent is required.");
            }

            if (m_RearSuspensionPointFrom == null)
            {
                isValidated = false;
                Debug.LogError("[VALIDATION FAILED] Rear Suspension Point From is NULL. To initialize rear wheel & rear suspension information Rear Suspension Point From is required.");
            }

            if (m_RearSuspensionPointTo == null)
            {
                isValidated = false;
                Debug.LogError("[VALIDATION FAILED] Rear Suspension Point To is NULL. To initialize rear wheel & rear suspension information Rear Suspension Point To is required.");
            }

            if (m_BikeModel == null)
            {
                isValidated = false;
                Debug.LogError("[VALIDATION FAILED] Bike Model is NULL. To initialize rear wheel & rear suspension information Bike Model is required.");
            }

            if (!isValidated)
                return;

            m_BikeModel.GetPositionAndRotation(out Vector3 currentPosition, out Quaternion currentRotation);
            m_BikeModel.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

            currentPosition = (currentPosition == Vector3.zero) ? Vector3.zero : currentPosition;
            currentRotation = (currentRotation == Quaternion.identity) ? Quaternion.identity : currentRotation;

            Vector3 suspensionDirWS = (m_RearSuspensionPointTo.position - m_RearSuspensionPointFrom.position).normalized;

            m_RearWheel.angle = Vector3.Angle(Vector3.up, Mathf.Sign(Vector3.Dot(Vector3.up, suspensionDirWS)) * suspensionDirWS);
            m_RearWheel.startingPositionLS = m_RearSuspension.parent.InverseTransformPoint(m_RearWheel.transform.value.position);
            m_RearSuspension.directionLS = m_RearSuspension.parent.InverseTransformDirection(suspensionDirWS);
            m_RearWheel.rotateAxisLS = m_RearSuspension.transform.InverseTransformDirection(Vector3.right);

            m_BikeModel.SetPositionAndRotation(currentPosition, currentRotation);
        }

        private void OnDrawGizmos()
        {
            bool isFrontWheelGrounded = false;
            Vector3 frontWheelHitPoint = Vector3.zero;
            bool isRearWheelGrounded = false;
            Vector3 rearWheelHitPoint = Vector3.zero;

            if (m_Blackboard != null)
            {
                isFrontWheelGrounded = m_Blackboard.isFrontWheelGrounded;
                frontWheelHitPoint = m_Blackboard.frontWheelHit.point;
                isRearWheelGrounded = m_Blackboard.isRearWheelGrounded;
                rearWheelHitPoint = m_Blackboard.rearWheelHit.point;
            }

            if (m_FrontSuspension.transform != null)
            {
                Transform suspension = m_FrontSuspension.transform;

                if (m_FrontWheel.transform.value != null)
                {
                    Transform wheel = m_FrontWheel.transform.value;
                    Vector3 rotateAxis = suspension.TransformDirection(m_FrontWheel.rotateAxisLS);

                    Gizmos.color = Color.yellow;
                    Gizmos.DrawRay(wheel.position, rotateAxis * 0.3f);

                    Gizmos.color = isFrontWheelGrounded ? Color.green : Color.red;
                    GizmosUtility.DrawCylinder(wheel.position, rotateAxis, m_FrontWheel.radius, 0.12f, 24);
                }

                if (m_FrontSuspension.parent != null)
                {
                    Vector3 suspensionDirectionWS = m_FrontSuspension.parent.TransformDirection(m_FrontSuspension.directionLS);
                    Vector3 wheelPositionWS = m_FrontSuspension.parent.TransformPoint(m_FrontWheel.startingPositionLS);
                    Vector3 rayOriginWS = wheelPositionWS - suspensionDirectionWS * m_FrontWheel.radius;

                    float maxRayDistance = m_FrontWheel.radius / Mathf.Cos(m_FrontWheel.angle * Mathf.Deg2Rad) + m_FrontWheel.radius;

                    Gizmos.color = isFrontWheelGrounded ? Color.green : Color.red;
                    Gizmos.DrawRay(rayOriginWS, suspensionDirectionWS * maxRayDistance);
                    Gizmos.DrawSphere(rayOriginWS, 0.01f);
                }
            }

            if (m_FrontWheel.transform.value != null && isFrontWheelGrounded)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(frontWheelHitPoint, 0.01f);
            }
            

            if (m_RearSuspension.transform != null)
            {
                Transform suspension = m_RearSuspension.transform;

                if (m_RearWheel.transform.value != null)
                {
                    Transform wheel = m_RearWheel.transform.value;
                    Vector3 rotateAxis = suspension.TransformDirection(m_RearWheel.rotateAxisLS);

                    Gizmos.color = Color.yellow;
                    Gizmos.DrawRay(wheel.position, rotateAxis * 0.3f);

                    Gizmos.color = isRearWheelGrounded ? Color.green : Color.red;
                    GizmosUtility.DrawCylinder(wheel.position, rotateAxis, m_RearWheel.radius, 0.12f, 24);
                }

                if (m_RearSuspension.parent != null)
                {
                    Vector3 suspensionDirectionWS = m_RearSuspension.parent.TransformDirection(m_RearSuspension.directionLS);
                    Vector3 wheelPositionWS = m_RearSuspension.parent.TransformPoint(m_RearWheel.startingPositionLS);
                    Vector3 rayOriginWS = wheelPositionWS - suspensionDirectionWS * m_RearWheel.radius;

                    float maxRayDistance = m_RearWheel.radius / Mathf.Cos(m_RearWheel.angle * Mathf.Deg2Rad) + m_RearWheel.radius;

                    Gizmos.color = isRearWheelGrounded ? Color.green : Color.red;
                    Gizmos.DrawRay(rayOriginWS, suspensionDirectionWS * maxRayDistance);
                    Gizmos.DrawSphere(rayOriginWS, 0.01f);
                }
            }

            if (m_RearWheel.transform.value != null && isRearWheelGrounded)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(rearWheelHitPoint, 0.01f);
            }
        }

#endif

    }
}
