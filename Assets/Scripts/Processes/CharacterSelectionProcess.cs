using Carp.Process;
using UnityEngine;

public class CharacterSelectionProcess<P> : SelectionProcess<Character, P>
{
    private CharacterType effectTargetType;

    public CharacterSelectionProcess(CharacterType _effectTargetType, P _extraParam, int _numberOfItemsToSelect = 1, string _description = "")
        : base(_extraParam, _numberOfItemsToSelect, _description)
    {
        effectTargetType = _effectTargetType;
    }

    public override void InvokeProcess()
    {
        Debug.Log(description);

        itemSelector = new CharacterSelector(effectTargetType);
        itemSelector.OnTypeSelected += OnItemSelected;
    }
}
