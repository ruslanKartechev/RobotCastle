namespace RobotCastle.Battling
{
    public struct DamageArgs
    {
        public float physDamage;
        public float magicDamage;

        public IDamageSource damageSource;
        
        public DamageArgs(float physDamage, float magicDamage, IDamageSource source)
        {
            this.physDamage = physDamage;
            this.magicDamage = magicDamage;
            this.damageSource = source;
        }

        public string GetStr()
        {
            var msg = $"Damage. Phys: {physDamage}. Mag: {magicDamage}";
            return msg;
        }
    }
}