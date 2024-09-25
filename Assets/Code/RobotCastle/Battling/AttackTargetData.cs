namespace RobotCastle.Battling
{
    public class AttackTargetData
    {
        public IHeroController CurrentEnemy;
        /// <summary>
        /// In case two heroes attack each other, one should move, while another is waiting for the other to take position.  
        /// </summary>
        public bool IsMovingForDuel;

        public void Reset()
        {
            CurrentEnemy = null;
            IsMovingForDuel = false;
        }
    }
}