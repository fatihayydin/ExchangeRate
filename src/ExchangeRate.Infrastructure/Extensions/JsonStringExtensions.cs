﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ExchangeRate.Infrastructure.Extensions
{
    public static class JsonStringExtensions
    {
        public static string RemoveUnescapeCharacters(this string value)
        {
            value = Regex.Unescape(value); //almost there
            value = value.Remove(value.Length - 1, 1).Remove(0, 1); //remove first and last qoutes

            return value;
        }
    }
}
