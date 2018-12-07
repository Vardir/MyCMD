namespace Core.Continuations
{
    public enum ContinuationFailure
    {
        None,
        EmptySource,
        SourceEnded,
        SourceNotEnded,
        PreviousFailed,
        PredicateFailed,
    }
}