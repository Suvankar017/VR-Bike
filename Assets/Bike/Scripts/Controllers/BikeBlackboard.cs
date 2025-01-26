using UnityEngine;

namespace ReVR.Vehicles.Bike
{
    public class BikeBlackboard
    {

        private readonly Rigidbody m_Rigidbody;


        public RaycastHit frontWheelHit;
        public RaycastHit rearWheelHit;


        public Vector3 localVelocity;
        public Vector3 groundNormal;
        public Vector3 frontWheelGroundNormal;
        public Vector3 rearWheelGroundNormal;
        public Vector3 projectedForward;
        public Vector3 projectedUp;


        #region Shared Variables

        // Wheel
        public SharedTransform frontWheel;
        public SharedTransform rearWheel;

        // Suspension
        public SharedFloat maxCompression;

        // Accelerate & Reverse
        public SharedFloat maxSpeed;
        public SharedFloat deceleration;

        // Align & Rotator
        public SharedTransform rotator;
        public SharedFloat alignRotatorSpeedInGround;

        // Lean
        public SharedTransform leanTransform;

        #endregion


        public float totalCompression;
        public float currentSteerAngle;
        public float currentLeanAngle;
        public float currentMaxSpeed;


        public bool isFrontWheelGrounded;
        public bool isRearWheelGrounded;
        public bool isGrounded;
        public bool wasGrounded;
        public bool canAccelerate;
        public bool isApplyingBrake;
        public bool isApplyingHandBrake;
        public bool isDoingBurnout;
        public bool isDoingWheelie;
        public bool isDoingStoppie;
        public bool isBurnoutRotating;


        #region Inputs

        public float accelerateInput;
        public float reverseInput;
        public float steerInput;
        public float gearInput;
        public bool handBrakeInput;
        public bool clutchInput;

        #endregion


        public Rigidbody Rigidbody => m_Rigidbody;
        public Transform RotatorTransform => rotator.value;
        public Transform LeanTransform => leanTransform.value;


        public BikeBlackboard(Rigidbody rigidbody)
        {
            m_Rigidbody = rigidbody;
        }

    }
}
