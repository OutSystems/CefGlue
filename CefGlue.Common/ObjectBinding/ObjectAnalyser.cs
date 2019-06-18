using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Xilium.CefGlue.Common.ObjectBinding
{
    internal class ObjectAnalyser
    {
        public void AnalyseObjectMembers(object obj)
        {
            var methods = obj.GetType().GetMethods();
        }
    }
}
