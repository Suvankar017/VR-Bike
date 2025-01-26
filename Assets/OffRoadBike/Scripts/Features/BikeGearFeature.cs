namespace ReVR.Vehicles.Bike
{
    public abstract class BikeGearFeature : BikeFeature
    {
        protected BikeBlackboard m_Blackboard;

        public override void OnAwake(BikeBlackboard blackboard)
        {
            m_Blackboard = blackboard;
        }

        public override abstract void OnStart();
        public override abstract void OnPhysicsUpdate();
        public override abstract void OnUpdate();
    }
}
