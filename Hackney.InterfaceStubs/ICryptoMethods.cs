using System;

namespace Hackney.InterfaceStubs
{
    public interface ICryptoMethods : IService
    {
        string Encrypt(string textToEncrypt);
        string Decrypt(string textToDecrypt);
        string EncryptionKey { set; }
    }
}
