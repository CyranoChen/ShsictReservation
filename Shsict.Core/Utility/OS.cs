﻿using System;

namespace Shsict.Core.Utility
{
    public class OSInfo
    {
        public static string GetOS()
        {
            return Environment.OSVersion.VersionString;
        }
    }
}