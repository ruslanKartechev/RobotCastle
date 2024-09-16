using System.Collections.Generic;
using RobotCastle.Data;
using RobotCastle.Merging;

namespace RobotCastle.Battling
{
    public interface IPlayerUnit
    {
        List<CoreItemData> Upgrades { get; }
        
    }
    
    
}