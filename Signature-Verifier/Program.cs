using System;
using Signature_Verifier;

public class Program
{
    public static void Main(string[] args)
    {
        SignatureVerifier handler = new SignatureVerifier();
        handler.VerifySignature(args);
    }
}