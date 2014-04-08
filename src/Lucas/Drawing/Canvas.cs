using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace Lucas.Drawing
{
    public class Canvas
    {
        private int _lastKey = 0;
        private readonly Dictionary<int, IShape> _elements;
        private readonly Dictionary<string, ShapeDefinition> _definitions;

        public Canvas()
        {
            _elements = new Dictionary<int, IShape>();
            _definitions = new Dictionary<string, ShapeDefinition>();
        }

        public IShape this[int key]
        {
            get
            {
                IShape value;
                _elements.TryGetValue(key, out value);
                return value;
            }
            set
            {
                if (key > _lastKey)
                    _lastKey = key;

                _elements[key] = value;
            }
        }

        public int Count { get { return _elements.Count; } }

        public IQueryable<KeyValuePair<string, ShapeDefinition>> Definitions
        {
            get { return _definitions.AsQueryable(); }
        }

        public IQueryable<KeyValuePair<int, IShape>> Elements
        {
            get { return _elements.AsQueryable(); }
        }

        public int Add(IShape shape)
        {
            var key = Interlocked.Increment(ref _lastKey);

            _elements.Add(key, shape);

            return key;
        }

        public void Register<T>(Func<double[], T> constructor, Func<string> summary, Func<string> example)
            where T : IShape
        {
            if (constructor == null)
                throw new ArgumentNullException("constructor");

            _definitions[typeof(T).Name.ToLower()] = new ShapeDefinition
            {
                Constructor = constructor,
                Summary = summary,
                Example = example
            };
        }

        public int[] Keys
        {
            get { return _elements.Keys.ToArray(); }
        }

        public void Clear()
        {
            _elements.Clear();
        }

        public IShape Remove(int key)
        {
            IShape value;

            if (_elements.TryGetValue(key, out value))
                _elements.Remove(key);

            return value;
        }

        public bool TryParse(string s, out KeyValuePair<int?, IShape> result)
        {
            result = new KeyValuePair<int?, IShape>();

            var tokens = s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (tokens.Length == 0 || string.IsNullOrEmpty(tokens[0]))
                return false;

            int key;
            if (int.TryParse(tokens[0], out key))
            {
                result = new KeyValuePair<int?, IShape>(key, null);
                tokens = tokens.Skip(1).ToArray();
            }

            ShapeDefinition descriptor;

            if (!_definitions.TryGetValue(tokens[0].ToLower(), out descriptor))
                return false;

            var args = new double[tokens.Length - 1];

            for (var i = 0; i < args.Length; i++)
            {
                double.TryParse(tokens[i + 1], out args[i])
                    // Evaluate precondition. All arguments must be a double type
                    .Assert(true, () => Strings.ERROR_PRINT + string.Format(Strings.ERROR_ARGUMENT_TYPE, "double", tokens[i + 1], i + 1));
            }

            var shape = (IShape)descriptor.Constructor.DynamicInvoke(args);

            result = new KeyValuePair<int?, IShape>(result.Key, shape);

            return true;
        }
    }
}
