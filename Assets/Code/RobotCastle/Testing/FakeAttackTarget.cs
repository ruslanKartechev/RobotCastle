using RobotCastle.Battling;
using UnityEngine;

namespace RobotCastle.Testing
{
    public class FakeAttackTarget : MonoBehaviour, IDamageReceiver
    {
        public static FakeAttackTarget GetNew()
        {
            var go = new GameObject("fake_attack_target");
            return go.AddComponent<FakeAttackTarget>();
        }
            
        public bool IsDamageable => true;

        public void TakeDamage(DamageArgs args)
        {
        }

        public GameObject GetGameObject() => gameObject;
    }
}