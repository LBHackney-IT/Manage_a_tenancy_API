using System;

namespace Hackney.Plugin.Crypto
{
    public class EmptyEncryptionKeyException : Exception
    {
        public EmptyEncryptionKeyException()
            : base("No encryption key has been set or it is empty.")
        {
        }
    }
}
