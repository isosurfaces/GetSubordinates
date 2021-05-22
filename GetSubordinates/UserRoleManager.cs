using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GetSubordinates
{
    public class UserRoleManager {
        public UserRole UserRoles = new UserRole();
        public List<User> Users { get => UserRoles.Users; }
        public List<Role> Roles { get => UserRoles.Roles; }
        private Dictionary<int, List<int>> RoleHierarchy = new Dictionary<int, List<int>>();
        private Dictionary<int, List<int>> SubRoleIDCache = new Dictionary<int, List<int>>();

        public bool LoadUserRoleFromFile(string path, ref string errorMessage)
        {
            // Validate file

            if (!File.Exists(path))
            {
                errorMessage = "Invalid file path.";
                return false;
            }

            using (var file = File.OpenText(path))
            {
                var serializer = new JsonSerializer();
                try
                {
                    UserRoles = (UserRole)serializer.Deserialize(file, typeof(UserRole));
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                    return false;
                }
            }

            return BuildUserRole(ref errorMessage);
        }

        public bool BuildUserRole(ref string errorMessage)
        {
            // Validate input

            if (Users.Count == 0 || Users.Any(x => x.Id == int.MinValue || string.IsNullOrEmpty(x.Name) || x.Role == int.MinValue) ||
                Roles.Count == 0 || Roles.Any(x => x.Id == int.MinValue || string.IsNullOrEmpty(x.Name) || x.Parent == int.MinValue))
            {
                errorMessage = "User data, Role date, Id/Name/Role/Parent fields must not be empty.";
                return false;
            }

            var idDuplicates = Users.GroupBy(x => x.Id)
              .Where(g => g.Count() > 1)
              .Select(y => y.Key)
              .ToList();
            if (idDuplicates.Count > 0)
            {
                errorMessage = $"Multiple users with ID(s) were found: {string.Join(",", idDuplicates)}.";
                return false;
            }

            idDuplicates = Roles.GroupBy(x => x.Id)
              .Where(g => g.Count() > 1)
              .Select(y => y.Key)
              .ToList();
            if (idDuplicates.Count > 0)
            {
                errorMessage = $"Multiple roles with ID(s) were found: {string.Join(",", idDuplicates)}.";
                return false;
            }

            // Process input
            BuildRoleHierarchy();
            return true;
        }

        private void BuildRoleHierarchy()
        {
            if (Roles == null || Roles.Count == 0) return;
            var result = new Dictionary<int, List<int>>();

            // Build a parent-children dictionary for each parent role
            // This dictionary is used to compile uncached result
            foreach (var r in Roles)
            {
                var parentId = r.Parent;
                var childID = r.Id;

                if (!result.ContainsKey(parentId))
                {
                    result.Add(parentId, new List<int>());
                }
                result[parentId].Add(childID);
            }
            
            RoleHierarchy = result;
        }

        public List<User> GetSubOrdinates(int id)
        {
            var user = Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return null;

            var subRoleIDs = GetSubRoleIDs(user.Role);
            if (subRoleIDs == null) return null;

            return Users.Where(x => subRoleIDs.Contains(x.Role)).ToList();
        }

        private List<int> GetSubRoleIDs(int parentId)
        {
            // Get cached result from previous searches if available
            if (SubRoleIDCache.ContainsKey(parentId))
            {
                return SubRoleIDCache[parentId];
            }

            var result = new List<int>();

            if (!RoleHierarchy.ContainsKey(parentId))
            {
                // No Subordinates
                SubRoleIDCache.Add(parentId, result);
                return result;
            }

            // Given a list of parents, find all children
            // Given a list of children, find all grand children
            // Find one generation of children per Do loop
            var parentIDs = new List<int>() { parentId };
            do
            {
                // Get cached result from previous searches if available
                var parentsCached = SubRoleIDCache.Where(x => parentIDs.Contains(x.Key));
                var childrenIDsCached = parentsCached.SelectMany(x => x.Value).Except(result).ToList();

                // Compile uncached result from RoleHierarchy child-parent relationships
                parentIDs = parentIDs.Except(parentsCached.Select(x => x.Key)).ToList();
                var parentsUnCached = RoleHierarchy.Where(x => parentIDs.Contains(x.Key));
                var childrenIDsUnCached = parentsUnCached.SelectMany(x => x.Value).Except(result).ToList();

                result.AddRange(childrenIDsCached.Union(childrenIDsUnCached));

                // Do until there is no one left in the next generation
                parentIDs = childrenIDsUnCached.ToList();
            }
            while (parentIDs.Count > 0);

            // Add to cache
            SubRoleIDCache.Add(parentId, result);
            return result;
        }
    }

    public class UserRole
    {
        public List<User> Users = new List<User>();
        public List<Role> Roles = new List<Role>();
    }

    public class User
    {
        public int Id = int.MinValue;
        public string Name;
        public int Role = int.MinValue;
    }

    public class Role
    {
        public int Id = int.MinValue;
        public string Name;
        public int Parent = int.MinValue;
    }
}
