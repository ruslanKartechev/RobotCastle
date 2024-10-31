using System;

namespace RobotCastle.Relics
{
    [System.Serializable]
    public struct RelicCore
    {
        public string id;
        public int tier;

        public RelicCore(string id, int tier)
        {
            this.id = id;
            this.tier = tier;
        }

        public RelicCore(RelicCore other)
        {
            id = other.id;
            tier = other.tier;
        }

        
        public bool Equals(RelicCore other)
        {
            return id == other.id && tier == other.tier;
        }

        public override bool Equals(object obj)
        {
            return obj is RelicCore other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(id, tier);
        }

        public static bool operator ==(RelicCore l, RelicCore r)
        {
            return l.id == r.id && l.tier == r.tier;
        }

        public static bool operator !=(RelicCore l, RelicCore r)
        {
            return !(l == r);
        }
    }
}