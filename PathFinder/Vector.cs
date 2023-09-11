using System.Collections.Specialized;

namespace PathFinder
{
    public class Vector
    {
        public int y, w;

        public Vector(int y, int w)
        {
            this.y = y;
            this.w = w;
        }

        public Vector Plus(Vector v) => new Vector(y + v.y, w + v.w);
        
        public Vector Plus(int vy, int vw) => new Vector(y + vy, w + vw);

        public Vector Negate() => new Vector(y * -1, w * -1);
        
        public Vector Copy() => new Vector(y, w);
        
        public void Move(Vector v)
        {
            y += v.y;
            w += v.w;
        }

        public void Move(int vy, int vw) => Move(new Vector(vy, vw));

        public override bool Equals(object obj)
        {
            if (obj is Vector v)
            {
                return v.y == y && v.w == w;
            }

            return false;
        }

        public override string ToString()
        {
            return $"({y}:{w})";
        }

        public static Vector Right => new Vector(0, 1);
        public static Vector Left => new Vector(0, -1);
        public static Vector Up => new Vector(1, 0);
        public static Vector Down => new Vector(-1, 0);
    }
}