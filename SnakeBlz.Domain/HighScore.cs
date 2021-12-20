using System;
using System.Collections.Generic;
using System.Text;

namespace SnakeMobile.Domain.Model
{
    public class HighScore
    {
        public int Id { get; set; }
        public string PlayerName { get; set; }
        public int Score { get; set; }
        public DateTime CreatedOnDateTime { get; set; }
    }
}
