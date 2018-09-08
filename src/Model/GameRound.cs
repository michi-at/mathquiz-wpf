using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathQuizWPF
{
    public abstract class GameRound
    {
        protected string Question { set; get; }
        protected string Answer { set; get; }

        public abstract string GetQuestion();
        public abstract bool? CheckAnswer(string input);
        public abstract string GetAnswer();
        public abstract void Setup(DifficultyLevel mode);
    }
}
