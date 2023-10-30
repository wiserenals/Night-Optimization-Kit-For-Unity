using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Karayel;

namespace NightJob
{
    public struct ActionJob : INightJob
    {
        public bool JobCompleted { get; set; }

        private Action _mainThreadAction;
        private Action<List<object>> _mainThreadActionWithParams;
        private AsyncDelegate _secondaryThreadAsyncDelegate;
        private Action<List<object>> _secondaryThreadActionWithParams;

        private List<object> _params;

        public ActionJob(Action mainThreadAction,
            Action<List<object>> mainThreadActionWithParams,
            AsyncDelegate secondaryThreadAsyncDelegate,
            Action<List<object>> secondaryThreadActionWithParams)
        {
            JobCompleted = false;
            _mainThreadAction = mainThreadAction;
            _mainThreadActionWithParams = mainThreadActionWithParams;
            _secondaryThreadAsyncDelegate = secondaryThreadAsyncDelegate;
            _secondaryThreadActionWithParams = secondaryThreadActionWithParams;
            _params = new List<object>();
        }

        public IEnumerator OnMainThread()
        {
            if (_mainThreadAction != null)
            {
                _mainThreadAction();
            }
            else if (_mainThreadActionWithParams != null)
            {
                _mainThreadActionWithParams(_params);
            }
            yield break;
        }

        public async Task OnSecondaryThread()
        {
            if (_secondaryThreadAsyncDelegate != null)
            {
                await _secondaryThreadAsyncDelegate(_params);
            }
            else if (_secondaryThreadActionWithParams != null)
            {
                _secondaryThreadActionWithParams(_params);
            }
        }
    }
}