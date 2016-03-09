using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Security
{
    public static class RSAAlgo
    {
        public static RSAParameters PrivateKey { get; set; }
        public static RSAParameters PublicKey { get; set; }

        static RSAAlgo()
        {
            RSACryptoServiceProvider rcsp = new RSACryptoServiceProvider(2048);
            PrivateKey = rcsp.ExportParameters(true);
            PublicKey = rcsp.ExportParameters(false);
        }

        public static string EncryptText(string plainTextData)
        {
            RSACryptoServiceProvider rcsp = new RSACryptoServiceProvider();
            rcsp.ImportParameters(PublicKey);

            //for encryption, always handle bytes...
            Byte[] bytesPlainTextData = System.Text.Encoding.Unicode.GetBytes(plainTextData);

            //apply pkcs#1.5 padding and encrypt our data 
            Byte[] bytesCypherText = rcsp.Encrypt(bytesPlainTextData, false);

            //we might want a string representation of our cypher text... base64 will do
            string cypherText = Convert.ToBase64String(bytesCypherText);

            return cypherText;
        }

        public static string DecryptText(string cypherText)
        {
            string plainTextData = string.Empty;
            //first, get our bytes back from the base64 string ...
            Byte[] bytesCypherText = Convert.FromBase64String(cypherText);

            //we want to decrypt, therefore we need a csp and load our private key
            RSACryptoServiceProvider rcsp = new RSACryptoServiceProvider();
            rcsp.ImportParameters(PrivateKey);

            //decrypt and strip pkcs#1.5 padding
            Byte[] bytesPlainTextData = rcsp.Decrypt(bytesCypherText, false);

            //get our original plainText back...
            plainTextData = System.Text.Encoding.Unicode.GetString(bytesPlainTextData);

            return plainTextData;
        }
    }
}
