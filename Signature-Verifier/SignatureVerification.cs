﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Signature_Verifier
{
    public class SignatureVerification
    {
        public MetadataValues metadata;
        public string targetFile;

        private string byteArrToHex(byte[] arr)
        {
            return BitConverter.ToString(arr);
        }

        private byte[]? computeActualHash()
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

            if (publicKey == null)
            {
                Console.WriteLine("Null pub key");
            }
            byte[]? actualHash = computeActualHash();
            string hexHash = byteArrToHex(actualHash).Replace("-", "");
            Console.WriteLine($"Actual hash of xlsm: {hexHash}");
            byte[]? digitalSignature = Convert.FromBase64String(metadata.signature);
            Console.WriteLine(metadata.hashedContents.Length);

            try
            {
                bool isValid = publicKey.VerifyData(metadata.hashedContents, digitalSignature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                Console.WriteLine(isValid);
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public SignatureVerification(MetadataValues metadata, string filePath) 
        { 
            this.metadata = metadata;
            this.targetFile = filePath;
        }
    }
}
