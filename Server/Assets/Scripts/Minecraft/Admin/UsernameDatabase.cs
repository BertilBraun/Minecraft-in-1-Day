using System;
using System.Collections.Generic;

namespace Assets.Scripts.Minecraft.Admin
{
    public class UsernameDatabase
    {
        private static Dictionary<string, Guid> database = new Dictionary<string, Guid>();

        public static Guid Get(string name)
        {
            if (!database.ContainsKey(name))
                database.Add(name, Guid.NewGuid());
            return database[name];
        }
    }
}
