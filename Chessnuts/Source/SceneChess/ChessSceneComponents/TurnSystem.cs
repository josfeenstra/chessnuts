using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Raven;
using Nez.UI;
using Microsoft.Xna.Framework;

namespace Chessnuts
{
    class TurnSystem : ChessSceneComponent
    {
        ChessSceneBoard visualBoard;
        Player[] players;

        int turn;
        int currentPlayer;

        public int Turn { get => turn; }
        public Player Player { get => players[currentPlayer]; }

        // UI
        public Label label1 = null, label2 = null;

        public TurnSystem(ChessSceneBoard visualBoard, IEnumerable<Player> players)
        {
            this.visualBoard = visualBoard;

            this.players = players.ToArray();
            foreach (Player player in players)
            {
                player.SetTurnSystem(this);
            }
        }

        public void Activate()
        {
            Player.CalculatePlay(visualBoard.GetCurrentBoardState(), false);
        }

        public void CommenseNextTurn(ChessMove move, float time)
        {
            Timers.ActionAfterSeconds(time, NextTurn, move, this.Scene);
        }

        /// <summary>
        /// Called when a new play has been found by the player. 
        /// </summary>
        public bool NextTurn(ChessMove move)
        {
            if (move.IsLike(ChessMove.None))
            {
                SetText("Draw", "");
                return false;
            }
            var succes = visualBoard.CheckAndApplyPlay(move);
            // Nez.Insist.IsTrue(succes); // otherwise player did something illegal
            
            var status = visualBoard.BoardState.CheckForCheck(Player.ColorOwned.Next(), Player.ColorOwned);
            bool isCheck = false;
            switch (status)
            {
                case CheckStatus.None:
                    SetText("", "");
                    break;
                case CheckStatus.Check:
                    SetText("Check", "");
                    isCheck = true;
                    break;
                case CheckStatus.CheckMate:
                    SetText("CheckMate", Player.ColorOwned.ToString() + " Player Wins!");
                    return false;
                case CheckStatus.StaleMate:
                    SetText("Draw", "");
                    return false;
            }
            // commense next turn
            turn += 1;
            currentPlayer += 1;
            if (currentPlayer >= players.Length) currentPlayer = 0;
            Player.CalculatePlay(visualBoard.GetCurrentBoardState(), isCheck);
            return true;
        }

        /// <summary>
        /// Create a table or something, and change the text
        /// </summary>
        public void SetText(string message1, string message2)
        {
            // lazy inititalization
            if (label1 == null || label2 == null)
            {
                Shared.InitTable(Scene, out label1, out label2);
            }
            label1.SetText(message1);
            label2.SetText(message2);
        }


    }
}
