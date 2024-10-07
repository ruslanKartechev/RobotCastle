using System.Collections.Generic;

namespace RobotCastle.Core
{
    /// <summary>
    /// DEPRICATED !!!
    /// </summary>
    [System.Serializable]
    public class SaveScrolls
    {
        public List<ScrollSave> scrolls;
        
        public SaveScrolls(){}

        public SaveScrolls(SaveScrolls other)
        {
            var count = other.scrolls.Count;
            scrolls = new(count);
            for (var i = 0; i < count; i++)
            {
                scrolls.Add(new ScrollSave(other.scrolls[i]));
            }
        }
        
     
    }
}