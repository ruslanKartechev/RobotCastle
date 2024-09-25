namespace RobotCastle.Battling
{
    [System.Serializable]
    public class PercentStatDecorator : IStatDecorator
    {
        public PercentStatDecorator(){}

        public PercentStatDecorator(float percent) => this.percent = percent;
        
        public float percent;
        
        public int priority => 1;
        
        public string name => $"stat_decorator_+{percent}%";
        
        public float Decorate(float val)
        {
            return val * (percent + 1f);
        }
    }
}