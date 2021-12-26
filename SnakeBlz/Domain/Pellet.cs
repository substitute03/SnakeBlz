using SnakeBlz.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace SnakeBlz.Domain
{
    public class Pellet
    {
        public static readonly Color UnitColor = Color.Black;
        public CellComponent Cell { get; set; }

        public Pellet() { }

        //public void Render()
        //{
        //    Cell.Color = UnitColor;
        //}
    }
}
