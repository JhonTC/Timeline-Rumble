using System;

namespace Carp.Collections.Generic
{
    [Serializable]
    public abstract class Pair<K, V>
    {
        public K key;
        public V value;

        #region operators
        public static bool operator ==(Pair<K, V> _pair, K _key)
        {
            return _pair.key.Equals(_key);
        }

        public static bool operator !=(Pair<K, V> _pair, K _key)
        {
            return !_pair.key.Equals(_key);
        }

        public static bool operator ==(Pair<K, V> _pair, V _value)
        {
            return _pair.value.Equals(_value);
        }

        public static bool operator !=(Pair<K, V> _pair, V _value)
        {
            return !_pair.value.Equals(_value);
        }
        #endregion

        #region overrides
        public override bool Equals(object _other)
        {
            if (_other is K)
            {
                return this == _other;
            }

            if (_other is V)
            {
                return this == _other;
            }

            return base.Equals(_other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}
