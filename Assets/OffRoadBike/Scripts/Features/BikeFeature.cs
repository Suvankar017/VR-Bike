using UnityEngine;

namespace ReVR.Vehicles.Bike
{
    public abstract class BikeFeature : MonoBehaviour
    {
        protected virtual void Awake() { }
        protected virtual void Start() { }

        public abstract void OnAwake(BikeBlackboard blackboard);
        public abstract void OnStart();
        public abstract void OnUpdate();
        public abstract void OnPhysicsUpdate();
    }
}
