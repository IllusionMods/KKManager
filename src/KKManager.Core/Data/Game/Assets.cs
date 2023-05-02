using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Dynamic;
//using Mono.Cecil;


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
            if (string.IsNullOrEmpty(gameInstallPath))
            {
                gameInstallPath = Consts.GetInstallPath(gameName);
            }
            string assemblyPath = Path.Combine(gameInstallPath, $"{gameName}_Data", "Managed", "Assembly-CSharp.dll");
            OrderedDictionary gameBlocks = BlockHelper.GetValidBlockNames(gameName);
            // using mono is always an option too.
            // AssemblyLoader loader = new AssemblyLoader();
            // loader.LoadAssemblies(new string[] { assemblyPath });
            /*foreach (TypeDefinition type in loader.GetTypesByAssemblyName("Assembly-CSharp"))
            {
                if (gameBlocks.ContainsKey(type.Name) || type.Name.Equals("ChaFile") || type.Name.Equals("ChaFileDefine"))
                {
                    Console.WriteLine($"Found {gameName} {type.Name}: {type}");
                    this._instance[type.Name] = type;
                }
            }*/

            // luckily reflection does. I did not want to create an entire AppDomain.
            try
            {
                Assembly assembly = Assembly.LoadFrom(assemblyPath);
                foreach (Type type in assembly.GetTypes())
                {
                    if (!type.IsClass)
                    {
                        continue;
                    }
                    if (
                        gameBlocks.Contains(type.Name)
                        || type.Name == "ChaFileDefine"
                        || type.Name == "ChaFile"
                    )
                    {
                        Console.WriteLine($"Found {gameName} {type.Name}: {type}");
                        if (type.IsAbstract && type.IsSealed)
                        {
                            _instance[type.Name] = type;
                        }
                        else
                        {
                            try
                            {
                                _instance[type.Name] = Activator.CreateInstance(type);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Failed to create instance of type {type.Name}: {ex}");
                            }
                        }
                    }
                }
                GC.Collect();
                Console.WriteLine($"{gameName} modules successfully loaded!");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to load assembly {assemblyPath}: {ex}");
            }
        }

        public void Acquire(GameName gameName)
        {
            this.gameName = gameName;
            GetAssemblies(gameName);
        }

        private readonly Dictionary<string, object> _instance;
        public IllusionObject(GameName gameName)
        {
            _instance = new Dictionary<string, object>();
            Acquire(gameName);
        }
        public IllusionObject() => _instance = new Dictionary<string, object>();
        public object this[string key]
        {
            get => _instance.TryGetValue(key, out object value) ? value : throw new KeyNotFoundException($"The key {key} was not found.");
            set => _instance[key] = value;
        }
        public T Get<T>(string key)
        {
            if (!_instance.TryGetValue(key, out object obj))
            {
                throw new KeyNotFoundException($"The key {key} was not found.");
            }

            switch (obj)
            {
                case T result:
                    return result;

                case Type type when typeof(T) != typeof(Type):
                    if (type.IsAbstract && type.IsSealed)
                    {
                        Console.WriteLine($"Getting static class {key} of type {type}");
                        return (T)(object)type;
                    }
                    else
                    {
                        throw new InvalidCastException($"The value associated with key {key} cannot be cast to type {typeof(T)}");
                    }

                case Type instanceType:
                    Console.WriteLine($"Getting instance of class {key} of type {instanceType}");
                    try
                    {
                        return (T)Activator.CreateInstance(instanceType);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"An error occurred while creating instance of class {key} of type {instanceType}.", ex);
                    }

                default:
                    if (typeof(T).IsAssignableFrom(obj.GetType()))
                    {
                        Console.WriteLine($"Getting instance of field/property {key} with type {typeof(T)}");
                        return (T)obj;
                    }
                    else
                    {
                        throw new InvalidCastException($"The value associated with key {key} cannot be cast to type {typeof(T)}");
                    }
            }
        }

        public T GetField<T>(string className, string fieldName)
        {
            if (!_instance.TryGetValue(className, out object obj))
            {
                throw new KeyNotFoundException($"The key {className} was not found or is not a valid class, field, or property.");
            }

            try
            {
                switch (obj)
                {
                    case Type type:
                        FieldInfo field = type.GetField(fieldName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                        if (field != null)
                        {
                            object value = field.GetValue(null);
                            if (value is T result2)
                            {
                                return result2;
                            }
                            else
                            {
                                throw new InvalidCastException($"The value of field {fieldName} in class {className} cannot be cast to type {typeof(T)}.");
                            }
                        }

                        PropertyInfo prop = type.GetProperty(fieldName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                        if (prop != null && prop.CanRead)
                        {
                            object value = prop.GetValue(null);
                            if (value is T result3)
                            {
                                return result3;
                            }
                            else
                            {
                                throw new InvalidCastException($"The value of property {fieldName} in class {className} cannot be cast to type {typeof(T)}.");
                            }
                        }

                        throw new KeyNotFoundException($"The field or property {fieldName} was not found in class {className}.");

                    case PropertyInfo property:
                        if (property.CanRead)
                        {
                            object value = property.GetValue(null);
                            if (value is T result4)
                            {
                                return result4;
                            }
                            else
                            {
                                throw new InvalidCastException($"The value of property {fieldName} in class {className} cannot be cast to type {typeof(T)}.");
                            }
                        }
                        else
                        {
                            throw new InvalidOperationException($"The property {fieldName} in class {className} is write-only.");
                        }

                    case FieldInfo field2:
                        object fieldValue = field2.GetValue(null);
                        if (fieldValue is T result)
                        {
                            return result;
                        }
                        else
                        {
                            throw new InvalidCastException($"The value of field {fieldName} in class {className} cannot be cast to type {typeof(T)}.");
                        }

                    default:
                        throw new System.Exception($"Unknown value type: {obj}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while getting the value of field or property {fieldName} in class {className}.", ex);
            }
        }


        public void SetField(string className, string fieldName, object value)
        {
            if (_instance.TryGetValue(className, out object obj))
            {
                if (obj is Type type)
                {
                    FieldInfo field = type.GetField(fieldName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    if (field != null)
                    {
                        field.SetValue(null, value);
                        return;
                    }

                    PropertyInfo prop = type.GetProperty(fieldName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    if (prop != null && prop.CanWrite)
                    {
                        prop.SetValue(null, value);
                        return;
                    }

                    throw new KeyNotFoundException($"The field or property {fieldName} was not found in class {className}.");
                }
                else if (obj is PropertyInfo property)
                {
                    if (property.CanWrite)
                    {
                        property.SetValue(null, value);
                    }
                }
                else if (obj is FieldInfo field)
                {
                    field.SetValue(null, value);
                }
                else
                {
                    throw new System.Exception($"Unknown value type: {value}");
                }
            }
            throw new KeyNotFoundException($"The key {className} was not found or is not a valid class, field, or property.");
        }
        public object Call(string className, string methodName, object[] parameters)
        {
            if (!_instance.TryGetValue(className, out object target))
            {
                throw new KeyNotFoundException($"The key {className} was not found.");
            }

            Type targetType = target.GetType();

            if (!targetType.IsClass)
            {
                throw new ArgumentException($"The key {className} does not correspond to a class.");
            }

            MethodInfo method = targetType.GetMethod(methodName);
            if (method == null)
            {
                throw new ArgumentException($"The method {methodName} does not exist in class {className}.");
            }

            if (method.GetParameters().Length > 0 && (parameters == null || parameters.Length == 0))
            {
                throw new ArgumentException($"The method {methodName} requires parameters but none were provided.");
            }

            try
            {
                return method.Invoke(target, parameters);
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    throw new Exception($"An error occurred while invoking method {methodName} on class {className}: {ex.InnerException.Message}", ex.InnerException);
                }
                else
                {
                    throw new Exception($"An error occurred while invoking method {methodName} on class {className}.", ex);
                }
            }
        }



        public object Call(Type classType, string methodName, object[] parameters)
        {
            if (classType is null)
            {
                throw new ArgumentNullException(nameof(classType), "The class type cannot be null.");
            }

            if (!classType.IsClass)
            {
                throw new ArgumentException($"The {nameof(classType)} argument does not correspond to a class.");
            }

            MethodInfo method = classType.GetMethod(methodName);
            if (method is null)
            {
                throw new ArgumentException($"The method {methodName} does not exist in class {classType.Name}.");
            }

            if (method.GetParameters().Length > 0 && (parameters is null || parameters.Length == 0))
            {
                throw new ArgumentException($"The method {methodName} requires parameters but none were provided.");
            }

            object target = null;
            if (!method.IsStatic)
            {
                if (classType.IsAbstract && classType.IsSealed)
                {
                    throw new ArgumentException($"The class {classType.Name} is static and cannot be instantiated.");
                }

                if (classType.GetConstructor(Type.EmptyTypes) != null)
                {
                    target = Activator.CreateInstance(classType);
                }
                else
                {
                    throw new ArgumentException($"The class {classType.Name} does not have a public parameterless constructor and cannot be instantiated.");
                }
            }

            try
            {
                return method.Invoke(target, parameters);
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    throw new Exception($"An error occurred while invoking method {methodName} on class {classType.Name}: {ex.InnerException.Message}", ex.InnerException);
                }
                else
                {
                    throw new Exception($"An error occurred while invoking method {methodName} on class {classType.Name}.", ex);
                }
            }
        }



        public bool Contains(string className, string memberName, bool isStatic = false)
        {
            if (!_instance.TryGetValue(className, out object obj))
            {
                return false;
            }

            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic;
            if (isStatic)
            {
                flags |= BindingFlags.Static;
            }
            else
            {
                flags |= BindingFlags.Instance;
            }


            Type type = obj.GetType();
            if (type.GetField(memberName, flags) != null)
            {
                return true;
            }

            PropertyInfo property = type.GetProperty(memberName, flags);
            if (property != null)
            {
                if ((isStatic ? property.GetGetMethod(true) : property.GetGetMethod()) != null && (isStatic ? property.GetGetMethod(true) : property.GetGetMethod()).IsStatic == isStatic)
                {
                    return true;
                }
            }

            return false;
        }

        public void Add<T>(string key, T value) where T : class
        {
            _instance[key] = value;
        }
    }
}
