using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Dynamic;
using Microsoft.Win32;
using MessagePack;
using System.Collections.Specialized;


namespace KKManager.Data.Game
{
    /// <summary>
    /// There's two scenarios where you may need an IllusionObject: when you know the game and when you don't.
    /// That is correct. You do not need to specify the game name.
    /// Obviously specifying a game gives you a more accurate IllusionObject, but it is not necessary for basic lookups of default values.
    /// Most Illusion games are the same with the sole outlier being PH (doesn't use MessagePack serialization)
    /// How to use; you can get an instance via two methods:
    /// IllusionObject assets = new IllusionObject(new GameName("name of game"));
    /// IllusionObject assets = new IllusionObject();
    /// if you do not specify the game, you can always call Acquire(new GameName("name of game")) at any time to convert it.
    /// Todo:
    /// Indexing IllusionObject[different GameName] after an IllusionObject already called Acquire(GameName) should specify that you'd like to use a specific method from another game.
    /// Creating all possible IllusionObjects on all possible games
    /// Would allow the potential call the method 'IllusionObject.merge(GameName)'. This would be nice for the CharEditor, as the user could convert between game versions (even different games if they wanted).
    /// Not planned but worth looking into:
    /// introduce version control: Would need access to multiple older chara cards and multiple versions of Assembly-CSharp.dll of the older versions of illusion games.
    /// </summary>
    public class IllusionObject : DynamicObject
    {

        public GameName gameName;
        public void GetAssemblies(GameName gameName, string gameInstallPath = "")
        {
            if (gameInstallPath.Equals(""))
            {
                gameInstallPath = Consts.GetInstallPath(gameName);
            }
            string assemblyPath = Path.Combine(gameInstallPath, $"{gameName}_Data", "Managed", "Assembly-CSharp.dll");
            try
            {
                Assembly assembly = Assembly.LoadFrom(assemblyPath);
                SortedDictionary<string, object> gameBlocks = BlockHelper.GetValidBlockNames(gameName);
                foreach (Type t in assembly.GetTypes())
                {
                    if (gameBlocks.ContainsKey(t.Name) || t.Name.Equals("ChaFile") || t.Name.Equals("ChaFileDefine"))
                    {
                        Console.WriteLine($"Found {gameName} {t.Name}: {t}");
                        this._instance[t.Name] = t;
                    }
                }
                Console.WriteLine($"{gameName} modules successfully loaded!");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error loading {gameName} modules: {e}");
            }
        }
        public void Acquire(GameName gameName)
        {
            this.gameName = gameName;
            string installPath = Consts.GetInstallPath(gameName);
            GetAssemblies(gameName);
        }


        private readonly Dictionary<string, object> _instance;
        public IllusionObject(GameName gameName)
        {
            _instance = new Dictionary<string, object>();
            Acquire(gameName);
        }
        public IllusionObject()
        {
            _instance = new Dictionary<string, object>();
        }

        public object this[string key]
        {
            get
            {
                if (_instance.TryGetValue(key, out object value))
                {
                    return value;
                }
                else
                {
                    return new IllusionObject();
                }
            }
            set
            {
                _instance[key] = value;
            }
        }

        public T Get<T>(string key) where T : class
        {
            if (_instance.TryGetValue(key, out object value))
            {
                if (value is T tValue)
                {
                    return tValue;
                }
                else if (value is IllusionObject childObject)
                {
                    return childObject.Get<T>();
                }
            }
            return null;
        }

        /*
        public T Get<T>(string key)
        {
            if (_instance.TryGetValue(key, out object value))
            {
                if (value is T tValue)
                {
                    return tValue;
                }
                else if (value is IllusionObject childObject)
                {
                    return childObject.Get<T>();
                }
            }
            return default(T);
        }*/


        public T Get<T>()
        {
            if (typeof(T) == typeof(IllusionObject))
            {
                return (T)(object)this;
            }
            else if (typeof(T) == typeof(Dictionary<string, object>))
            {
                return (T)(object)_instance;
            }
            else if (typeof(T) == typeof(object))
            {
                return (T)(object)_instance;
            }
            else if (typeof(T).IsClass)
            {
                var instance = Activator.CreateInstance(typeof(T));
                foreach (var property in typeof(T).GetProperties())
                {
                    if (property.CanWrite)
                    {
                        var value = Get<object>(property.Name);
                        if (value != null)
                        {
                            property.SetValue(instance, value);
                        }
                    }
                }
                return (T)instance;
            }
            else
            {
                return default(T);
            }
        }
        public void Add<T>(string key, T value) where T : class
        {
            this._instance[key] = value;
        }
        public object Call(string key, params object[] args)
        {
            if (_instance.TryGetValue(key, out object value) && value is Delegate del)
            {
                return del.DynamicInvoke(args);
            }
            return null;
        }

        public object GetPropertyValue(string objectName, string propertyName)
        {
            if (this._instance.TryGetValue(objectName, out object obj))
            {
                Type type = obj.GetType();
                PropertyInfo property = type.GetProperty(propertyName);
                if (property != null)
                {
                    return property.GetValue(obj);
                }
            }

            return null;
        }
    }
}
