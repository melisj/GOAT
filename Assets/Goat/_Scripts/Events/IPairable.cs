namespace Goat.Events
{
    public interface IPairable<T, E>
    {
        T Item1 { get; set; }
        E Item2 { get; set; }

        void Deconstruct(out T item1, out E item2);
    }
}