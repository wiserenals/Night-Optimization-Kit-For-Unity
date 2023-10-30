using System;
using System.Threading.Tasks;
#if UNITY_EDITOR
    using UnityEditor;
#endif
using UnityEngine;

namespace Karayel
{
    public class Loop
    {
        private readonly int _milliseconds;
        private readonly Func<int> _msAction;
        private bool _run = true;

        private bool GameRunning =>
        #if UNITY_EDITOR
            EditorApplication.isPlaying;
        #else
            Application.isPlaying;
        #endif

        public Loop(int milliseconds)
        {
            _milliseconds = milliseconds;
        }
        
        public Loop(Func<int> milliseconds)
        {
            _msAction = milliseconds;
        }
    
        public async void Start(Action loopAction)
        {
            if(_msAction == null) while (_run && GameRunning)
            {
                loopAction();
                await Task.Delay(_milliseconds);
            }
            else while (_run && GameRunning)
            {
                loopAction();
                await Task.Delay(_msAction());
            }
        }

        public void Stop()
        {
            _run = false;
        }
    }
}
