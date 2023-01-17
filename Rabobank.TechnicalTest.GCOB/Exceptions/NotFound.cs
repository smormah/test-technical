using System;

namespace Rabobank.TechnicalTest.GCOB.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException()
        {
            
        }

        public NotFoundException(string message) : base(message)
        {
        }
    }
}
