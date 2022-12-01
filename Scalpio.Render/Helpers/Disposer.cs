using System;

namespace Scalpio.Render.Helpers {
    public static class Disposer {
        public static void SafeDispose<T>( ref T? resource ) where T : class {
            switch (resource)
            {
                case null:
                    return;
                case IDisposable disposer:
                    disposer.Dispose();
                    break;
            }

            resource = null;
        }
    }
}
