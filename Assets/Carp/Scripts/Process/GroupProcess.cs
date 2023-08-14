using System.Collections.Generic;

namespace Carp.Process
{
    public class GroupProcess : AbstractProcess
    {
        protected List<AbstractProcess> processQueue = new List<AbstractProcess>();
        protected List<AbstractProcess> activeProcesses = new List<AbstractProcess>();
        public AbstractProcess waitingForProcessValue = null;
        protected bool isWaitingForProcess = false;
        public bool quitWhenQueueIsEmpty = true;

        public GroupProcess(string _name, bool _quitWhenQueueIsEmpty = true) : base(true, 0, _name)
        {
            quitWhenQueueIsEmpty = _quitWhenQueueIsEmpty;
        }
        public GroupProcess(bool _quitWhenQueueIsEmpty = true) : base(true, 0)
        {
            quitWhenQueueIsEmpty = _quitWhenQueueIsEmpty;
        }

        public override void InvokeProcess() { }

        public bool RequestProcess(AbstractProcess _action)
        {
            if (_action == null) return false;

            //Debug.Log($"RequestAction : {_action.name}");
            processQueue.Add(_action);

            return true;
        }

        public void InvokeNextProcess()
        {
            var actionRequest = processQueue[0];
            processQueue.RemoveAt(0);

            activeProcesses.Add(actionRequest);

            if (actionRequest != null)
            {
                //Debug.Log($"InvokeNextActionRequest: {actionRequest.name}");

                isWaitingForProcess = actionRequest.waitForResult;

                if (isWaitingForProcess)
                {
                    waitingForProcessValue = actionRequest;
                    actionRequest.onComplete += OnWaitingActionComplete;
                }

                actionRequest.InvokeProcess();

                if (!isWaitingForProcess)
                {
                    if (quitWhenQueueIsEmpty && processQueue.Count <= 0)
                    {
                        onComplete?.Invoke(this, null, null);
                    }
                }
            }
        }

        public void OnWaitingActionComplete(AbstractProcess sender, object value, object parameter)
        {
            isWaitingForProcess = false;
            waitingForProcessValue = null;

            OnActionComplete(sender, value, parameter);
        }

        public void OnActionComplete(AbstractProcess sender, object value, object parameter)
        {
            if (activeProcesses.Contains(sender))
            {
                activeProcesses.Remove(sender);
                sender = null;
            }

            if (quitWhenQueueIsEmpty && processQueue.Count <= 0)
            {
                onComplete?.Invoke(this, null, null);
            }
        }

        public override void Update()
        {
            if (processQueue.Count > 0 && !isWaitingForProcess)
            {
                InvokeNextProcess();
            }

            if (isWaitingForProcess)
            {
                if (waitingForProcessValue != null)
                {
                    waitingForProcessValue.Update();
                }

                //todo: handle timeout
            }
        }
    }
}
