using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Signature_Verifier
{
    public class ZipManager
    {
        public string? xlsmPath {  get; set; }
        public StatusCodes statusCodes { get; set; }
        private string signatureXML = "_xmlsignatures/sig1.xml";

        private List<XmlNode?> GetAllNodes(XmlNode? node)
        {
            List<XmlNode?> result = new List<XmlNode?>();

            if (node == null)
            {
                Console.WriteLine("Error with getting nodes, node was null");
                Environment.Exit(statusCodes.nullNode);
                return null;
            }

            result.Add(node);
            foreach (XmlNode childNode in node.ChildNodes)
            {
                result.Add(childNode);
            }
            return result;
        }

        private string randFileName(string workingDir)
        {
            Random random = new Random();
            int length = 7;
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string filename = new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
            return filename;
        }

        public (List<XmlNode?>, XmlDocument) getXMLNodes()
        {
            if (xlsmPath == null)
            {
                Console.WriteLine("XLSM Path cannot be null");
                Environment.Exit(statusCodes.nullXLSMPath);
            }
            using (ZipArchive zip = ZipFile.OpenRead(xlsmPath))
            {
                foreach (ZipArchiveEntry entry in zip.Entries.ToArray())
                {
                    Console.WriteLine(entry.Name);
                }
                ZipArchiveEntry? archiveEntry = zip.GetEntry(signatureXML);
                if (archiveEntry == null)
                {
                    Console.WriteLine("Error opening zip archive");
                    Environment.Exit(statusCodes.zipArchiveError);
                }

                string tempFileName = randFileName(Directory.GetCurrentDirectory());
                archiveEntry.ExtractToFile(tempFileName);
                XmlDocument signatureDocument = new XmlDocument();
                signatureDocument.Load(tempFileName);

                return (GetAllNodes(signatureDocument.DocumentElement), signatureDocument);
            }
        }
        public ZipManager()
        {

        }
    }
}
