﻿using System;

namespace Estuite
{
    public class SessionId
    {
        public SessionId(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentOutOfRangeException(nameof(value));
            Value = value;
        }

        public string Value { get; private set; }
    }
}