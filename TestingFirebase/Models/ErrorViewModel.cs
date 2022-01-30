using System;

namespace TestingFirebase.Models;//Miguel upgraded this to scoped namespaces -a C# 10 new feature

public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
