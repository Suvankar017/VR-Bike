namespace ReVR.Vehicles.Bike
{
    public struct BikeInput
    {
        /// <summary>
        /// Input for acceleration.
        /// <para>Range [0.0, 1.0], if the value is outside of the range then it get clamped between the range.</para>
        /// <para>0.0 -> No acceleration.</para>
        /// <para>1.0 -> Max acceleration.</para>
        /// </summary>
        public float accelerate;

        /// <summary>
        /// Input for reverse acceleration.
        /// <para>Range [0.0, 1.0], if the value is outside of the range then it get clamped between the range.</para>
        /// <para>0.0 -> No reverse acceleration.</para>
        /// <para>1.0 -> Max reverse acceleration.</para>
        /// </summary>
        public float reverse;

        /// <summary>
        /// Input for steering.
        /// <para>Range [-1.0, 1.0], if the value is outside of the range then it get clamped between the range.</para>
        /// <para>-1.0 -> Max left steering.</para>
        /// <para>0.0 -> No steering.</para>
        /// <para>1.0 -> Max right steering.</para>
        /// </summary>
        public float steer;

        /// <summary>
        /// Input for changing gear, in manual gear system.
        /// <para>Range [-1.0, 1.0], if the value is outside of the range then it get clamped between the range.</para>
        /// <para>Negative value -> Down gearing</para>
        /// <para>Positive value -> Up gearing</para>
        /// </summary>
        public float gear;

        /// <summary>
        /// Input for hand brake.
        /// <para>False -> Not applying hand brake.</para>
        /// <para>True -> Applying hand brake.</para>
        /// </summary>
        public bool handBrake;

        /// <summary>
        /// Input for clutch.
        /// <para>False -> Not applying clutch.</para>
        /// <para>True -> Applying hand clutch.</para>
        /// </summary>
        public bool clutch;
    }
}
