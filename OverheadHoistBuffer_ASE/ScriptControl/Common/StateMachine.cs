using com.mirle.ibg3k0.sc.App;
using Stateless;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Common
{
    public interface IStateMachine<TState, TTrigger>
    {
        bool IsInState(TState state);
        bool CanFire(TTrigger trigger_evevt);
        void Fire(TTrigger trigger_event);
        TState State { get; }
    }



    public class StateMachineStateless<TState, TTrigger> : IStateMachine<TState, TTrigger>
    {
        public string ID { get; private set; }
        private Stateless.StateMachine<TState, TTrigger> sm;
        public StateMachineStateless(string _id, Stateless.StateMachine<TState, TTrigger> _sm)
        {
            ID = _id;
            sm = _sm;

            sm.OnTransitioned(TransitionedHandler);
            sm.OnUnhandledTrigger(UnhandledTriggerHandler);
        }
        public bool IsInState(TState state)
        {
            return sm.IsInState(state);
        }
        public bool CanFire(TTrigger trigger_evevt)
        {
            return sm.CanFire(trigger_evevt);
        }
        public void Fire(TTrigger trigger_event)
        {
            sm.Fire(trigger_event);
        }
        public TState State
        {
            get { return sm.State; }
        }

        //Task TransitionedHandler(Stateless.StateMachine<TState, TTrigger>.Transition transition)
        void TransitionedHandler(Stateless.StateMachine<TState, TTrigger>.Transition transition)
        {
            string Destination = transition.Destination.ToString();
            string Source = transition.Source.ToString();
            string Trigger = transition.Trigger.ToString();
            string IsReentry = transition.IsReentry.ToString();
            LogCollection.VHStateDebug(string.Format("VH-[{0}] state,From:{1} to:{2} by:{3}.IsReentry:{4}"
                                                            , ID
                                                            , Source
                                                            , Destination
                                                            , Trigger
                                                            , IsReentry));
        }

        //Task UnhandledTriggerHandler(TState state, TTrigger trigger)
        void UnhandledTriggerHandler(TState state, TTrigger trigger)
        {
            string SourceState = state.ToString();
            string Trigger = trigger.ToString();
            LogCollection.VHStateWarn(string.Format("VH-[{0}] state ,unhandled trigger happend ,source state:{1} trigger:{2}"
                                                            , ID
                                                            , SourceState
                                                            , Trigger));
        }


    }
}
