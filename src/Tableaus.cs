using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FreeCellSolver
{
    public class Tableaus : IEquatable<Tableaus>
    {
        private readonly Tableau[] _state = new Tableau[]
        {
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
        };

        public Tableau this[int index] => _state[index];

        public int EmptyTableauCount
        {
            get
            {
                var emptyCount = 0;

                for (var i = 0; i < 8; i++)
                {
                    if (_state[i].IsEmpty)
                    {
                        emptyCount++;
                    }
                }

                return emptyCount;
            }
        }

        public Tableaus(Tableau tableau1, Tableau tableau2, Tableau tableau3, Tableau tableau4, Tableau tableau5, Tableau tableau6, Tableau tableau7, Tableau tableau8)
        {
            _state[0] = tableau1.Clone();
            _state[1] = tableau2.Clone();
            _state[2] = tableau3.Clone();
            _state[3] = tableau4.Clone();
            _state[4] = tableau5.Clone();
            _state[5] = tableau6.Clone();
            _state[6] = tableau7.Clone();
            _state[7] = tableau8.Clone();
        }

        public bool CanReceive(Card card, int exclude)
        {
            for (var i = 0; i < 8 && card != null; i++)
            {
                if (exclude != i && _state[i].CanPush(card))
                {
                    return true;
                }
            }

            return false;
        }

        public Tableaus Clone() => new Tableaus(
            _state[0],
            _state[1],
            _state[2],
            _state[3],
            _state[4],
            _state[5],
            _state[6],
            _state[7]);

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("01 02 03 04 05 06 07 08");
            sb.AppendLine("-- -- -- -- -- -- -- --");

            for (var r = 0; r < _state.Max(t => t.Size); r++)
            {
                for (var c = 0; c < 8; c++)
                {
                    var size = _state[c].Size;
                    sb.Append(size > r ? _state[c][size - r - 1].ToString() : "  ");
                    sb.Append(c < 7 ? " " : "");
                }

                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }

        // Used only for post moves asserts
        internal IEnumerable<Card> AllCards() => _state.SelectMany(t => t.AllCards());

        #region Equality overrides and overloads
        public bool Equals([AllowNull] Tableaus other) => other == null
            ? false
            : _state[0] == other._state[0]
                && _state[1] == other._state[1]
                && _state[2] == other._state[2]
                && _state[3] == other._state[3]
                && _state[4] == other._state[4]
                && _state[5] == other._state[5]
                && _state[6] == other._state[6]
                && _state[7] == other._state[7];

        public override bool Equals(object obj) => obj is Tableaus deal && Equals(deal);

        public override int GetHashCode() => HashCode.Combine(
            _state[0],
            _state[1],
            _state[2],
            _state[3],
            _state[4],
            _state[5],
            _state[6],
            _state[7]);

        public static bool operator ==(Tableaus a, Tableaus b) => Equals(a, b);

        public static bool operator !=(Tableaus a, Tableaus b) => !(a == b);
        #endregion
    }
}