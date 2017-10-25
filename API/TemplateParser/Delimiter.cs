namespace API.TemplateParser
{
    public struct Delimiters
    {
        public static readonly string PlaceholderLeft = "<<";
        public static readonly string PlaceholderRight = ">>";
        public static readonly string ExpressionLeft = "<=";
        public static readonly string ExpressionRight = "=>";
        public static readonly string VariableLeft = "<\\$";
        public static readonly string VariableRight = "\\$>";
    }

    public struct Delimiter
    {
        public string Left;
        public string Right;
    }
}