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
            foreach (var s in activeProcesses)
                s.Stop();
            activeProcesses.Clear();
        }
    }
}