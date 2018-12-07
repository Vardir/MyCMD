using System;
using System.Collections.Generic;

namespace Core.Continuations
{
    public struct Continuation<T>
    {
        private readonly IEnumerator<T> Source;
        
        public readonly bool CanContinue;
        public readonly ContinuationFailure Failure;
        public readonly string FailureMessage;

        public Continuation(IEnumerator<T> source)
        {
            Source = source;
            CanContinue = true;
            FailureMessage = null;
            Failure = ContinuationFailure.None;
        }
        private Continuation(bool canContinue, IEnumerator<T> source)
        {
            Source = source;
            FailureMessage = null;
            CanContinue = canContinue;
            Failure = ContinuationFailure.None;
        }
        private Continuation(ContinuationFailure failure, string failureMessage, IEnumerator<T> source)
        {
            Source = source;
            Failure = failure;
            CanContinue = false;
            FailureMessage = failureMessage;
        }
        private Continuation(bool canContinue, IEnumerator<T> source, ContinuationFailure failure, string failureMessage)
        {
            Source = source;
            Failure = failure;
            CanContinue = canContinue;
            FailureMessage = failureMessage;
        }

        public static Continuation<T> Empty() => new Continuation<T>(false, null);
        
        /// <summary>
        /// Fully copies the current continuation
        /// </summary>
        /// <returns></returns>
        public Continuation<T> Copy()
        {
            return new Continuation<T>(CanContinue, Source, Failure, FailureMessage);
        }
        /// <summary>
        /// Begins the continuation routine
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="onSuccess"></param>
        /// <param name="failureMessage"></param>
        /// <returns></returns>
        public Continuation<T> BeginWith(Func<T, bool> predicate, Action<T> onSuccess, string failureMessage)
        {
            if (Source.Current != null)
                throw new InvalidOperationException("can not begin continuation, it is already started");
            if (!Source.MoveNext())
                return FailWith(ContinuationFailure.EmptySource);
            return TryContinue(predicate, onSuccess, failureMessage);
        }
        /// <summary>
        /// Executes only if the previous continuation succeeded
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="onSuccess"></param>
        /// <param name="failureMessage"></param>
        /// <returns></returns>
        public Continuation<T> AndThen(Func<T, bool> predicate, Action<T> onSuccess, string failureMessage)
        {
            if (Source.Current == null || !Source.MoveNext())
                return FailWith(ContinuationFailure.SourceEnded);
            if (!CanContinue)
                return FailWith(ContinuationFailure.PreviousFailed, FailureMessage);
            return TryContinue(predicate, onSuccess, failureMessage);
        }
        /// <summary>
        /// Executes only if the previous continuation failed
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="onSuccess"></param>
        /// <param name="failureMessage"></param>
        /// <returns></returns>
        public Continuation<T> OrThen(Func<T, bool> predicate, Action<T> onSuccess, string failureMessage)
        {
            if (Source.Current == null || !Source.MoveNext())
                return FailWith(ContinuationFailure.SourceEnded);
            if (CanContinue)
                return Copy();
            return TryContinue(predicate, onSuccess, failureMessage);
        }
        /// <summary>
        /// Resets continuation if the previous was failed
        /// </summary>
        /// <returns></returns>
        public Continuation<T> OrReset()
        {
            if (CanContinue)
                return Copy();
            Source.Reset();
            return new Continuation<T>(true, Source);
        }
        /// <summary>
        /// Finishes continuation if it is reached it's ending
        /// </summary>
        /// <returns></returns>
        public Continuation<T> Break()
        {
            if (!CanContinue)
                return FailWith(Failure, FailureMessage);
            if (Source.Current == null)
                throw new InvalidOperationException("can not break empty continuation");
            if (Source.MoveNext())
                return FailWith(ContinuationFailure.SourceNotEnded);
            Source.Reset();
            return new Continuation<T>(Source);
        }
        /// <summary>
        /// Finishes executing of the continuation any way
        /// </summary>
        /// <returns></returns>
        public Continuation<T> Stop()
        {
            Source.Reset();
            return new Continuation<T>(Source);
        }
                
        private Continuation<T> TryContinue(Func<T, bool> predicate, Action<T> onSuccess, string failureMessage)
        {
            bool success = predicate(Source.Current);
            if (success)
            {
                onSuccess(Source.Current);
                return new Continuation<T>(true, Source);
            }
            else
                return FailWith(ContinuationFailure.PredicateFailed, failureMessage);
        }
        private Continuation<T> FailWith(ContinuationFailure failure, string failureMessage = null)
        {
            return new Continuation<T>(failure, failure.GetMessage(failureMessage), Source);
        }
    }
}