using System;
using System.Collections.Generic;
using System.Text;

namespace Millionaires
{
    public enum Stage
    {
        [StringValue("0 zł")]
        [Guaranteed(true)]
        Zero = 0,

        [StringValue("500 zł")]
        [Guaranteed(false)]
        First = 1,

        [StringValue("1000 zł [GWARANTOWANE]")]
        [Guaranteed(true)]
        Second = 2,

        [StringValue("2000 zł")]
        [Guaranteed(false)]
        Third = 3,

        [StringValue("5000 zł")]
        [Guaranteed(false)]
        Fourth = 4,

        [StringValue("10 000 zł")]
        [Guaranteed(false)]
        Fifth = 5,

        [StringValue("20 000 zł")]
        [Guaranteed(false)]
        Sixth = 6,

        [StringValue("40 000 zł [GWARANTOWANE]")]
        [Guaranteed(true)]
        Seventh = 7,

        [StringValue("75 000 zł")]
        [Guaranteed(false)]
        Eighth = 8,

        [StringValue("125 000 zł")]
        [Guaranteed(false)]
        Ninth = 9,

        [StringValue("250 000 zł")]
        [Guaranteed(false)]
        Tenth = 10,

        [StringValue("500 000 zł")]
        [Guaranteed(false)]
        Eleventh = 11,

        [StringValue("1 000 000 zł")]
        Twelfth = 12
    }

    public class StringValueAttribute : Attribute
    {
        public string Value { get; }

        public StringValueAttribute(string value)
        {
            Value = value;
        }
    }

    public class GuaranteedAttribute : Attribute
    {
        public bool Value { get; }

        public GuaranteedAttribute(bool value)
        {
            Value = value;
        }
    }
}
