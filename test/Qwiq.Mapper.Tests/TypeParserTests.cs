using System;
using System.Xml;
using Microsoft.Qwiq.Core.Tests;
using Should;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Qwiq.Mapper.Tests
{
    public abstract class TypeParserTestsContext : ContextSpecification
    {
        protected object Expected;
        protected object Actual;
        protected ITypeParser TypeParser;

        public override void Given()
        {
            TypeParser = new TypeParser();
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_an_enum_value : TypeParserTestsContext
    {
        public override void When()
        {
            Expected = Formatting.Indented;
            Actual = TypeParser.Parse("Indented", Formatting.None);
        }

        [TestMethod]
        public void enum_value_is_returned()
        {
            Assert.AreEqual(Expected, Actual);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_a_null_to_nonnullable_double : TypeParserTestsContext
    {
        public override void When()
        {
            Expected = 11d;
            Actual = TypeParser.Parse<double>(null, 11);
        }

        [TestMethod]
        public void default_value_is_returned()
        {
            Assert.AreEqual(Expected, Actual);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_an_invalid_string_to_nullable_double : TypeParserTestsContext
    {
        public override void When()
        {
            Expected = null;
            Actual = TypeParser.Parse(typeof(double?), (object)"blarg");
        }

        [TestMethod]
        public void default_value_is_returned()
        {
            Assert.AreEqual(Expected, Actual);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_an_invalid_string_to_nonnullable_double : TypeParserTestsContext
    {
        public override void When()
        {
            Expected = 0d;
            Actual = TypeParser.Parse(typeof(double), (object)"blarg");
        }

        [TestMethod]
        public void default_value_is_returned()
        {
            Assert.AreEqual(Expected, Actual);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_an_int64_string_to_int32 : TypeParserTestsContext
    {
        public override void When()
        {
            Expected = 0;
            Actual = TypeParser.Parse(typeof(int), (object)"0x7FFFFFFFFFFFFFFF");
        }

        [TestMethod]
        public void default_value_is_returned()
        {
            Assert.AreEqual(Expected, Actual);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_an_null_to_nonnullable_double : TypeParserTestsContext
    {
        public override void When()
        {
            Expected = 0d;
            Actual = TypeParser.Parse(typeof(double), (object)null);
        }

        [TestMethod]
        public void default_value_is_returned()
        {
            Assert.AreEqual(Expected, Actual);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_a_null_DateTime_to_nonnullable_DateTime : TypeParserTestsContext
    {
        public override void When()
        {
            Expected = new DateTime(2014, 1, 1);
            Actual = TypeParser.Parse(typeof(DateTime), null, Expected);
        }

        [TestMethod]
        public void default_value_is_returned()
        {
            Assert.AreEqual(Expected, Actual);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_an_empty_string_nonnullable_double : TypeParserTestsContext
    {
        public override void When()
        {
            Expected = 11d;
            Actual = TypeParser.Parse<double>("", 11);
        }

        [TestMethod]
        public void default_value_is_returned()
        {
            Assert.AreEqual(Expected, Actual);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_a_valid_nonnullable_double : TypeParserTestsContext
    {
        public override void When()
        {
            Expected = 7d;
            Actual = TypeParser.Parse<double>("7");
        }

        [TestMethod]
        public void value_is_parsed_as_double()
        {
            Assert.AreEqual(Expected, Actual);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_a_null_nullable_double : TypeParserTestsContext
    {
        public override void When()
        {
            Expected = null;
            Actual = TypeParser.Parse<double?>(null);
        }

        [TestMethod]
        public void value_is_null()
        {
            Assert.AreEqual(Expected, Actual);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_a_valid_nullable_double : TypeParserTestsContext
    {
        public override void When()
        {
            Expected = 7d;
            Actual = TypeParser.Parse<double?>("7");
        }

        [TestMethod]
        public void value_is_parsed_as_double()
        {
            Assert.AreEqual(Expected, Actual);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_a_valid_generic_nullable_double : TypeParserTestsContext
    {
        public override void When()
        {
            Expected = 7d;
            Actual = TypeParser.Parse<Nullable<double>>("7");
        }

        [TestMethod]
        public void value_is_parsed_as_double()
        {
            Assert.AreEqual(Expected, Actual);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_a_null_nullable_int : TypeParserTestsContext
    {
        public override void When()
        {
            Expected = null;
            Actual = TypeParser.Parse<int?>(null);
        }

        [TestMethod]
        public void value_is_null()
        {
            Assert.AreEqual(Expected, Actual);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_a_valid_nullable_int : TypeParserTestsContext
    {
        public override void When()
        {
            Expected = 7;
            Actual = TypeParser.Parse<int?>("7");
        }

        [TestMethod]
        public void value_is_parsed_as_int()
        {
            Assert.AreEqual(Expected, Actual);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_a_null_nullable_date : TypeParserTestsContext
    {
        public override void When()
        {
            Expected = null;
            Actual = TypeParser.Parse<DateTime?>(null);
        }

        [TestMethod]
        public void value_is_null()
        {
            Assert.AreEqual(Expected, Actual);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_a_valid_nullable_date : TypeParserTestsContext
    {
        public override void When()
        {
            Expected = DateTime.Today;
            Actual = TypeParser.Parse<DateTime?>(Expected.ToString());
        }

        [TestMethod]
        public void value_is_parsed_as_date()
        {
            Assert.AreEqual(Expected, Actual);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_a_null_nonnullable_string : TypeParserTestsContext
    {
        public override void When()
        {
            Expected = "";
            Actual = TypeParser.Parse<string>(null, "");
        }

        [TestMethod]
        public void value_is_empty_string()
        {
            Assert.AreEqual(Expected, Actual);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_a_valid_nonnullable_string : TypeParserTestsContext
    {
        public override void When()
        {
            Expected = "test string";
            Actual = TypeParser.Parse<string>((object)"test string");
        }

        [TestMethod]
        public void value_is_parsed_as_string()
        {
            Assert.AreEqual(Expected, Actual);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_a_null_nonnullable_int : TypeParserTestsContext
    {
        public override void When()
        {
            Expected = 14;
            Actual = TypeParser.Parse<int>(null, 14);
        }

        [TestMethod]
        public void value_is_default()
        {
            Assert.AreEqual(Expected, Actual);
        }
    }

    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class when_parsing_a_valid_nonnullable_int : TypeParserTestsContext
    {
        public override void When()
        {
            Expected = 7;
            Actual = TypeParser.Parse<int>((object)7);
        }

        [TestMethod]
        public void value_is_parsed_as_int()
        {
            Assert.AreEqual(Expected, Actual);
        }
    }
}

