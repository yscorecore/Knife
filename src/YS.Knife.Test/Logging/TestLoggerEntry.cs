﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace YS.Knife.Test.Logging
{
    public class TestLoggerEntry
    {
        public string CategoryName { get; set; }
        public LogLevel LogLevel { get; set; }
        public EventId EventId { get; set; }
        public Exception Exception { get; set; }
        public string Message { get; set; }
    }
}
