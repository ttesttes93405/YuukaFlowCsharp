
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace YuukaFlow
{

    public class ImplementationCreator<TPortId, TContext>
    {

        readonly TPortId defaultOutputPortId;
        public ImplementationCreator(TPortId defaultOutputPortId)
        {
            this.defaultOutputPortId = defaultOutputPortId;
        }

        public Func<TContext, Task<TPortId>> Create(Func<TContext, TPortId> implementation = null)
        {
            return context =>
            {
                implementation ??= _ => defaultOutputPortId;
                var result = implementation.Invoke(context);
                return Task.FromResult(result ?? defaultOutputPortId);
            };
        }

        public Func<TContext, Task<TPortId>> Create(Action<TContext> implementation = null, TPortId portId = default)
        {
            return state =>
            {
                implementation?.Invoke(state);
                return Task.FromResult(portId ?? defaultOutputPortId);
            };
        }

        public Func<TContext, Task<TPortId>> CreateTo(TPortId portId)
        {
            return Create(null, portId);
        }

        public Func<TContext, Task<TPortId>> CreateToDefaultOutput(Action<TContext> implementation)
        {
            return Create(implementation, defaultOutputPortId);
        }

        public Func<TContext, Task<TPortId>> CreateTerminal()
        {
            return Create(null, defaultOutputPortId);
        }

    }
}