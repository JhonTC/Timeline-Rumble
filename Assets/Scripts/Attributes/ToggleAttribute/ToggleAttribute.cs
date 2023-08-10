using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Carp.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class ToggleAttribute : PropertyAttribute 
    {
        public string toggleName;

        public ToggleAttribute(string toggleName)
        {
            this.toggleName = toggleName;
        }
    }
}
