using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signature_Verifier
{
    public class StatusCodes
    {
        public int incorrectArgs = -1;
        public int nullXLSMPath = -2;
        public int zipArchiveError = -3;
        public int nullNode = -4;
        public StatusCodes() 
        { 
        
        }
    }
}
