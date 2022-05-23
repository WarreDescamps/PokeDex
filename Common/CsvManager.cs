using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class CsvManager<T> where T : class
    {
        private readonly string _seperator = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
        private readonly string _path;

        private string _content = string.Empty;
        private List<T> _objects = new List<T>();

        public CsvManager(string path)
        {
            _path = path;
            Seed();
        }

        private void Seed()
        {
            if (File.Exists(_path))
                _objects = Read().ToList();
            if (_objects.Count <= 0)
                AddHeader();
            else
                _content = File.ReadAllText(_path);
        }

        private IEnumerable<T> Read()
        {
            if (!File.Exists(_path))
                yield break;
            using (StreamReader sr = new StreamReader(_path))
            {
                while (!sr.EndOfStream)
                {
                    string[] line = sr.ReadLine()?.Split(_seperator);
                    if (line.Length == 0 || line[0] == typeof(T).GetProperties().First().Name)
                        continue;
                    T item = (T)Activator.CreateInstance(typeof(T));
                    var props = item.GetType().GetProperties();
                    for (int i = 0; i < props.Length; i++)
                    {
                        var prop = props[i];
                        if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType) && prop.PropertyType != typeof(string))
                            continue;
                        if (typeof(DateTime).IsAssignableFrom(prop.PropertyType))
                        {
                            if (!DateTime.TryParse(line[i], out DateTime time))
                                continue;
                            prop.SetValue(item, time);
                            continue;
                        }
                        if (typeof(TimeSpan).IsAssignableFrom(prop.PropertyType))
                        {
                            if (!TimeSpan.TryParse(line[i], out TimeSpan time))
                                continue;
                            prop.SetValue(item, time);
                            continue;
                        }
                        prop.SetValue(item, line[i]);
                    }
                    yield return item;
                }
            }
        }

        private CsvManager<T> AddHeader()
        {
            foreach (var prop in typeof(T).GetProperties())
            {
                _content += prop.Name + _seperator;
            }
            _content = _content[0..^1] + Environment.NewLine;

            return this;
        }

        public CsvManager<T> AddObject(T item)
        {
            _objects.Add(item);
            foreach (var prop in item.GetType().GetProperties())
            {
                if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType) && prop.GetValue(item) is not string)
                {
                    foreach (var obj in prop.GetValue(item) as IEnumerable)
                    {
                        _content += obj + " ";
                    }
                    _content = _content.TrimEnd() + _seperator;
                }
                else
                    _content += prop.GetValue(item) + _seperator;
            }
            _content = _content[0..^1] + Environment.NewLine;

            return this;
        }

        public CsvManager<T> AddObjects(IEnumerable<T> objs)
        {
            _objects.AddRange(objs);
            foreach (T item in objs)
            {
                _content += AddObject(item);
            }

            return this;
        }

        public CsvManager<T> RemoveAll()
        {
            _objects = new List<T>();
            _content = string.Empty;
            AddHeader();

            return this;
        }

        public CsvManager<T> RemoveAll(Func<T, bool> predicate)
        {
            _objects.RemoveAll(new Predicate<T>(predicate));
            return this;
        }

        public CsvManager<T> TryRemoveObject(T obj)
        {
            if (_objects.Contains(obj))
                return this;
            _objects.Remove(obj);
            return this;
        }

        public CsvManager<T> UpdateObjs()
        {
            _objects = Read().ToList();
            return this;
        }

        public bool Write()
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(_path))
                {
                    sw.Write(_content);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public IReadOnlyList<T> Objects => _objects;
    }
}
