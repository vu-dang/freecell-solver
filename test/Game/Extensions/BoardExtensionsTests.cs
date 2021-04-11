﻿using System.IO;
using FreeCellSolver.Game;
using FreeCellSolver.Game.Extensions;
using Xunit;

namespace FreeCellSolver.Test
{
    public class BoardExtensionsTests
    {
        static readonly Board _b1 = Board.Create(Reserve.Create(), Foundation.Create(), Tableaus.Create(
            Tableau.Create("JD KD 2S 4C 3S 6D 6S"),
            Tableau.Create("2D KC KS 5C TD 8S 9C"),
            Tableau.Create("9H 9S 9D TS 4S 8D 2H"),
            Tableau.Create("JC 5S QD QH TH QS 6H"),
            Tableau.Create("5D AD JS 4H 8H 6C"),
            Tableau.Create("7H QC AS AC 2C 3D"),
            Tableau.Create("7C KH AH 4D JH 8C"),
            Tableau.Create("5H 3H 3C 7S 7D TC")
        ));

        static readonly Board _b2 = Board.Create(Reserve.Create(), Foundation.Create(), Tableaus.Create(
            Tableau.Create("QD 4D TD 7S AH 3H AS"),
            Tableau.Create("QC JD JC 9D 9S AD 5S"),
            Tableau.Create("KC JS 8C KS TC 7H TH"),
            Tableau.Create("3C 6H 6C 7C 2S 3D JH"),
            Tableau.Create("4C QS 8S 6S 3S 5H"),
            Tableau.Create("2C 6D 4S 4H TS 8D"),
            Tableau.Create("KD 2D 5D AC 9H KH"),
            Tableau.Create("5C 9C QH 8H 2H 7D")
        ));

        [Fact]
        public void FromDealNum_returns_correct_result()
        {
            Assert.True(Board.FromDealNum(1) == _b1);
            Assert.True(Board.FromDealNum(2) == _b2);
        }

        [Fact]
        public void Traverse_traverses_states_backwards()
        {
            // Arrange
            var b = Board.FromDealNum(4);
            var c = b.ExecuteMove(Move.Get(MoveType.TableauToReserve, 0, 0));

            var i = 0;
            c.Traverse(n =>
            {
                if (i++ == 0)
                {
                    Assert.Same(c, n);
                }
                else
                {
                    Assert.Equal(b, n);
                }
            });
        }

        [Fact]
        public void AsJson_returns_json_string() => Assert.Equal(
            "[[41,49,7,12,11,21,23,],[5,48,51,16,37,31,32,],[34,35,33,39,15,29,6,],[40,19,45,46,38,47,22,],[17,1,43,14,30,20,],[26,44,3,0,4,9,],[24,50,2,13,42,28,],[18,10,8,27,25,36,],];",
            Board.FromDealNum(1).AsJson());

        [Fact]
        public void EmitCSharpCode_emits_cscode()
        {
            var writer = new StringWriter();
            Board
                .FromDealNum(1)
                .ExecuteMove(Move.Get(MoveType.TableauToReserve, 5, 0))
                .ExecuteMove(Move.Get(MoveType.TableauToReserve, 5, 1))
                .EmitCSharpCode(writer);
            Assert.Equal(
@"/*
CC DD HH SS
2C -- -- AS

00 01 02 03
3D -- -- --

00 01 02 03 04 05 06 07
-- -- -- -- -- -- -- --
JD 2D 9H JC 5D 7H 7C 5H
KD KC 9S 5S AD QC KH 3H
2S KS 9D QD JS    AH 3C
4C 5C TS QH 4H    4D 7S
3S TD 4S TH 8H    JH 7D
6D 8S 8D QS 6C    8C TC
6S 9C 2H 6H            
*/

var b = Board.Create(
	Reserve.Create(""3D""),
	Foundation.Create(Ranks.R2, Ranks.Nil, Ranks.Nil, Ranks.Ace),
	Tableaus.Create(
		Tableau.Create(""JD KD 2S 4C 3S 6D 6S""),
		Tableau.Create(""2D KC KS 5C TD 8S 9C""),
		Tableau.Create(""9H 9S 9D TS 4S 8D 2H""),
		Tableau.Create(""JC 5S QD QH TH QS 6H""),
		Tableau.Create(""5D AD JS 4H 8H 6C""),
		Tableau.Create(""7H QC""),
		Tableau.Create(""7C KH AH 4D JH 8C""),
		Tableau.Create(""5H 3H 3C 7S 7D TC"")
	)
);
if (!b.IsValid()) { throw new Exception(); }",
            writer.ToString());
        }
    }
}