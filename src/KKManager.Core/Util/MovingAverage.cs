using System.Collections.Generic;

namespace KKManager.Util
{
    public class MovingAverage
    {
        private readonly int _windowSize;
        private readonly Queue<long> _samples;
        private long _sampleAccumulator;

        public MovingAverage(int windowSize = 11)
        {
            _windowSize = windowSize;
            _samples = new Queue<long>(_windowSize + 1);
        }

        public long GetAverage()
        {
            return _samples.Count > 0 ? _sampleAccumulator / _samples.Count : 0;
        }

        public void Sample(long newSample)
        {
            _sampleAccumulator += newSample;
            _samples.Enqueue(newSample);

            if (_samples.Count > _windowSize)
                _sampleAccumulator -= _samples.Dequeue();
        }
    }
}