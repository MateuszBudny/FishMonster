using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NpcCharacterState : CharacterState
{
    protected NpcCharacterBehaviour NpcCharacterBehaviour => characterBehaviour as NpcCharacterBehaviour;

    public NpcCharacterState(NpcCharacterBehaviour npcCharacterBehaviour)
    {
        characterBehaviour = npcCharacterBehaviour;
    }
}
