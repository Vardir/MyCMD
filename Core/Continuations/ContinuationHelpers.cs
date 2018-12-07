using System;
using System.Collections.Generic;

namespace Core.Continuations
{
    public static class Continuation
    {
        public static string GetMessage(this ContinuationFailure failure, string expectedMessage)
        {
            if (string.IsNullOrEmpty(expectedMessage))
            {
                switch (failure)
                {
                    case ContinuationFailure.EmptySource:
                        return "source is empty";
                    case ContinuationFailure.PredicateFailed:
                        return "continuation predicate failed";
                    case ContinuationFailure.PreviousFailed:
                        return "previous continuation failed";
                    case ContinuationFailure.SourceEnded:
                        return "source is ended";
                    case ContinuationFailure.SourceNotEnded:
                        return "source is not ended";
                    case ContinuationFailure.None:
                        return null;
                }
            }
            return expectedMessage;
        }
        public static string GetMessageOn(this ContinuationFailure failure,
                                          string onEmpty = null, string onPredicateFailed = null, 
                                          string onPreviousFailed = null,
                                          string onEnded = null, string onNotEnded = null,
                                          string defaults = null)
        {
            switch (failure)
            {
                case ContinuationFailure.EmptySource:
                    return failure.GetMessage(onEmpty ?? defaults);
                case ContinuationFailure.PredicateFailed:
                    return failure.GetMessage(onPredicateFailed ?? defaults);
                case ContinuationFailure.PreviousFailed:
                    return failure.GetMessage(onPreviousFailed ?? defaults);
                case ContinuationFailure.SourceEnded:
                    return failure.GetMessage(onEnded ?? defaults);
                case ContinuationFailure.SourceNotEnded:
                    return failure.GetMessage(onNotEnded ?? defaults);
                case ContinuationFailure.None:
                default:
                    return null;
            }
        }

        public static Continuation<T> Of<T>(IEnumerator<T> source)
        {
            if (source == null)
                throw new ArgumentNullException();
            return new Continuation<T>(source);
        }
    }
}