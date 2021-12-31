namespace SnakeBlz.Domain
{
    public class Direction
    {
        public int Id { get; }
        private List<string> Keys { get; }
        public string Value { get; }

        public static readonly Direction Up = new(1, new() { "w", "ArrowUp" }, "Up");
        public static readonly Direction Down = new(2, new() { "s", "ArrowDown" }, "Down");
        public static readonly Direction Left = new(3, new() { "a", "ArrowLeft" }, "Left");
        public static readonly Direction Right = new(4, new() { "d", "ArrowRight" }, "Right");
        public static readonly Direction None = new(0, new(), "None");
        public static readonly List<Direction> AllDirections = new() { Up, Down, Left, Right };

        private Direction(int id, List<string> keys, string value)
        {
            Id = id;
            Keys = keys;
            Value = value;
        }

        public bool IsOppositeTo(Direction direction)
        {
            if (direction == Direction.None || this == Direction.None)
            {
                return false;
            }

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
            return AllDirections.Find(d => d.Keys.Contains(key)) ?? Direction.None;
        }
    }
}
