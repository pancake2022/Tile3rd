using System;

namespace CSFramework
{
    public struct LVBool
    {
        private bool _value;
        public bool Value
        {
            get => _value;
            set { _value = value; OnValueChanged?.Invoke(value); }
        }

        public Action<bool> OnValueChanged; 
    }

    public struct LVInt
    {
        private int _value;
        public int Value
        {
            get => _value;
            set { _value = value; OnValueChanged?.Invoke(value); }
        }

        public Action<int> OnValueChanged; 
    }

    public struct LVLong
    {
        private long _value;
        public long Value
        {
            get => _value;
            set { _value = value; OnValueChanged?.Invoke(value); }
        }

        public Action<long> OnValueChanged; 
    }

    public struct LVFloat
    {
        private float _value;
        public float Value
        {
            get => _value;
            set { _value = value; OnValueChanged?.Invoke(value); }
        }

        public Action<float> OnValueChanged; 
    }

    public struct LVDouble
    {
        private double _value;
        public double Value
        {
            get => _value;
            set { _value = value; OnValueChanged?.Invoke(value); }
        }

        public Action<double> OnValueChanged; 
    }

    public struct LVString
    {
        private string _value;
        public string Value
        {
            get => _value;
            set { _value = value; OnValueChanged?.Invoke(value); }
        }

        public Action<string> OnValueChanged; 
    }
}