namespace Endure
{
    public class Pair<TFirst, TSecond>
    {
        public Pair()
        {
        }

        public Pair(TFirst first, TSecond second)
        {
            this.First = first;
            this.Second = second;
        }

        public TFirst First { get; set; }
        public TSecond Second { get; set; }
    };

    public class Trupele<T, U, O>
    {
        public Trupele()
        {
        }

        public Trupele(T first, U second, O third)
        {
            this.First = first;
            this.Second = second;
            this.Third = third;
        }

        public T First { get; set; }
        public U Second { get; set; }
        public O Third { get; set; }
    }
}
