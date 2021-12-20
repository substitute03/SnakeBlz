namespace SnakeMobile.Domain.Model
{
    public class Direction
    {
        public int Id { get; }
        public string Value { get; }

        public static Direction Up = new Direction(1, "Up");
        public static Direction Down = new Direction(2, "Down");
        public static Direction Left = new Direction(3, "Left");
        public static Direction Right = new Direction(4, "Right");

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
    }
}
