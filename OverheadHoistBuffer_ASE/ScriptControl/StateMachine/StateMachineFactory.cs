using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using com.mirle.iibg3k0.ttc.Common.TCPIP;
using Stateless;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.StateMachine
{
    internal static class StateMachineFactory
    {
        static public StateMachine<TSCState, TSCEvent> CreatTSCStateMachine(Func<TSCState> stateAccessor, Action<TSCState> stateMutator)
        {
            StateMachine<TSCState, TSCEvent> fsm = new Stateless.StateMachine<TSCState, TSCEvent>(stateAccessor, stateMutator);
            #region define transition
            fsm.Configure(TSCState.Tscnone).
                Permit(TSCEvent.Tscinitial, TSCState.Tscint);

            fsm.Configure(TSCState.Tscint).
                Permit(TSCEvent.SystemStartedUpSuccessfully, TSCState.Paused);

            fsm.Configure(TSCState.Paused).
                Permit(TSCEvent.Tscresumed, TSCState.Auto);

            fsm.Configure(TSCState.Auto).
                Permit(TSCEvent.TcsrequestedToPause, TSCState.Pausing);

            fsm.Configure(TSCState.Pausing).
                Permit(TSCEvent.Tscresumed, TSCState.Auto).
                Permit(TSCEvent.PauseComplete, TSCState.Paused);
            #endregion
            return fsm;
        }
    }
}
