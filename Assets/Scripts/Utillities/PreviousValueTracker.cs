public class PreviousValueTracker<T> where T : class
{
    public T Current { get; private set; }
    public T Previous { get; private set; }

    public bool Set(T newValue)
    {
        if (newValue == null || newValue == Current)
            return false;

        Previous = Current;
        Current = newValue;
        return true;
    }

    public bool ReturnToPrevious()
    {
        if (Previous == null)
            return false;

        // Fancy tupple logic, to switch the two values
        (Previous, Current) = (Current, Previous);
        return true;
    }

    public void Clear()
    {
        Current = null;
    }
}