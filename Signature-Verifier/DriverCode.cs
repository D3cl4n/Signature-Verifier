using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.IO.Compression;
using System.Security.Cryptography.X509Certificates;

namespace Signature_Verifier
{    
    public class SignatureData
    {
        public string base64Certificate;
        public SignatureData(string b64Certificate)
        {
            this.base64Certificate = b64Certificate;
        }
    }
    public class HelperTools
    {
        public SignatureData LogSignatureDetails(XmlNodeList xmlNodes)
        {
            string certificate = "";

            if (xmlNodes.Count == 1 && xmlNodes[0].Name == "X509Data")
            {
                foreach (XmlNode child in xmlNodes[0].ChildNodes)
                {
                    if (child.Name == "X509Certificate")
                    {
                        certificate = child.InnerText;
                        SignatureData returnData = new SignatureData(certificate);
                        return returnData;
                    }
                }
            }
            return null;
        }
        public string GenFileName(string workingDir)
        {
            Random random = new Random();
            int length = 7;
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string filename = new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
            return filename;
        }
        public HelperTools() { }
    }

    public class SignatureVerifier
    {
        //get and return a list of all child nodes from a node
        private List<XmlNode?> GetAllNodes(XmlNode? node)
        {
            List<XmlNode?> result = new List<XmlNode?>();
            result.Add(node);
            foreach (XmlNode childNode in node.ChildNodes)
            {
                result.Add(childNode);
            }
            return result;
        }

        //This method is the main code for extracting metadata from the file
        private void ExtractInfo(string filePath)
        {
            string signatureDir = "_xmlsignatures/sig1.xml"; //the path to the signature xml file within the compressed .xlsm
            using (ZipArchive zip = ZipFile.OpenRead(filePath)) //open the .xlsm file as a ZipArchive
            {
                var signatureFiles = zip.GetEntry(signatureDir); //Load the specific xml file for the signature
                if (signatureFiles != null)
                {
                    HelperTools util = new HelperTools(); //outside utils I needed for the project
                    string tempFile = util.GenFileName(filePath); //make a random file and load the xml into it
                    signatureFiles.ExtractToFile(tempFile); //actually loading the file
                    XmlDocument signatureDoc = new XmlDocument(); //create an XMLDocument object with the signature xml file contents
                    signatureDoc.Load(tempFile); //loading in the contents from the temp file
                    List<XmlNode?> xmlNodes = GetAllNodes(signatureDoc.DocumentElement); //get all nodes 

                    foreach (XmlNode? xmlNode in xmlNodes)
                    {
                        if (xmlNode != null && xmlNode.Name == "KeyInfo") //the KeyInfo node contains all the signature details
                        {
                            XmlNodeList keyInfoChildren = xmlNode.ChildNodes;
                            util.LogSignatureDetails(keyInfoChildren);
                        }
                        else if (xmlNode != null && xmlNode.Name == "SignatureValue")
                        {
                            Console.WriteLine("here");
                            Console.WriteLine(xmlNode.InnerText);
                        }
                        else if (xmlNode != null && xmlNode.Name == "SignedInfo")
                        {
                            XmlNodeList xmlNodeList = xmlNode.ChildNodes;
                            foreach (XmlNode xmlNode2 in xmlNodeList)
                            {
                                Console.WriteLine(xmlNode2.Name);
                                if (xmlNode2.Name == "SignatureMethod")
                                {
                                    Console.WriteLine("here3");
                                    Console.WriteLine(xmlNode2.Attributes["Algorithm"]?.Value);
                                }
                            }
                        }
                    }
                    File.Delete(tempFile);
                }

                else
                {
                    Console.WriteLine("No digital signature found");
                }
            }
        }

        public void VerifySignature(string[] args)
        {
            string filePath = "D:\\CISA\\test.xlsm";
            ExtractInfo(filePath);
        }
    }
}
