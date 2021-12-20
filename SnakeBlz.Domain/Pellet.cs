using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace SnakeMobile.Domain.Model
{
    public class Pellet
    {
        public static readonly Color UnitColor = Color.Black;
        public Cell Cell { get; set; }

        public Pellet() { }

        public void Render()
        {
            Cell.Color = UnitColor;
        }
    }
}
