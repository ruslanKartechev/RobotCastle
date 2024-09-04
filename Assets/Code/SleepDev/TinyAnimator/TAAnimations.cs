using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace SleepDev.TinyAnimator
{ 
    public static class TAAnimations
    {
        public static async Task Scaling12<TFormula>(Transform target, TFormula formula, CancellationToken token, float scale1, float scale2, float time) where TFormula : ITAFormula
        {
            var elapsed = 0f;
            var scale = scale1;
            while (!token.IsCancellationRequested && elapsed < time && target != null)
            {
                var t = formula.GetValue(elapsed / time);
                scale = Mathf.Lerp(scale1, scale2, t);
                target.localScale = new Vector3(scale, scale, scale);
                elapsed += Time.deltaTime;
                // yield return null;
                await Task.Yield();
            }

            if (token.IsCancellationRequested || target == null)
                return;
            scale = scale2;
            target.localScale = new Vector3(scale, scale, scale);
        }
        
        
        
    }
}