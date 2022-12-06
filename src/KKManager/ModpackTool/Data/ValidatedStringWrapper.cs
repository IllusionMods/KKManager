using System;
using System.ComponentModel;
using KKManager.Util;
using KKManager.Windows;

namespace KKManager.ModpackTool;

public class ValidatedStringWrapper : INotifyPropertyChanged
{
    public static implicit operator string(ValidatedStringWrapper w) => w.Value;

    protected ValidatedStringWrapper(Func<string, bool> verifyValue)
    {
        VerifyValue = verifyValue ?? throw new ArgumentNullException(nameof(verifyValue));
    }
    public ValidatedStringWrapper(Action<string> set, Func<string> get, Func<string, bool> verifyValue) : this(verifyValue)
    {
        _set = set ?? throw new ArgumentNullException(nameof(set));
        _get = get ?? throw new ArgumentNullException(nameof(get));
    }

    private readonly Action<string> _set;
    private readonly Func<string> _get;
    protected readonly Func<string, bool> VerifyValue;

    public virtual event PropertyChangedEventHandler PropertyChanged;

    public virtual string Value
    {
        get => _get();
        set
        {
            //if (Value != value)
            {
                var prevValid = VerifyValue(value);
                _set(value);
                OnPropertyChanged(nameof(Value));
                if (prevValid != IsValid)
                    OnPropertyChanged(nameof(IsValid));
            }
        }
    }

    private void OnPropertyChanged(string propertyName)
    {
        // Prevent issues if values are changed outside main thread
        MainWindow.Instance.SafeInvoke(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)));
    }

    public virtual bool IsValid => VerifyValue(_get());//todo handle needs check values

    public override string ToString() => Value ?? "";
}
