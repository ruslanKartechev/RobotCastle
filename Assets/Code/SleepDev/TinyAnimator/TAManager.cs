using System.Threading;
using UnityEngine;

namespace SleepDev.TinyAnimator
{
    public class TAManager : MonoBehaviour
    {
        private static TAManager _instance;
        public static TAManager Instance => _instance;
         

        public static void Stop<T>(TinyAnimator<T> animator)
        {
            animator.Stop();
        }

    }

    public class TinyAnimator<T>
    {
        private CancellationTokenSource _tokenSource;
        public CancellationToken StopToken => _tokenSource.Token;

        public T Target
        {
            get;
            private set;
        }
        
        public TinyAnimator(T target)
        {
            this.Target = target;
            _tokenSource = new CancellationTokenSource();
        }

        public void Stop()
        {
            _tokenSource.Cancel();
        }
    }
}