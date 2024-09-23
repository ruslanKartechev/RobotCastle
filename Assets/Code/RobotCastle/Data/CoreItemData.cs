namespace RobotCastle.Data
{
    [System.Serializable]
    public class CoreItemData
    {
        /// <summary>
        /// Merge Level
        /// </summary>
        public int level;
        public string id;
        public string type;
        
        public CoreItemData(){}
        
        public CoreItemData(int level, string id, string type)
        {
            this.level = level;
            this.id = id;
            this.type = type;
        }
        
        public CoreItemData(CoreItemData other)
        {
            this.level = other.level; 
            this.id = other.id;
            this.type = other.type;
        }
        
        public static bool operator ==(CoreItemData lhs, CoreItemData rhs)
        {
            if (lhs is null && rhs is null)
                return true;
            if (lhs is null)
                return false;
            if (rhs is null)
                return false;
            var equal = true;
            equal &= lhs.id.Equals(rhs.id);
            equal &= lhs.level.Equals(rhs.level);
            equal &= lhs.type.Equals(rhs.type);
            return equal;
        }

        public static bool operator !=(CoreItemData lhs, CoreItemData rhs)
        {
            return !(lhs == rhs);
        }
        
        public string AsStr() => $"ID: {id}. {type}. Level: {level}";

    }
}