using System;
using System.Collections;
using RobotCastle.Core;
using RobotCastle.Saving;
using SleepDev;
using UnityEngine;

#if UNITY_IOS
using UnityEngine.iOS;
#elif UNITY_ANDROID
using Google.Play.Review;
#endif

namespace MergeHunt
{
    public class RateUsManager : MonoBehaviour
    {
        private Action _callback;
        #if UNITY_ANDROID
        private PlayReviewInfo _info;
        private ReviewManager _reviewManager;
        #endif
        private EState _state;
        private Coroutine _operation;
        
        private enum EState
        {
            DidNothing, WaitingForRequest, RequestCompleted, RequestFailed, FailedToShow
        }

        public void Reject()
        {
            CLog.Log($"[{nameof(RateUsManager)}] Player rejected rate us...");
            ServiceLocator.Get<IDataSaver>().GetData<RateUsData>().SetAsRejected();
        }
        
        public void Accept(Action callback)
        {
            _callback = callback;
#if UNITY_IOS
            var reviewResult = Device.RequestStoreReview();
            if (reviewResult)
                _state = EState.RequestCompleted;
            else
                _state = EState.RequestFailed;
            OnShownSuccessfully();
            return;
#elif UNITY_ANDROID
            _reviewManager = new ReviewManager();
            CLog.Log($"[{nameof(RateUsManager)}] Player Accepted to rate. State: {_state.ToString()}");
            
            switch (_state)
            {
                case EState.DidNothing or EState.RequestFailed:
                    _operation = StartCoroutine(PreparingAndShowing());
                    break;
                case EState.RequestCompleted:
                    _operation = StartCoroutine(Showing());
                    break;
                case EState.WaitingForRequest:
                    StartCoroutine(WaitingUntilRequestCompleted());
                    break;
            }
#endif
        }
        
#if UNITY_ANDROID
        // DO NOT USE FOR NOW
        public void Prepare()
        {
            CLog.Log($"[{nameof(RateUsManager)}] state: {_state.ToString()}");
            if (_state == EState.DidNothing || _state == EState.RequestFailed)
            {
                _operation = StartCoroutine(Requesting());
            }
        }
#endif
        private void OnRequestFailed()
        {
            CLog.Log($"[{nameof(RateUsManager)}] On on error");
            _state = EState.RequestFailed;
            DelayNextShow();
        }

        private void OnFailedToShow()
        {
            CLog.Log($"[{nameof(RateUsManager)}] On Failed to show");
            DelayNextShow();
            InvokeContinueCallback();
        }

        private void InvokeContinueCallback()
        {
            CLog.Log($"[{nameof(RateUsManager)}] ContinueCallback");
            _callback?.Invoke();
        }

        private void DelayNextShow()
        {
            var data = ServiceLocator.Get<IDataSaver>().GetData<RateUsData>();
            data.didShow = true;
            data.didAccept = false;
            data.SetNextShowTime();
        }

        private void OnShownSuccessfully()
        {
            CLog.Log($"[{nameof(RateUsManager)}] On native shown successfully");
            var data = ServiceLocator.Get<IDataSaver>().GetData<RateUsData>();
            data.didAccept = true;
            data.didShow = true;
            InvokeContinueCallback();
        }
        
#if UNITY_ANDROID
        private IEnumerator PreparingAndShowing()
        {
            yield return Requesting();
            if(_state == EState.RequestCompleted)
                yield return Showing();
            else
                OnFailedToShow();
        }
#endif

#if UNITY_ANDROID
        private IEnumerator Requesting()
        {
            CLog.Log($"[{nameof(RateUsManager)}] Started Requesting");
            _state = EState.WaitingForRequest;
            var requestFlow = _reviewManager.RequestReviewFlow();
            yield return requestFlow;
            if (requestFlow.Error != ReviewErrorCode.NoError)
            {
                CLog.LogRed($"[{nameof(RateUsManager)}] ERROR! {requestFlow.Error.ToString()}" );
                OnRequestFailed();
                yield break;
            }
            _info = requestFlow.GetResult();
            if (_info != null)
            {
                CLog.LogGreen($"[{nameof(RateUsManager)}] Successfully obtained play review info");
                _state = EState.RequestCompleted;
            }
            else
                _state = EState.RequestFailed;
        }

        private IEnumerator Showing()
        {
            var launchFlow = _reviewManager.LaunchReviewFlow(_info);
            yield return launchFlow;
            _info = null; // Reset the object
            if (launchFlow.Error != ReviewErrorCode.NoError)
            {
                CLog.LogRed($"[RateUs] ERROR! {launchFlow.Error.ToString()}" );
                _state = EState.FailedToShow;
                OnFailedToShow();
                yield break;
            }
            OnShownSuccessfully();
        }

        private IEnumerator WaitingUntilRequestCompleted()
        {
            const float maxTime = 5f;
            var elapsed = 0f;
            while (elapsed < maxTime && _state == EState.WaitingForRequest)
            {
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
            if (elapsed >= maxTime)
            {
                CLog.Log($"[{nameof(RateUsManager)}] Time out,  request essentially failed");
                _state = EState.RequestFailed;
                if(_operation != null)
                    StopCoroutine(_operation);
                OnRequestFailed();
                InvokeContinueCallback();
            }
            else
            {
                switch (_state)
                {
                    case EState.RequestCompleted:
                        _operation = StartCoroutine(Showing());
                        break;
                    case EState.RequestFailed:
                        OnRequestFailed();
                        InvokeContinueCallback();
                        break;
                }
            }
        }
#endif

    }
}