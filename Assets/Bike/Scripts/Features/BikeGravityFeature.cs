using UnityEngine;

namespace ReVR.Vehicles.Bike
{
    [AddComponentMenu("ReVR/Vehicles/Bike/Gravity")]
    public class BikeGravityFeature : BikeFeature
    {
        [FlexiableHeader("Environmental Dynamics", 18)]
        [Tooltip("Gravity force applied to the bike.")]
        [SerializeField] private float m_Gravity = -9.81f;


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

        }

        public override void OnPhysicsUpdate()
        {
            m_Blackboard.Rigidbody.AddForce(new(0.0f, m_Gravity, 0.0f), ForceMode.Acceleration);
        }
    }
}
