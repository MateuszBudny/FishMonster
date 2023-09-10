using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void Interact(IPlayer player);
    bool JumpToThisInteractable { get; }
}
