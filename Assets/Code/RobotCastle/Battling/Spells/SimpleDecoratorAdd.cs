namespace RobotCastle.Battling
{
    public class SimpleDecoratorAdd : IStatDecorator
    {
        public SimpleDecoratorAdd(float added)
        {
            this.added = added;
        }
        
        public SimpleDecoratorAdd(float added, int order, string name)
        {
            this.added = added;
            this.name = name;
            this.order = order;
        }

        
        public float added;
        
        public string name { get; set; }
        
        public int order { get; set; }
        
        public float Decorate(float val) => val + added;
    }
}