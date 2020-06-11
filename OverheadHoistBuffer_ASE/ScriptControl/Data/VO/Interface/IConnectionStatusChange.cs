using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.VO.Interface
{
    public interface IConnectionStatusChange
    {
        event EventHandler<bool> ConnectionStatusChange;
    }
}
