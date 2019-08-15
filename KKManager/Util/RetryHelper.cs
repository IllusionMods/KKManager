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
        public static async Task RetryOnExceptionAsync(Func<Task> operation, int times, TimeSpan delay, CancellationToken cancellationToken)
        {
            await RetryOnExceptionAsync<Exception>(times, delay, operation, cancellationToken);
        }

        public static async Task RetryOnExceptionAsync<TException>(
            int times, TimeSpan delay, Func<Task> operation, CancellationToken cancellationToken) where TException : Exception
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
                    if (ex is OperationCanceledException || attempts == times)
                        throw;

                    await CreateDelayForException(times, attempts, delay, ex, cancellationToken);
                }
            } while (true);
        }

        private static Task CreateDelayForException(int times, int attempts, TimeSpan delay, Exception ex, CancellationToken cancellationToken)
        {
            //var delay = IncreasingDelayInSeconds(attempts);
            Console.WriteLine($"Exception on attempt {attempts} of {times}. Will retry after sleeping for {delay}.", ex);
            return Task.Delay(delay, cancellationToken);
        }

        internal static int[] DelayPerAttemptInSeconds =
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