using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractInvoker : MonoBehaviour, IInteractable
{
    [SerializeField]
    private bool jumpToThisInteractable = true;
    [SerializeField]
    private UnityEvent<IPlayer> onInteractEvent;

    public bool JumpToThisInteractable => jumpToThisInteractable;

    public void Interact(IPlayer player)
    {
        onInteractEvent.Invoke(player);
    }
}
