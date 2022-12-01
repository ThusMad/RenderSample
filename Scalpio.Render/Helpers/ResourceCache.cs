//using System;
//using System.Collections.Generic;
//using SharpDX.Direct2D1;

//namespace Scalpio.Render.Helpers
//{
//    public class ResourceCache
//    {

//        private readonly Dictionary<string, Func<RenderTarget, object>> _generators = new Dictionary<string, Func<RenderTarget, object>>();
//        private readonly Dictionary<string, object> _resources = new Dictionary<string, object>();
//        private RenderTarget? _renderTarget;


//        public RenderTarget? RenderTarget
//        {
//            get => _renderTarget;
//            set { _renderTarget = value; UpdateResources(); }
//        }

//        public int Count => _resources.Count;

//        public object this[string key]
//        {
//            set
//            {
//                if (_resources.ContainsKey(key)) _resources[key] = value;
//            }
//            get => _resources[key];
//        }

//        public Dictionary<string, object>.KeyCollection Keys => _resources.Keys;

//        public Dictionary<string, object>.ValueCollection Values => _resources.Values;


//        public void Add(string key, Func<RenderTarget, object> gen)
//        {
//            if (_resources.TryGetValue(key, out var resOld))
//            {
//                Disposer.SafeDispose(ref resOld);
//                _generators.Remove(key);
//                _resources.Remove(key);
//            }

//            if (_renderTarget == null)
//            {
//                _generators.Add(key, gen);
//                _resources.Add(key, null);
//            }
//            else
//            {
//                var res = gen(_renderTarget);
//                _generators.Add(key, gen);
//                _resources.Add(key, res);
//            }
//        }

//        public void Clear()
//        {
//            foreach (var key in _resources.Keys)
//            {
//                var res = _resources[key];
//                Disposer.SafeDispose(ref res);
//            }

//            _generators.Clear();
//            _resources.Clear();
//        }

//        public bool ContainsKey(string key)
//        {
//            return _resources.ContainsKey(key);
//        }

//        public bool ContainsValue(object val)
//        {
//            return _resources.ContainsValue(val);
//        }

//        public Dictionary<string, object>.Enumerator GetEnumerator()
//        {
//            return _resources.GetEnumerator();
//        }

//        public bool Remove(string key)
//        {
//            if (_resources.TryGetValue(key, out var res))
//            {
//                Disposer.SafeDispose(ref res);
//                _generators.Remove(key);
//                _resources.Remove(key);
//                return true;
//            }
//            else
//            {
//                return false;
//            }
//        }

//        public bool TryGetValue(string key, out object? res)
//        {
//            return _resources.TryGetValue(key, out res);
//        }

//        private void UpdateResources()
//        {
//            if (_renderTarget == null) { return; }

//            foreach (var g in _generators)
//            {
//                var key = g.Key;
//                var gen = g.Value;
//                var res = gen(_renderTarget);

//                if (_resources.TryGetValue(key, out var resOld))
//                {
//                    Disposer.SafeDispose(ref resOld);
//                    _resources.Remove(key);
//                }

//                _resources.Add(key, res);
//            }
//        }
//    }
//}
