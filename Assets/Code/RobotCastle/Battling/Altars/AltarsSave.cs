using System.Collections.Generic;

namespace RobotCastle.Battling.Altars
{
    [System.Serializable]
    public class AltarsSave
    {
        public int pointsTotal;
        public int pointsFree;
        public List<AltarSave> altars;
        
        public AltarsSave(){}

        public AltarsSave(AltarsSave other)
        {
            pointsTotal = other.pointsTotal;
            pointsFree = other.pointsFree;
            var count = other.altars.Count;
            altars = new List<AltarSave>(count);
            for (var i = 0; i < count; i++)
            {
                altars.Add(new AltarSave(other.altars[i]));
            }
        }
    }
}