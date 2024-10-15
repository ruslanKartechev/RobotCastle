namespace RobotCastle.Battling.Altars
{
    [System.Serializable]
    public class AltarSave
    {
        public string id;
        public int points;
        
        public AltarSave(){}

        public AltarSave(AltarSave other)
        {
            id = other.id;
            points = other.points;
        }
    }
}