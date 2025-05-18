namespace DiÆon.Exceptions;

internal class MultipleLifeTimeException:Exception
{
    private const string DefaultMessage = 
        "Multiple lifetime attributes have been applied to the same service. " +
        "A service must have only one lifetime declaration (e.g., Singleton, Scoped, or Transient).";

    public MultipleLifeTimeException() : base(DefaultMessage){}
}