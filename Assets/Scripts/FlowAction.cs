using Carp.Attributes;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractAction
{
    public Action<AbstractAction, object, object> onComplete { get; set; }
    public bool waitForResult { get; set; }
    public int waitTimeout { get; set; }
    public string name { get; set; }
    public string description { get; set; }

    public AbstractAction(bool _waitForResult = true, int _waitTimeout = 0, string _name = "", string _description = "")
    {
        SetupAction(_waitForResult, _waitTimeout, _name, _description);
    }
    public AbstractAction(bool _waitForResult = true, int _waitTimeout = 0, string _description = "")
    {
        SetupAction(_waitForResult, _waitTimeout, "", _description);
    }
    public AbstractAction(bool _waitForResult = true, int _waitTimeout = 0)
    {
        SetupAction(_waitForResult, _waitTimeout, "", "");
    }

    public void SetupAction(bool _waitForResult, int _waitTimeout, string _name, string _description)
    {
        waitForResult = _waitForResult;
        waitTimeout = _waitTimeout;
        name = _name;

        if (name == "")
        {
            name = ToString();
        }

        description = _description;
    }

    public abstract void InvokeRequest();

    public virtual void Update() {}
}

public class FlowAction : AbstractAction
{
    protected List<AbstractAction> actionQueue = new List<AbstractAction>();
    protected List<AbstractAction> activeActions = new List<AbstractAction>();
    public AbstractAction waitingForActionValue = null;
    protected bool isWaitingForAction = false;
    public bool quitWhenQueueIsEmpty = true;

    public FlowAction(string _name, bool _quitWhenQueueIsEmpty = true) : base(true, 0, _name) 
    {
        quitWhenQueueIsEmpty = _quitWhenQueueIsEmpty;
    }
    public FlowAction(bool _quitWhenQueueIsEmpty = true) : base(true, 0)
    {
        quitWhenQueueIsEmpty = _quitWhenQueueIsEmpty;
    }

    public override void InvokeRequest() { }

    public bool RequestAction(AbstractAction _action)
    {
        if (_action == null) return false;

        //Debug.Log($"RequestAction : {_action.name}");
        actionQueue.Add(_action);

        return true;
    }

    public void InvokeNextActionRequest()
    {
        var actionRequest = actionQueue[0];
        actionQueue.RemoveAt(0);

        activeActions.Add(actionRequest);

        if (actionRequest != null)
        {
            //Debug.Log($"InvokeNextActionRequest: {actionRequest.name}");

            isWaitingForAction = actionRequest.waitForResult;
            
            if (isWaitingForAction)
            {
                waitingForActionValue = actionRequest;
                actionRequest.onComplete += OnWaitingActionComplete;
            }

            actionRequest.InvokeRequest();

            if (!isWaitingForAction)
            {
                if (quitWhenQueueIsEmpty && actionQueue.Count <= 0)
                {
                    onComplete?.Invoke(this, null, null);
                }
            }
        }
    }

    public void OnWaitingActionComplete(AbstractAction sender, object value, object parameter)
    {
        isWaitingForAction = false;
        waitingForActionValue = null;

        OnActionComplete(sender, value, parameter);
    }

    public void OnActionComplete(AbstractAction sender, object value, object parameter)
    {
        if (activeActions.Contains(sender))
        {
            activeActions.Remove(sender);
            sender = null;
        }

        if (quitWhenQueueIsEmpty && actionQueue.Count <= 0)
        {
            onComplete?.Invoke(this, null, null);
        }
    }

    public override void Update()
    {
        if (actionQueue.Count > 0 && !isWaitingForAction)
        {
            InvokeNextActionRequest();
        }

        if (isWaitingForAction)
        {
            if (waitingForActionValue != null)
            {
                waitingForActionValue.Update();
            }

            //todo: handle timeout
        }
    }
}

public class SelectionAction<T> : AbstractAction
{
    protected List<T> selectedItems = new List<T>();
    protected Selector<T> itemSelector;
    protected int numberOfItemsToSelect;

    public SelectionAction(int _numberOfItemsToSelect = 1, bool _waitForResult = true, int _waitTimeout = 0, string _description = "") 
        : base(_waitForResult, _waitTimeout, _description)
    {
        numberOfItemsToSelect = _numberOfItemsToSelect;
    }

    public override void InvokeRequest()
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

public class SelectionAction<T, P> : SelectionAction<T>
{
    protected P extraParam;
    public SelectionAction(P _extraParam, int _numberOfItemsToSelect = 1, bool _waitForResult = true, int _waitTimeout = 0, string _description = "") 
        : base(_numberOfItemsToSelect, _waitForResult, _waitTimeout, _description)
    {
        extraParam = _extraParam;
    }

    public SelectionAction(P _extraParam, int _numberOfItemsToSelect = 1, string _description = "")
        : base(_numberOfItemsToSelect, true, 0, _description)
    {
        extraParam = _extraParam;
    }

    protected override void OnComplete()
    {
        onComplete?.Invoke(this, selectedItems.ToArray(), extraParam);
    }
}

public class CharacterSelectionAction<P> : SelectionAction<Character, P>
{
    private CharacterType effectTargetType;

    public CharacterSelectionAction(CharacterType _effectTargetType, P _extraParam, int _numberOfItemsToSelect = 1, string _description = "") 
        : base(_extraParam, _numberOfItemsToSelect, _description)
    {
        effectTargetType = _effectTargetType;
    }

    public override void InvokeRequest()
    {
        Debug.Log(description);

        itemSelector = new CharacterSelector(effectTargetType);
        itemSelector.OnTypeSelected += OnItemSelected;
    }
}
