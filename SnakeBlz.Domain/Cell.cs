using System.ComponentModel;
using System.Drawing;

namespace SnakeMobile.Domain.Model
{
    public class Cell : INotifyPropertyChanged
    {
        public int PositionX { get; private set; }
        public int PositionY { get; private set; }
        public static readonly Color UnitColor = Color.LightGray;

        private Color _color = Color.LightGray;
        public Color Color
        {
            get => _color;
            set
            {
                _color = value;
                RaisePropertyChanged(nameof(Color));
            }
        }

        public Cell(int positionX, int positionY)
        {
            PositionX = positionX;
            PositionY = positionY;
        }

        public bool IsEmpty => Color == UnitColor;

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
