using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoCS.Client
{
    public class CSSyntaxProvider
    {
        static List<string> tags = new List<string>();
        static List<char> specials = new List<char>();
        #region ctor
        static CSSyntaxProvider()
        {
            string[] strs = {
                "Array",
                "Assert",
                "Boolean",
                "DateTime",
                "Dictionary",
                "Enum",
                "Form",
                "Font",
                "List",
                "Math",
                "NaN",
                "String",
                "Test",
                "TestFixture",
                "XMLDocument",
                "boolean",
                "break",
                "byte",
                "case",
                "catch",
                "char",
                "class",
                "do",
                "double",
                "else",
                "false",
                "finally",
                "float",
                "function",
                "goto",
                "if",
                "int",
                "interface",
                "long",
                "namespace",
                "new",
                "null",
                "private",
                "public",
                "protected",
                "return",
                "select",
                "short",
                "switch",
                "throw",
                "true",
                "try",
                "using",
                "var",
                "void",
                "volatile",
                "while",
                "whiteSpace",
                "with",
                "write",
                "writeln"
            };
            tags = new List<string>(strs);

            char[] chrs = {
                '.',
                ')',
                '(',
                '[',
                ']',
                '>',
                '<',
                ':',
                ';',
                '\n',
                '\t'
            };
            specials = new List<char>(chrs);
        }
        #endregion
        public static List<char> GetSpecials
        {
            get { return specials; }
        }
        public static List<string> GetTags
        {
            get { return tags; }
        }
        public static bool IsKnownTag(string tag)
        {
            return tags.Exists(delegate(string s) { return s.ToLower().Equals(tag.ToLower()); });
        }
        public static List<string> GetJSProvider(string tag)
        {
            return tags.FindAll(delegate(string s) { return s.ToLower().StartsWith(tag.ToLower()); });
        }


    }
}
