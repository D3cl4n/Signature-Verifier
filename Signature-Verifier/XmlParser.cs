using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Signature_Verifier
{
    public class XmlParser
    {
        public MetadataValues metadata = new MetadataValues();
        public List<XmlNode?> xmlNodes {  get; set; }
        public XmlDocument document { get; set; }
        public MetadataValues parseXMLNodes()
        {
            string signatureMethod = String.Empty;
            string certificate = String.Empty;
            string signature = String.Empty;
            string referenceBytes = String.Empty;
            byte[] referenceData;

            foreach (XmlNode? node in xmlNodes)
            {
                Console.WriteLine(node.Name);
                if (node == null)
                {
                    continue;
                }
                if (node.Name == "SignedInfo")
                {
                    XmlNodeList childNodes = node.ChildNodes;
                    foreach (XmlNode childNode in childNodes)
                    {
                        Console.WriteLine(childNode.Name);
                        if (childNode.Name == "SignatureMethod")
                        {
                            signatureMethod = childNode.Attributes["Algorithm"]?.Value;
                        }
                        else if (childNode.Name == "Reference")
                        {
                            referenceBytes = childNode.Attributes["URI"].Value;
                            referenceBytes = referenceBytes.Substring(1);
                            XmlNode referenceNode = document.SelectSingleNode("//*[@Id='" + referenceBytes + "']");
                            referenceData = Encoding.UTF8.GetBytes(referenceNode.OuterXml);
                            metadata.hashedContents = referenceData;
                        }
                    }
                }
                else if (node.Name == "KeyInfo")
                {
                    XmlNodeList childNodes = node.ChildNodes;
                    foreach (XmlNode childNode in childNodes[0].ChildNodes)
                    {
                        if (childNode.Name == "X509Certificate")
                        {
                            certificate = childNode.InnerText;
                        }
                    }
                }
                else if (node.Name == "SignatureValue")
                {
                    signature = node.InnerText;
                }
            }
            metadata.algorithm = signatureMethod;
            metadata.certificate = certificate;
            metadata.signature = signature;

            return metadata;
        }
        public XmlParser() 
        { 
            
        }
    }
}
