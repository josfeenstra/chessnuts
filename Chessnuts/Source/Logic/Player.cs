using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Raven;

namespace Chessnuts
{
    // Player vs Player
    // Player vs AI 
    // AI vs AI
    abstract class Player
    {
        public readonly PieceColor ColorOwned;
        public string name;
        private TurnSystem TurnSystem;

        public Player(PieceColor owns, string name)
        {
            this.ColorOwned = owns;
            this.name = name;
        }

        /// <summary>
        /// Called when a new Play is requested. Call "CommitPlay" when done
        /// </summary>
        public abstract void CalculatePlay(ChessBoard board, bool IsCheck);

        public void CommitMove(ChessMove move)
        {
            TurnSystem.NextTurn(move);
        }

        public void CommitDelayedMove(ChessMove move, float time)
        {
            TurnSystem.CommenseNextTurn(move, time);
        }

        internal void SetTurnSystem(TurnSystem turnSystem)
        {
            TurnSystem = turnSystem;
        }
    }
}
