using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new CharacterTypeDetails", menuName = "Carp/Character Type Details")]
public class CharacterTypeDetails : ScriptableObject
{
    public CharacterType characterType;
    public Color colour;

    #region operators
    public static bool operator ==(CharacterTypeDetails _details, CharacterType _characterType)
    {
        return _details.characterType == _characterType;
    }

    public static bool operator !=(CharacterTypeDetails _details, CharacterType _characterType)
    {
        return _details.characterType != _characterType;
    }
    #endregion

    #region overrides
    public override bool Equals(object _other)
    {
        if (_other is CharacterType)
        {
            return this == (CharacterType)_other;
        }

        return base.Equals(_other);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    #endregion
}
