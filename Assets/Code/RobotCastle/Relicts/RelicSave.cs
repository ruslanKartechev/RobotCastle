namespace RobotCastle.Relics
{
    [System.Serializable]
    public class RelicSave
    {
        public RelicCore core;
        public bool isEquipped;
        public int amount;
        
        public RelicSave(){}

        public RelicSave(RelicCore core)
        {
            this.core = core;
            isEquipped = false;
            amount = 1;
        }
        
        public RelicSave(RelicSave other)
        {
            core = new RelicCore(other.core);
            isEquipped = other.isEquipped;
            amount = other.amount;
        }
        
    }
}