using GetSubordinates;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GetSubordinatesTest
{
    [TestClass]
    public class GetSubordinatesTest
    {
        [TestMethod]
        public void ValidateFile_Path()
        {
            var manager = new UserRoleManager();
            string message = null;
            var result = manager.LoadUserRoleFromFile("Invalid/Path", ref message);
            Assert.IsFalse(result, "Should fail path validation");
            Assert.AreEqual(message, "Invalid file path.");
        }

        [TestMethod]
        [DeploymentItem(@"Resources\fail.json", "")]
        [DeploymentItem(@"Resources\pass.json", "")]
        [DataRow("fail.json", false)]
        [DataRow("pass.json", true)]
        public void ValidateFile_Deserialisation(string fileName, bool canDeserialise)
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var path = Path.Combine(basePath, fileName);

            var manager = new UserRoleManager();
            string message = null;
            var result = manager.LoadUserRoleFromFile(path, ref message);
            Assert.AreEqual(canDeserialise, result, "Should return correct deserialisation result");

            if (canDeserialise)
            {
                Assert.IsTrue(string.IsNullOrEmpty(message), "Should return no message if it succeded");
                Assert.IsTrue(manager.UserRoles != null, "Successfully load data");
                Assert.IsTrue(manager.Roles.Count > 0, "Successfully load roles");
                Assert.IsTrue(manager.Users.Count > 0, "Successfully load users");
            }
            else
            {
                Assert.IsTrue(message.StartsWith("Invalid character after parsing property name."), "Should return error message if it failed");
            }
        }

        [TestMethod]
        public void ValidateInput_UserRoleFields()
        {
            UserRoleManager manager = new UserRoleManager();

            var inputNoUser = new UserRole();
            inputNoUser.Roles.Add(new Role() { Id = 1, Name = "Manager", Parent = 0 });

            var inputNoRole = new UserRole();
            inputNoRole.Users.Add(new User() { Id = 1, Name = "Mike Manager", Role = 1 });

            var inputNoRoleID = new UserRole();
            inputNoRoleID.Roles.Add(new Role() { Name = "Manager", Parent = 0 });
            inputNoRoleID.Users.Add(new User() { Id = 1, Name = "Mike Manager", Role = 1 });

            var inputNoRoleName = new UserRole();
            inputNoRoleName.Roles.Add(new Role() { Id = 1, Parent = 0 });
            inputNoRoleName.Users.Add(new User() { Id = 1, Name = "Mike Manager", Role = 1 });

            var inputNoRoleParent = new UserRole();
            inputNoRoleParent.Roles.Add(new Role() { Id = 1, Name = "Manager" });
            inputNoRoleParent.Users.Add(new User() { Id = 1, Name = "Mike Manager", Role = 1 });

            var inputNoUserID = new UserRole();
            inputNoUserID.Roles.Add(new Role() { Id = 1, Name = "Manager", Parent = 0 });
            inputNoUserID.Users.Add(new User() { Name = "Mike Manager", Role = 1 });

            var inputNoUserName = new UserRole();
            inputNoUserName.Roles.Add(new Role() { Id = 1, Name = "Manager", Parent = 0 });
            inputNoUserName.Users.Add(new User() { Id = 1, Role = 1 });

            var inputNoUserRole = new UserRole();
            inputNoUserRole.Roles.Add(new Role() { Id = 1, Name = "Manager", Parent = 0 });
            inputNoUserRole.Users.Add(new User() { Id = 1, Name = "Mike Manager" });

            foreach (var inputData in new List<UserRole> { inputNoUser, inputNoRole, inputNoRoleID, inputNoRoleName, inputNoRoleParent, inputNoUserID, inputNoUserName, inputNoUserRole })
            {
                string message = null;
                manager.UserRoles = inputData;

                var result = manager.BuildUserRole(ref message);
                Assert.IsFalse(result, "Should fail input validation");
                Assert.AreEqual(message, "User data, Role date, Id/Name/Role/Parent fields must not be empty.");
            }
        }

        [TestMethod]
        public void ValidateInput_UserRoleDuplicates()
        {
            UserRoleManager manager = new UserRoleManager();

            var inputDupUser = new UserRole();
            inputDupUser.Roles.Add(new Role() { Id = 1, Name = "Manager", Parent = 0 });
            inputDupUser.Roles.Add(new Role() { Id = 1, Name = "Worker", Parent = 9 });
            inputDupUser.Users.Add(new User() { Id = 1, Name = "Mike Manager", Role = 1 });

            var inputDupRole = new UserRole();
            inputDupRole.Roles.Add(new Role() { Id = 1, Name = "Manager", Parent = 0 });
            inputDupRole.Users.Add(new User() { Id = 1, Name = "Mike Manager", Role = 1 });
            inputDupRole.Users.Add(new User() { Id = 1, Name = "Michelle Manager", Role = 1 });

            foreach (var inputData in new List<UserRole> { inputDupUser, inputDupRole })
            {
                string message = null;
                manager.UserRoles = inputData;
                var problem = inputData.Roles.Count > 1 ? "roles" : "users";

                var result = manager.BuildUserRole(ref message);
                Assert.IsFalse(result, "Should fail input validation");
                Assert.AreEqual(message, $"Multiple {problem} with ID(s) were found: 1.");
            }
        }

        [TestMethod]
        [DataRow(1, 4)]
        [DataRow(2, 0)]
        [DataRow(3, 2)]
        [DataRow(4, 3)]
        [DataRow(5, 0)]
        public void GetSubordinatesFromID(int userID, int subOrdinates)
        {
            var input = new UserRole();
            input.Roles.Add(new Role() { Id = 1, Name = "System Administrator", Parent = 0 });
            input.Roles.Add(new Role() { Id = 2, Name = "Location Manager", Parent = 1 });
            input.Roles.Add(new Role() { Id = 3, Name = "Supervisor", Parent = 2 });
            input.Roles.Add(new Role() { Id = 4, Name = "Employee", Parent = 3 });
            input.Roles.Add(new Role() { Id = 5, Name = "Trainer", Parent = 3 });

            input.Users.Add(new User() { Id = 1, Name = "Adam Admin", Role = 1 });
            input.Users.Add(new User() { Id = 2, Name = "Emily Employee", Role = 4 });
            input.Users.Add(new User() { Id = 3, Name = "Sam Supervisor", Role = 3 });
            input.Users.Add(new User() { Id = 4, Name = "Mary Manager", Role = 2 });
            input.Users.Add(new User() { Id = 5, Name = "Steve Trainer", Role = 5 });

            string message = null;
            UserRoleManager manager = new UserRoleManager();
            manager.UserRoles = input;
            manager.BuildUserRole(ref message);

            var result = manager.GetSubOrdinates(userID);
            Assert.AreEqual(subOrdinates, result.Count, "should have the specified number of subordinates");
            Assert.IsTrue(string.IsNullOrEmpty(message));

            switch (userID)
            {
                case 1:
                    Assert.IsTrue(result.Any(x => x.Id == 2));
                    Assert.IsTrue(result.Any(x => x.Id == 3));
                    Assert.IsTrue(result.Any(x => x.Id == 4));
                    Assert.IsTrue(result.Any(x => x.Id == 5));
                    break;
                case 4:
                    Assert.IsTrue(result.Any(x => x.Id == 3));
                    Assert.IsTrue(result.Any(x => x.Id == 2));
                    Assert.IsTrue(result.Any(x => x.Id == 5));
                    break;
                case 3:
                    Assert.IsTrue(result.Any(x => x.Id == 2));
                    Assert.IsTrue(result.Any(x => x.Id == 5));
                    break;
            }
        }
    }
}
