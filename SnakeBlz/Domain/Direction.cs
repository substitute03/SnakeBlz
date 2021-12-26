namespace SnakeBlz.Domain
{
    public class Direction
    {
        public int Id { get; }
        public string Value { get; }

        public static Direction Up = new Direction(1, "Up");
        public static Direction Down = new Direction(2, "Down");
        public static Direction Left = new Direction(3, "Left");
        public static Direction Right = new Direction(4, "Right");
        public static Direction None = new Direction(0, "None");

        private Direction(int id, string value)
        {
            Id = id;
            Value = value;
        }

        public bool IsOppositeTo(Direction direction)
        {
            int sumOfIds = Id + direction.Id;

            return sumOfIds == 3 || sumOfIds == 7;
        }

        public bool IsEqualTo(Direction direction)
        {
            return Id == direction.Id;
        }

        public bool IsNotEqualTo(Direction direction)
        {
            return Id != direction.Id;
        }

        public static Direction FromKey(string key)
        {
            if (key == "w")
            {
                return Direction.Up;
            }
            else if (key == "a")
            {
                return Direction.Left;
            }
            else if (key == "s")
            {
                return Direction.Down;
            }
            else if (key == "d")
            {
                return Direction.Right;
            }

            return Direction.None;
        }
    }
}
