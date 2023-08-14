using System.Collections.Generic;
using UnityEngine;

namespace Carp.Process
{
    public class SelectionProcess<T> : AbstractProcess
    {
        protected List<T> selectedItems = new List<T>();
        protected Selector<T> itemSelector;
        protected int numberOfItemsToSelect;

        public SelectionProcess(int _numberOfItemsToSelect = 1, bool _waitForResult = true, int _waitTimeout = 0, string _description = "")
            : base(_waitForResult, _waitTimeout, _description)
        {
            numberOfItemsToSelect = _numberOfItemsToSelect;
        }

        public override void InvokeProcess()
        {
            itemSelector = new Selector<T>();
            itemSelector.OnTypeSelected += OnItemSelected;
        }

        protected void OnItemSelected(T _item)
        {
            if (!selectedItems.Contains(_item))
            {
                selectedItems.Add(_item);
                Debug.Log($"Selected: {_item}");
            }
            else
            {
                selectedItems.Remove(_item);
                Debug.Log($"Deselected: {_item}");
            }

            if (IsRequestFulfilled())
            {
                Player.LocalPlayer.selectors.Remove(itemSelector);
                itemSelector.OnTypeSelected -= OnItemSelected;
                itemSelector = null;

                OnComplete();
            }
        }

        protected virtual void OnComplete()
        {
            onComplete?.Invoke(this, selectedItems.ToArray(), null);
        }

        private bool IsRequestFulfilled()
        {
            return selectedItems.Count == numberOfItemsToSelect;
        }
    }

    public class SelectionProcess<T, P> : SelectionProcess<T>
    {
        protected P extraParam;
        public SelectionProcess(P _extraParam, int _numberOfItemsToSelect = 1, bool _waitForResult = true, int _waitTimeout = 0, string _description = "")
            : base(_numberOfItemsToSelect, _waitForResult, _waitTimeout, _description)
        {
            extraParam = _extraParam;
        }

        public SelectionProcess(P _extraParam, int _numberOfItemsToSelect = 1, string _description = "")
            : base(_numberOfItemsToSelect, true, 0, _description)
        {
            extraParam = _extraParam;
        }

        protected override void OnComplete()
        {
            onComplete?.Invoke(this, selectedItems.ToArray(), extraParam);
        }
    }
}
