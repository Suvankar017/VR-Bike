using System.Collections.Generic;
using UnityEngine;

namespace ReVR.Vehicles.Bike
{
    [AddComponentMenu("ReVR/Vehicles/Bike/Bike Controller")]
    [RequireComponent(typeof(Rigidbody))]
    public class BikeController : MonoBehaviour
    {
        [FlexiableHeader("Sequences", 18)]
        [SerializeField] private BikeFeature[] m_StartSequence;
        [SerializeField] private BikeFeature[] m_UpdateSequence;
        [SerializeField] private BikeFeature[] m_PhysicsUpdateSequence;

        private Rigidbody m_Rigidbody;
        private BikeBlackboard m_Blackboard;

        public float Speed
        {
            get
            {
                if (m_Rigidbody == null)
                    return 0.0f;

#if UNITY_6000_0_OR_NEWER
            Vector3 linearVelocity = m_Rigidbody.linearVelocity;
#else
                Vector3 linearVelocity = m_Rigidbody.velocity;
#endif

                return linearVelocity.magnitude;
            }
        }
        public float TopSpeed
        {
            get
            {
                if (m_Blackboard == null || m_Blackboard.maxSpeed == null)
                    return 0.0f;

                return m_Blackboard.maxSpeed.value;
            }
        }
        public float CurrentMaxSpeed
        {
            get
            {
                if (m_Blackboard == null)
                    return 0.0f;

                return m_Blackboard.currentMaxSpeed;
            }
        }
        public BikeBlackboard Blackboard => m_Blackboard;

        public void SetInput(BikeInput input)
        {
            m_Blackboard.accelerateInput = Mathf.Clamp01(input.accelerate);
            m_Blackboard.reverseInput = Mathf.Clamp01(input.reverse);
            m_Blackboard.steerInput = Mathf.Clamp(input.steer, -1.0f, 1.0f);
            m_Blackboard.gearInput = Mathf.Clamp(input.gear, -1.0f, 1.0f);
            m_Blackboard.handBrakeInput = input.handBrake;
            m_Blackboard.clutchInput = input.clutch;
        }

        #region Unity Methods

        private void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Rigidbody.useGravity = false;
            m_Blackboard = new(m_Rigidbody);

            List<BikeFeature> features = new();

            GetAllUniqueFeatures(m_StartSequence, features);
            GetAllUniqueFeatures(m_UpdateSequence, features);
            GetAllUniqueFeatures(m_PhysicsUpdateSequence, features);

            foreach (BikeFeature feature in features)
            {
                feature.OnAwake(m_Blackboard);
            }
        }

        private void Start()
        {
            m_Blackboard.canAccelerate = true;

            m_Blackboard.Rigidbody.useGravity = false;

            foreach (BikeFeature feature in m_StartSequence)
            {
                if (feature != null && feature.enabled)
                    feature.OnStart();
            }
        }

        private void Update()
        {
            foreach (BikeFeature feature in m_UpdateSequence)
            {
                if (feature != null && feature.enabled)
                    feature.OnUpdate();
            }
        }

        private void FixedUpdate()
        {
            // Convert world space velocity to local space (i.e rotator's space)
#if UNITY_6000_0_OR_NEWER
            Vector3 linearVelocity = m_Rigidbody.linearVelocity;
#else
            Vector3 linearVelocity = m_Rigidbody.velocity;
#endif

            m_Blackboard.localVelocity = m_Blackboard.RotatorTransform.InverseTransformDirection(linearVelocity);

            foreach (BikeFeature feature in m_PhysicsUpdateSequence)
            {
                if (feature != null && feature.enabled)
                    feature.OnPhysicsUpdate();
            }
        }

        #endregion

        private void GetAllUniqueFeatures(IEnumerable<BikeFeature> sequences, List<BikeFeature> uniqueFeatures)
        {
            foreach (BikeFeature feature in sequences)
            {
                if (feature != null && !uniqueFeatures.Contains(feature))
                    uniqueFeatures.Add(feature);
            }
        }

    }
}
