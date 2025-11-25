using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab04_4.MyForms.SpatialQuery.Helpers
{
    /// <summary>
    /// COM对象包装器，自动释放资源
    /// </summary>
    public class ComObjectWrapper<T> : IDisposable where T : class
    {
        public T Object { get; private set; }

        public ComObjectWrapper(T comObject)
        {
            Object = comObject;
        }

        public void Dispose()
        {
            if (Object != null)
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(Object);
                Object = null;
            }
        }
    }
}
