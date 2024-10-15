namespace RobotCastle.Battling
{
    public interface IDamageReceiver : IGameObjectComponent
    {
        /// <returns>Amount of damage actually received after all defence stats</returns>
        DamageReceivedArgs TakeDamage(HeroDamageArgs args);
    }

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