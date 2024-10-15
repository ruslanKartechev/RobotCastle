namespace RobotCastle.Battling
{
    public struct HeroDamageArgs
    {
        public float amount;
        public EDamageType type;
        public HeroComponents source;
        public bool reflected;
        public bool critical;

        public HeroDamageArgs(float amount, EDamageType type, HeroComponents source)
        {
            this.amount = amount;
            this.type = type;
            this.source = source;
            reflected = false;
            critical = false;
        }


        public HeroDamageArgs(float amount, EDamageType type, HeroComponents source, bool reflected, bool critical)
        {
            this.amount = amount;
            this.type = type;
            this.source = source;
            this.reflected = reflected;
            this.critical = critical;
        }

        public string GetStr()
        {
            var msg = $"Damage:{amount}, Type: {type.ToString()}";
            return msg;
        }
    }
}