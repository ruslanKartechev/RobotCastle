namespace RobotCastle.Battling
{
    public class SimpleDecoratorMultiplier : IStatDecorator
    {
        public SimpleDecoratorMultiplier(float multiplier)
        {
            this.multiplier = multiplier;
        }
        
        public SimpleDecoratorMultiplier(float multiplier, int order, string name)
        {
            this.multiplier = multiplier;
            this.name = name;
            this.order = order;
        }
        
        public float multiplier;
        
        public string name { get; set; }
        
        public int order { get; set; }

        public float Decorate(float val)
        {
            val *= (1 + multiplier);
            return val;
        }
    }
}