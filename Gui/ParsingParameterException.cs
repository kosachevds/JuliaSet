namespace Gui
{
    internal class ParsingParameterException : System.Exception
    {
        public string ParameterName { get; }

        public ParsingParameterException(string parameterName)
        {
            this.ParameterName = parameterName;
        }
    }
}
