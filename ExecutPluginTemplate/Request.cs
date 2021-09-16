﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExecutPluginTemplate
{
    /// <summary>
    /// First, this is the enumeration of each command triggered in the MainWindow (modeless dialog).
    /// </summary>
    public enum RequestId : int
    {
        None = 0,
        Command01 = 1,
        Command02 = 2,
        Command03 = 3,
        Command04 = 4
    }

    /// <summary>
    /// This is the class that will take the user command by the RequestId enum 
    /// and make the request once the RequestHandler identifies it while the external event is being raised.
    /// </summary>
    public class Request
    {
        private int m_request = (int)RequestId.None;

        public RequestId Take()
        {
            return (RequestId)Interlocked.Exchange(ref m_request, (int)RequestId.None);
        }

        public void Make(RequestId request)
        {
            Interlocked.Exchange(ref m_request, (int)request);
        }
    }
}
