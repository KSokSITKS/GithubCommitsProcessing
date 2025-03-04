namespace Domain.Exceptions
{
    public class ProgramGeneralException : Exception
    {
		public ProgramGeneralException(string message) 
			: base(message)
		{
		}

		public ProgramGeneralException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
