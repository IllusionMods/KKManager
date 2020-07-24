using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KKManager.Util
{
    /// <summary>
    /// https://alastaircrabtree.com/implementing-the-retry-pattern-for-async-tasks-in-c/
    /// </summary>
    public static class RetryHelper
    {
        /// <summary>
        /// Exception that does not trigger retries
        /// </summary>
        public sealed class UnretryableException : Exception
        {
            internal UnretryableException(Exception ex) : base(string.Empty, ex)
            {
                if (ex == null) throw new ArgumentNullException(nameof(ex));
            }
        }

        /// <summary>
        /// Wrap the exception to make it not trigger any more retries
        /// </summary>
        public static UnretryableException DoNotAttemptToRetry(Exception originalException)
        {
            return new UnretryableException(originalException);
        }

        public static async Task RetryOnExceptionAsync(Func<Task> operation, int times, TimeSpan delay, CancellationToken cancellationToken)
        {
            await RetryOnSpecificExceptionAsync<Exception>(operation, times, delay, cancellationToken);
        }

        public static async Task RetryOnSpecificExceptionAsync<TException>(Func<Task> operation, int times, TimeSpan delay, CancellationToken cancellationToken) where TException : Exception
        {
            if (times <= 0)
                throw new ArgumentOutOfRangeException(nameof(times));

            var attempts = 0;
            do
            {
                try
                {
                    attempts++;
                    await operation();
                    break;
                }
                catch (TException ex)
                {
                    if (ex is OperationCanceledException || attempts >= times)
                        throw;

                    if (ex is UnretryableException c)
                        throw c.InnerException;

                    // Deal with exceptions caused by cancellation
                    cancellationToken.ThrowIfCancellationRequested();

                    await CreateDelayForException(times, attempts, delay, ex, cancellationToken);
                }
            } while (true);
        }

        private static Task CreateDelayForException(int times, int attempts, TimeSpan delay, Exception ex, CancellationToken cancellationToken)
        {
            //var delay = IncreasingDelayInSeconds(attempts);
            Console.WriteLine($"Crash on attempt {attempts} of {times}. Retrying after {delay}. Error: " + ex.Message);
            return Task.Delay(delay, cancellationToken);
        }

        private static int[] DelayPerAttemptInSeconds =
        {
            (int) TimeSpan.FromSeconds(2).TotalSeconds,
            (int) TimeSpan.FromSeconds(30).TotalSeconds,
            (int) TimeSpan.FromMinutes(2).TotalSeconds,
            (int) TimeSpan.FromMinutes(10).TotalSeconds,
            (int) TimeSpan.FromMinutes(30).TotalSeconds
        };

        private static int IncreasingDelayInSeconds(int failedAttempts)
        {
            if (failedAttempts <= 0) throw new ArgumentOutOfRangeException();

            return failedAttempts > DelayPerAttemptInSeconds.Length ? DelayPerAttemptInSeconds.Last() : DelayPerAttemptInSeconds[failedAttempts];
        }
    }
}