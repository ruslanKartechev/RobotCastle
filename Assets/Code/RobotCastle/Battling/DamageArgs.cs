namespace RobotCastle.Battling
{
    public struct DamageArgs
    {
        public EDamageType type;
        public float amount;

        public DamageArgs(float amount, EDamageType type)
        {
            this.amount = amount;
            this.type = type;
        }

        public string GetStr()
        {
            var msg = $"Damage. {type.ToString()}. Amount: {amount}";
            return msg;
        }
    }
}