
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Codice.CM.SEIDInfo;

namespace YuukaFlow
{

    public class ImplementationCreator<TPortId, TState>
    {

        readonly TPortId defaultOutputPortId;
        public ImplementationCreator(TPortId defaultOutputPortId)
        {
            this.defaultOutputPortId = defaultOutputPortId;
        }

        public Func<TState, Task<TPortId>> Create(Action<TState> implementation = null, TPortId portId = default)
        {
            return state =>
            {
                implementation?.Invoke(state);
                return Task.FromResult(portId ?? defaultOutputPortId);
            };
        }

        public Func<TState, Task<TPortId>> CreateTo(TPortId portId)
        {
            return Create(null, portId);
        }

        public Func<TState, Task<TPortId>> CreateToDefaultOutput(Action<TState> implementation)
        {
            return Create(implementation, defaultOutputPortId);
        }

        public Func<TState, Task<TPortId>> CreateTerminal()
        {
            return Create(null, defaultOutputPortId);
        }

    }
}