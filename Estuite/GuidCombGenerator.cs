using System;
using Estuite.Domain;

namespace Estuite
{
    /// <summary>
    ///     Tribute to awesome NHibernate
    ///     https://github.com/nhibernate/nhibernate-core/blob/master/src/NHibernate/Id/GuidCombGenerator.cs
    /// </summary>
    public class GuidCombGenerator : IGenerateIdentities
    {
        private static readonly long BaseDateTicks = new DateTime(1900, 1, 1).Ticks;
        private readonly IProvideUtcDateTime _dateTime;

        public GuidCombGenerator(IProvideUtcDateTime dateTime)
        {
            _dateTime = dateTime;
        }

        public Guid Generate()
        {
            var guidArray = Guid.NewGuid().ToByteArray();

            var now = _dateTime.Now;

            // Get the days and milliseconds which will be used to build the byte string 
            var days = new TimeSpan(now.Ticks - BaseDateTicks);
            var msecs = now.TimeOfDay;

            // Convert to a byte array 
            // Note that SQL Server is accurate to 1/300th of a millisecond so we divide by 3.333333 
            var daysArray = BitConverter.GetBytes(days.Days);
            var msecsArray = BitConverter.GetBytes((long) (msecs.TotalMilliseconds/3.333333));

            // Reverse the bytes to match SQL Servers ordering 
            Array.Reverse(daysArray);
            Array.Reverse(msecsArray);

            // Copy the bytes into the guid 
            Array.Copy(daysArray, daysArray.Length - 2, guidArray, guidArray.Length - 6, 2);
            Array.Copy(msecsArray, msecsArray.Length - 4, guidArray, guidArray.Length - 4, 4);

            return new Guid(guidArray);
        }
    }
}