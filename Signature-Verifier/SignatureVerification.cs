using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Signature_Verifier
{
    public class SignatureVerification
    {
        public MetadataValues metadata;
        public string targetFile;

        private byte[]? computeExpectedHash()
        {
            byte[]? hash = null;
            string hashAlg = String.Empty;
            if (metadata.algorithm != null && (metadata.algorithm.Contains("sha256") || metadata.algorithm.Contains("SHA256")))
            {
                hashAlg = "SHA256";
                using (FileStream stream = File.OpenRead(targetFile))
                {
                    HashAlgorithm hashAlgorithm = SHA256.Create();
                    hash = hashAlgorithm.ComputeHash(stream);
                }
            }

            return hash;
        }
        public void verifySignature()
        {
            byte[] certificateData = Convert.FromBase64String(metadata.certificate);
            X509Certificate2 cert = new X509Certificate2(certificateData);
            RSA publicKey = cert.GetRSAPublicKey();
            byte[]? expectedHash = computeExpectedHash();
            Console.WriteLine($"Expected hash of xlsm: {expectedHash.ToString}");
        }

        public SignatureVerification(MetadataValues metadata, string filePath) 
        { 
            this.metadata = metadata;
            this.targetFile = filePath;
        }
    }
}
