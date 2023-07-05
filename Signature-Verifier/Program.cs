using System;
using System.Runtime.CompilerServices;
using System.Xml;
using Signature_Verifier;

public class Program
{
    public static void Main(string[] args)
    {
        StatusCodes codes = new StatusCodes();
        MetadataValues metadataValues = new MetadataValues();
        ZipManager zipManager = new ZipManager();

        if (!(args.Length == 1))
        {
            Console.WriteLine("Usage: .\\Signature-Verifier.exe <xlsm-path>");
            Environment.Exit(codes.incorrectArgs);
        }

        zipManager.xlsmPath = args[0];
        zipManager.statusCodes = codes;
        Console.WriteLine($"Extracting metadata from: {zipManager.xlsmPath}");
        List<XmlNode?> nodes = zipManager.getXMLNodes();
        XmlParser xmlParser = new XmlParser();
        xmlParser.xmlNodes = nodes;
        metadataValues = xmlParser.parseXMLNodes();
        Console.WriteLine($"Certificate: {metadataValues.certificate}\n");
        Console.WriteLine($"Algorithm: {metadataValues.algorithm}\n");
        Console.WriteLine($"Signature: {metadataValues.signature}\n");

        SignatureVerification verification = new SignatureVerification(metadataValues, zipManager.xlsmPath);
        verification.verifySignature();
    }
}