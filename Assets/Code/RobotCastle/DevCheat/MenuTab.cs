using System;

namespace RobotCastle.DevCheat
{
    public class MenuTab : Tab
    {
        public override void Show(Action closeCallback)
        {
            gameObject.SetActive(true);
        }

        public override void Close()
        {
            gameObject.SetActive(false);
        }
    }
}