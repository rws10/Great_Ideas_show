using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.CompilerServices;

namespace IdeaSite.Models
{
    public class LogHelper
    {
        public static log4net.ILog GetLogger([CallerFilePath]string filename= "")
        {
            return log4net.LogManager.GetLogger(filename);
        }
    }
}