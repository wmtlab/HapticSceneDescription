using System;
using System.Collections.Generic;

namespace HapticSceneDescription.Gltf.Properties.MpegSceneInteractivity
{
    public class Behavior
    {
        public List<int> Triggers { get; set; } = new List<int>();
        public List<int> Actions { get; set; } = new List<int>();

        #region TriggersCombinationControl
        private string _triggersCombinationControl = string.Empty;
        private IExpressionNode _expressionRoot = null;
        private Dictionary<int, Func<bool>> _triggerMethods = new Dictionary<int, Func<bool>>();
        private bool _isTriggerMethodsDirty = true;
        public string TriggersCombinationControl
        {
            get => _triggersCombinationControl;
            set
            {
                _triggersCombinationControl = value;
                _expressionRoot = IExpressionNode.Factory.Parse(value);
            }
        }
        #endregion

        public TriggersActivationControlType TriggersActivationControl { get; set; } = TriggersActivationControlType.FirstEnter;
        public ActionsControlType ActionsControl { get; set; } = ActionsControlType.Sequential;
        public int Priority { get; set; } = 0;

        private TriggerState _triggerState = TriggerState.FirstOff;
        public void TryTrigger(SceneDescription sd)
        {
            _triggerState = UpdateTriggerState(sd, _triggerState);
            if (IsActivationMatched(TriggersActivationControl, _triggerState))
            {
                foreach (var actionIndex in Actions)
                {
                    var action = sd.Actions[actionIndex];
                    action?.Invoke();
                }
            }
        }

        private bool IsActivationMatched(TriggersActivationControlType required, TriggerState state)
        {
            return required switch
            {
                TriggersActivationControlType.None => false,
                TriggersActivationControlType.FirstEnter => state == TriggerState.FirstEnter,
                TriggersActivationControlType.EachEnter => state == TriggerState.Enter || state == TriggerState.FirstEnter,
                TriggersActivationControlType.On => state == TriggerState.On || state == TriggerState.FirstOn,
                TriggersActivationControlType.FirstExit => state == TriggerState.FirstExit,
                TriggersActivationControlType.EachExit => state == TriggerState.Exit || state == TriggerState.FirstExit,
                TriggersActivationControlType.Off => state == TriggerState.Off || state == TriggerState.FirstOff,
                _ => false
            };
        }

        private TriggerState UpdateTriggerState(SceneDescription sd, TriggerState oldState)
        {
            bool isTriggered = CheckTrigger(sd);
            var newState = oldState switch
            {
                TriggerState.FirstOff => isTriggered ? TriggerState.FirstEnter : TriggerState.FirstOff,
                TriggerState.FirstEnter => isTriggered ? TriggerState.FirstOn : TriggerState.FirstExit,
                TriggerState.FirstOn => isTriggered ? TriggerState.FirstOn : TriggerState.FirstExit,
                TriggerState.FirstExit => isTriggered ? TriggerState.Enter : TriggerState.Off,
                TriggerState.Off => isTriggered ? TriggerState.Enter : TriggerState.Off,
                TriggerState.Enter => isTriggered ? TriggerState.On : TriggerState.Exit,
                TriggerState.On => isTriggered ? TriggerState.On : TriggerState.Exit,
                TriggerState.Exit => isTriggered ? TriggerState.Enter : TriggerState.Off,
                _ => TriggerState.FirstOff
            };
            return newState;
        }

        private bool CheckTrigger(SceneDescription sd)
        {
            if (_isTriggerMethodsDirty)
            {
                foreach (var triggerIndex in Triggers)
                {
                    var trigger = sd.Triggers[triggerIndex];
                    _triggerMethods[triggerIndex] = trigger.IsTriggered;
                }
                _isTriggerMethodsDirty = false;
            }

            return _expressionRoot.Evaluate(_triggerMethods);
        }

        public enum TriggerState
        {
            FirstOff,
            FirstEnter,
            FirstOn,
            FirstExit,
            Off,
            Enter,
            On,
            Exit
        }

        public enum TriggersActivationControlType
        {
            None,
            FirstEnter,
            EachEnter,
            On,
            FirstExit,
            EachExit,
            Off
        }
        public enum ActionsControlType
        {
            Sequential,
            Parallel
        }
    }
}