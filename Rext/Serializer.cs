using System;
using System.Collections.Generic;
using System.Text;

namespace Rext
{
    public interface ISerializer
    {
        string Serialize(object obj);
        T Deserialize<T>(string data);
    }
}
