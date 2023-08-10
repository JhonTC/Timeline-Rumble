using System;
using UnityEngine;

namespace Carp.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class ReadOnlyAttribute : PropertyAttribute {}
}

