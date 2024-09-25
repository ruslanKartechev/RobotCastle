namespace RobotCastle.Battling
{
    public struct DamageArgs
    {
        public float physDamage;
        public float magicDamage;
        
        public DamageArgs(float physDamage, float magicDamage)
        {
            this.physDamage = physDamage;
            this.magicDamage = magicDamage;
        }

        public string GetStr()
        {
            var msg = $"Damage. Phys: {physDamage}. Mag: {magicDamage}";
            return msg;
        }
    }
}