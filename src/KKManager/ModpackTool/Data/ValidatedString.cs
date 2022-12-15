using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace KKManager.ModpackTool
{
    public class ValidatedString : ValidatedStringWrapper
    {
        public ValidatedString(Func<string, bool> verifyValue) : this(string.Empty, verifyValue)
        { }

        public ValidatedString(string initialValue, Func<string, bool> verifyValue) : base(verifyValue)
        {
            _value = initialValue ?? throw new ArgumentNullException(nameof(initialValue));
            _isValid = verifyValue(initialValue);
        }

        private string _value;
        private bool _isValid;

        public override event PropertyChangedEventHandler PropertyChanged;

        public override string Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;

                    var newValid = VerifyValue(value);
                    if (_isValid != newValid)
                    {
                        _isValid = newValid;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsValid)));
                    }
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
                }
            }
        }

        public override bool IsValid
        {
            get => _isValid;
        }

        public static void Bind(BindingSource bindTarget, string stringPropName, Control inputTextbox, Label passfailLabel, DataSourceUpdateMode updateMode = DataSourceUpdateMode.OnPropertyChanged)
        {
            if (bindTarget == null) throw new ArgumentNullException(nameof(bindTarget));
            if (stringPropName == null) throw new ArgumentNullException(nameof(stringPropName));
            if (inputTextbox == null) throw new ArgumentNullException(nameof(inputTextbox));
            if (passfailLabel == null) throw new ArgumentNullException(nameof(passfailLabel));

            inputTextbox.DataBindings.Add(nameof(Control.Text), bindTarget, stringPropName + ".Value", false, updateMode);

            var binding = new Binding(nameof(Label.Text), bindTarget, stringPropName + ".IsValid", true, DataSourceUpdateMode.Never);
            binding.Format += (sender, args) => args.Value = (bool)args.Value ? "PASS" : "FAIL";
            passfailLabel.DataBindings.Add(binding);
            var binding1 = new Binding(nameof(Label.BackColor), bindTarget, stringPropName + ".IsValid", true, DataSourceUpdateMode.Never);
            binding1.Format += (sender, args) => args.Value = (bool)args.Value ? Color.DarkGreen : Color.DarkRed;
            passfailLabel.DataBindings.Add(binding1);
            passfailLabel.ForeColor = Color.White;
        }
    }
}