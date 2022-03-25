using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonMessage.ProtocolFormat.PortFun;
using Grpc.Core;
namespace com.mirle.ibg3k0.sc.WebAPI.Grpc
{
    internal class Port : PortGreeter.PortGreeterBase
    {
        App.SCApplication app;
        public Port(App.SCApplication _app)
        {
            app = _app;
        }

        public override Task<replyPortInfo> getAllPortInfo(Empty empty, ServerCallContext context)
        {
            replyPortInfo result = new replyPortInfo();
            return Task.FromResult(result);
        }
    }
}
