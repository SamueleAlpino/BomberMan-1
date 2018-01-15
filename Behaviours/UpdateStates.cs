using BehaviourEngine;
using BehaviourEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomberMan.Behaviours
{
    public class UpdateStates : Behaviour, IUpdatable
    {
        private List<IState> states = new List<IState>();
        private IState currentState;

        public UpdateStates(GameObject owner, List<IState> states) : base(owner) => this.states = states;

        public void UpdateState(IState state) => states.Add(state);

        public void Update() => states.ForEach(item => item.OnStateUpdate());
    }
}
