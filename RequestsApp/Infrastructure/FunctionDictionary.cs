using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text.Json;

namespace RequestsApp.Infrastructure
{
    public class FunctionDictionary
    {
        private readonly Dictionary<string, Func<dynamic, dynamic, dynamic>> _functions = new Dictionary<string, Func<dynamic, dynamic, dynamic>>();     
        
        public int Count => _functions.Count;

        public bool ContainsKey(string key) => _functions.ContainsKey(key);

        public void Add<dynamic>(string key, Func<dynamic, dynamic, dynamic> function) 
        {
            _functions.Add(key, function);
        }
        
        public Func<T1, T2, dynamic> Function<T1, T2, dynamic>(string key)    
        {
            return (Func<T1, T2, dynamic>)_functions[key];
        }
       
    }
}