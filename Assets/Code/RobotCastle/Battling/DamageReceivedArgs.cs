namespace RobotCastle.Battling
{
    public struct DamageReceivedArgs
    {
        public float amountReceived;
        public bool diedAfter;
        public bool wasBlocked;

        public DamageReceivedArgs(float amountReceived, bool diedAfter, bool wasBlocked = false)
        {
            this.amountReceived = amountReceived;
            this.diedAfter = diedAfter;
            this.wasBlocked = wasBlocked;
        }
    }
}