using ECommerce.Common.Helpers;

namespace NotificationListener.Configurations
{
    public class PolicySettings
    {
        public virtual RetryPolicySettings Retry { get; set; }

        public virtual CircuitBreakerPolicySettings CircuitBreaker { get; set; }
    }

    public class RetryPolicySettings
    {
        private const bool DEFAULT_ENABLED_RETRY = true;
        private static readonly IEnumerable<int> DEFAULT_RETRY_FOR_STATUS_CODES = Enumerable.Empty<int>();
        private const int DEFAULT_COUNT = 3;
        private const int DEFAULT_SLEEP_DURATION = 2 * 1000;

        public virtual string Enabled { get; set; }
        private bool? _enabledAsBoolean;
        public bool EnabledAsBoolean
        {
            get
            {
                if (_enabledAsBoolean is null)
                {
                    _enabledAsBoolean = Parser.ParseBoolean(Enabled, DEFAULT_ENABLED_RETRY);
                }
                return _enabledAsBoolean.Value;
            }
        }
        public virtual string? ForStatusCodes { get; set; }
        private IEnumerable<int>? _forStatusCodesAsArray;
        public IEnumerable<int> ForStatusCodesAsArray
        {
            get
            {
                if (_forStatusCodesAsArray is null)
                {
                    _forStatusCodesAsArray = string.IsNullOrEmpty(ForStatusCodes)
                        ? DEFAULT_RETRY_FOR_STATUS_CODES
                        : ForStatusCodes.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => Parser.ParseInt(x, -1));
                }
                return _forStatusCodesAsArray;
            }
        }

        public virtual string? Count { get; set; }
        private int? _countAsInteger;
        public int CountAsInteger
        {
            get
            {
                if (_countAsInteger is null)
                {
                    _countAsInteger = Parser.ParseInt(Count, DEFAULT_COUNT);
                }
                return _countAsInteger.Value;
            }
        }

        public virtual string? SleepDurationInMilliseconds { get; set; }
        private int? _sleepDurationInMillisecondsAsInteger;
        public int SleepDurationInMillisecondsAsInteger
        {
            get
            {
                if (_sleepDurationInMillisecondsAsInteger is null)
                {
                    _sleepDurationInMillisecondsAsInteger = Parser.ParseInt(SleepDurationInMilliseconds, DEFAULT_SLEEP_DURATION);
                }
                return _sleepDurationInMillisecondsAsInteger.Value;
            }
        }
    }


    public class CircuitBreakerPolicySettings
    {
        private const bool DEFAULT_ENABLED_CIRCUIT_BREAKER = true;
        private static readonly IEnumerable<int> DEFAULT_CIRCUIT_BREAKER_FOR_STATUS_CODES = Enumerable.Empty<int>();

        private const double DEFAULT_FAILURE_THRESHOLD = 0.6d;
        private const int DEFAULT_SAMPLING_DURATION = 10 * 1000;
        private const int DEFAULT_MINIMUM_THROUGHPUT = 20;
        private const int DEFAULT_DURATION_OF_BREAK = 30 * 1000;

        public virtual string? Enabled { get; set; }
        private bool? _enabledAsBoolean;
        public bool EnabledAsBoolean
        {
            get
            {
                _enabledAsBoolean ??= Parser.ParseBoolean(Enabled, DEFAULT_ENABLED_CIRCUIT_BREAKER);
                return _enabledAsBoolean.Value;
            }
        }

        public virtual string? ForStatusCodes { get; set; }
        private IEnumerable<int>? _forStatusCodesAsArray;
        public IEnumerable<int> ForStatusCodesAsArray
        {
            get
            {
                _forStatusCodesAsArray ??= string.IsNullOrWhiteSpace(ForStatusCodes)
                    ? DEFAULT_CIRCUIT_BREAKER_FOR_STATUS_CODES
                    : ForStatusCodes.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => Parser.ParseInt(x, -1));
                return _forStatusCodesAsArray;
            }
        }

        public virtual string? FailureThreshold { get; set; }
        private double? _failureThresholdAsDouble;
        public double FailureThresholdAsDouble
        {
            get
            {
                _failureThresholdAsDouble ??= Parser.ParseDouble(FailureThreshold, DEFAULT_FAILURE_THRESHOLD);
                return _failureThresholdAsDouble.Value;
            }
        }

        public virtual string? SamplingDurationInMilliseconds { get; set; }
        private int? _samplingDurationInMillisecondsAsInt;
        public int SamplingDurationInMillisecondsAsInt
        {
            get
            {
                _samplingDurationInMillisecondsAsInt ??= Parser.ParseInt(SamplingDurationInMilliseconds, DEFAULT_SAMPLING_DURATION);
                return _samplingDurationInMillisecondsAsInt.Value;
            }
        }

        public virtual string? MinimumThroughput { get; set; }
        private int? _minimumThroughputAsInt;
        public int MinimumThroughputAsInt
        {
            get
            {
                _minimumThroughputAsInt ??= Parser.ParseInt(MinimumThroughput, DEFAULT_MINIMUM_THROUGHPUT);
                return _minimumThroughputAsInt.Value;
            }
        }

        public virtual string? DurationOfBreakInMilliseconds { get; set; }
        private int? _durationOfBreakInMillisecondsAsInt;
        public int DurationOfBreakInMillisecondsAsInt
        {
            get
            {
                _durationOfBreakInMillisecondsAsInt ??= Parser.ParseInt(DurationOfBreakInMilliseconds, DEFAULT_DURATION_OF_BREAK);
                return _durationOfBreakInMillisecondsAsInt.Value;
            }
        }
    }
}
