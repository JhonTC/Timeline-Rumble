using System;

namespace Carp.Process
{
    public abstract class AbstractProcess
    {
        public Action<AbstractProcess, object, object> onComplete { get; set; }
        public bool waitForResult { get; set; }
        public int waitTimeout { get; set; }
        public string name { get; set; }
        public string description { get; set; }

        public AbstractProcess(bool _waitForResult = true, int _waitTimeout = 0, string _name = "", string _description = "")
        {
            SetupProcess(_waitForResult, _waitTimeout, _name, _description);
        }
        public AbstractProcess(bool _waitForResult = true, int _waitTimeout = 0, string _description = "")
        {
            SetupProcess(_waitForResult, _waitTimeout, "", _description);
        }
        public AbstractProcess(bool _waitForResult = true, int _waitTimeout = 0)
        {
            SetupProcess(_waitForResult, _waitTimeout, "", "");
        }

        public void SetupProcess(bool _waitForResult, int _waitTimeout, string _name, string _description)
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

        public abstract void InvokeProcess();

        public virtual void Update() { }
    }
}
