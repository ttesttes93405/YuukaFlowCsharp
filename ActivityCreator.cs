
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Codice.CM.SEIDInfo;

namespace YuukaFlow
{

    public class ActivityCreator<TPortId, TState>
    {

        readonly TPortId DefaultOutputPortId;
        public ActivityCreator(TPortId defaultOutputPortId)
        {
            DefaultOutputPortId = defaultOutputPortId;
        }

        public Func<TState, Task<TPortId>> Create(Action<TState> activity = null, TPortId portId = default)
        {
            return state =>
            {
                activity?.Invoke(state);
                return Task.FromResult(portId ?? DefaultOutputPortId);
            };
        }

        public Func<TState, Task<TPortId>> CreateTo(TPortId portId)
        {
            return Create(null, portId);
        }

        public Func<TState, Task<TPortId>> CreateActivityToDefaultOutput(Action<TState> activity)
        {
            return Create(activity, DefaultOutputPortId);
        }

        public Func<TState, Task<TPortId>> CreateTerminal()
        {
            return Create(null, DefaultOutputPortId);
        }

    }
}