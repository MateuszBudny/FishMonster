using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoatHook : MonoBehaviour, IInteractable
{
    public float DamageOnHookStarted { get; set; } = 10f;
    public float DamageOnEveryTickWhileHooked { get; set; } = 1f;
    public float TickDuration { get; set; } = 1f;
    public float DamageOnPlayerBoostWhileHooked { get; set; } = 5f;

    private FixedJoint joint;
    private FishMonster fishMonsterHooked;

    public bool JumpToThisInteractable => !joint;

    public void Interact(IPlayer player)
    {
        if(!fishMonsterHooked)
        {
            joint = gameObject.AddComponent<FixedJoint>();
            fishMonsterHooked = player as FishMonster;
            joint.connectedBody = fishMonsterHooked.Rigid;
            player.ReceiveDamage(DamageOnHookStarted);
            InvokeRepeating(nameof(ApplyDamageToPlayerOnEveryTick), TickDuration, TickDuration);
        }
        else
        {
            Destroy(joint);
            fishMonsterHooked = null;
            CancelInvoke(nameof(ApplyDamageToPlayerOnEveryTick));
        } 
    }

    public void PlayerBoostImpulse()
    {
        fishMonsterHooked.ReceiveDamage(DamageOnPlayerBoostWhileHooked);
    }

    private void ApplyDamageToPlayerOnEveryTick()
    {
        fishMonsterHooked.ReceiveDamage(DamageOnEveryTickWhileHooked);
    }
}
