namespace RobotCastle.Battling
{
    public struct DamageReceivedArgs
    {
        public float amountReceived;
        public bool diedAfter;

        public DamageReceivedArgs(float amountReceived, bool diedAfter)
        {
            this.amountReceived = amountReceived;
            this.diedAfter = diedAfter;
        }
    }
}