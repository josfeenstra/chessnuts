using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nez;

namespace Chessnuts
{
    enum Intelligence
    {
        VeryEasy = -1, // TODO make him just choose random moves with an intellgence this low
        Easy = 0,
        Medium = 2,
        Hard = 3,
        VeryHard = 4,
    }

    class PlayerAI : Player
    {
        public Intelligence intelligence = Intelligence.Easy;
        public float moveTime = 0.4f;

        public PlayerAI(PieceColor owns, Intelligence intelligence) : base(owns, "computer")
        {
            this.intelligence = intelligence;
        }

        public async override void CalculatePlay(ChessBoard board, bool isCheck)
        {
            float time = Time.TotalTime;
            var move = await board.GetBestMoveAsync(this.ColorOwned, (int)intelligence, isCheck);

            // dont make a move too fast
            float thinkTime = Time.TotalTime - time;
            if (thinkTime < moveTime)
            {
                CommitDelayedMove(move, moveTime - thinkTime);
            }
            else
            {
                // do a random move 
                CommitMove(move);
            }
        }
    }
}
