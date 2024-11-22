using System;
using RobotCastle.Core;
using RobotCastle.Data;
using RobotCastle.UI;

namespace MergeHunt
{
    public class RateUsOffer
    {
        private Action _callback;
        private RateUsPopup _ui;
        
        public RateUsOffer(Action callback)
        {
            _callback = callback;
        }

        public void Show()
        {
            _ui = ServiceLocator.Get<IUIManager>().Show<RateUsPopup>(UIConstants.UIRateUs, () => {});
            _ui.Show(Complete);
        }

        private void Complete()
        {
            _callback?.Invoke();
        }
    }
}