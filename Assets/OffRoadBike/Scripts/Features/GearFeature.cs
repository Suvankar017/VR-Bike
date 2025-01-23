namespace ReVR.Vehicles.Bike
{
    public abstract class GearFeature : BikeFeature
    {
        protected BikeBlackboard m_Blackboard;

        public override void OnAwake(BikeBlackboard blackboard)
        {
            m_Blackboard = blackboard;
        }

        public override void OnStart()
        {

        }

        public override void OnPhysicsUpdate()
        {

        }

        public override void OnUpdate()
        {

        }
    }
}
