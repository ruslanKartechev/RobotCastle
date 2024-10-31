using System.Collections.Generic;

namespace RobotCastle.Battling
{
    public class HeroProcessesContainer
    {
        private List<IHeroProcess> activeProcesses { get; } = new(10);

        public void Add(IHeroProcess process)
        {
            activeProcesses.Add(process);
        }

        public void Remove(IHeroProcess process)
        {
            activeProcesses.Remove(process);
        }
        
        public void StopAll()
        {
            for (var i = activeProcesses.Count-1; i >= 0; i--)
            {
                activeProcesses[i].Stop();
            }
            activeProcesses.Clear();
        }
    }
}