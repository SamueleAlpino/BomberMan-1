using BehaviourEngine;
using BehaviourEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BomberMan.GameObjects;
using OpenTK;

namespace BomberMan
{
    public sealed class GameManager : Behaviour, IUpdatable
    {
        #region Singleton
        private static GameManager instance;
        public static GameManager Instance => instance ?? ( new GameManager( null ) ); // null it's correct since this instance does not belong to any Owner.
        #endregion

        #region FSM
        private StateGameSetup gameSetup;
        private StateGameLoop gameLoop;
        private StateGameWin gameWin;
        private StateGameLose gameLose;
        private IState currentState;
        #endregion

        private Player  player;
        private Level   currentLevel;

        private GameManager(GameObject owner) : base(owner)
        {
            gameSetup      = new StateGameSetup(this);
            gameLoop       = new StateGameLoop(this);
            gameWin        = new StateGameWin(this);
            gameLose       = new StateGameLose(this);

            //Link up fsm any state
            gameSetup.Next = gameLoop;
            gameLoop.NextW = gameWin;
            gameLoop.NextL = gameLose;
            gameWin.Next   = gameLoop;
            gameLose.Next  = gameLoop;

            currentState   = gameSetup;
            gameSetup.OnStateEnter();
        }

        public void Update()
        {
            currentState = currentState.OnStateUpdate();
        }

        private class StateGameSetup : IState
        {
            public StateGameLoop Next { get; set; }
            private GameManager owner { get; }
            public StateGameSetup(GameManager owner)
            {
                this.owner = owner;
                //ciao
                //olè
            }

            public void OnStateEnter()
            {
                InitTextures();

                Engine.Spawn(new Camera());

                owner.currentLevel = new Level(Engine.LevelPath + "/Level00" + ".csv", "Base0", 0);
                Level.Load("Base0");

                owner.player        = new Player("Bomberman", Map.playerSpawnPos);
                Engine.Spawn(new SpawnManager(owner.currentLevel.currentMap, owner.player));

                Engine.Spawn(owner.player);

                InitObjectPooling();

                OnStateUpdate();
            }

            public void OnStateExit()
            {
            }

            public IState OnStateUpdate()
            {
                //Node.ShowPath();

                return this;
            }

            private void InitTextures()
            {
                FlyWeight.Add("Wall", "Textures/wall.dat");
                FlyWeight.Add("Obstacle", "Textures/obstacle.dat");
                FlyWeight.Add("Warrior", "Textures/warrior.dat");
                FlyWeight.Add("Bomb", "Textures/Bomb.dat");
                FlyWeight.Add("Explosion", "Textures/Explosion.dat");
                FlyWeight.Add("Bomberman","Textures/bomberman.dat" );
                FlyWeight.Add("Balloon","Textures/ballon.dat" );
                FlyWeight.Add("expl2","Textures/expl2.dat" );
            }

            private void InitObjectPooling()
            {
                Pool<Bomb>.Register(() => new Bomb(owner.player.Transform.Position), 100);
            }
        }

        private class StateGameLoop : IState
        {
            public StateGameWin     NextW { get; set; }
            public StateGameLose    NextL { get; set; }
            private GameManager     owner { get; }

            public StateGameLoop(GameManager owner)
            {
                this.owner = owner;
            }

            public void OnStateEnter()
            {
                throw new NotImplementedException();
            }

            public void OnStateExit()
            {
                throw new NotImplementedException();
            }

            public IState OnStateUpdate()
            {
                throw new NotImplementedException();
            }
        }

        private class StateGameWin : IState
        {
            public StateGameLoop Next { get; set; }
            private GameManager owner { get; }

            public StateGameWin(GameManager owner)
            {
                this.owner = owner;
            }

            public void OnStateEnter()
            {
                throw new NotImplementedException();
            }

            public void OnStateExit()
            {
                throw new NotImplementedException();
            }

            public IState OnStateUpdate()
            {
                throw new NotImplementedException();
            }
        }

        private class StateGameLose : IState
        {
            public StateGameLoop Next { get; set; }
            private GameManager owner { get; }

            public StateGameLose(GameManager owner)
            {
                this.owner = owner;
            }

            public void OnStateEnter()
            {
                throw new NotImplementedException();
            }

            public void OnStateExit()
            {
                throw new NotImplementedException();
            }

            public IState OnStateUpdate()
            {
                throw new NotImplementedException();
            }
        }
    }
}
