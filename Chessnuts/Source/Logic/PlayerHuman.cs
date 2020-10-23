using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chessnuts
{
    class PlayerHuman : Player
    {
        ChessSceneController controller;

        public PlayerHuman(PieceColor owns, ChessSceneController controller) : base(owns, "player")
        {
            this.controller = controller;
        }

        public override void CalculatePlay(ChessBoard board, bool IsCheck)
        {
            controller.SetActivePlayer(this);
            
        }
    }
}
