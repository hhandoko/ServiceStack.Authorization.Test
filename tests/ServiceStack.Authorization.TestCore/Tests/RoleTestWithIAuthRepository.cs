// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoleTestWithIAuthRepository.cs" company="ServiceStack.Authorization.Test">
//   Copyright (c) ServiceStack.Authorization.Test contributors 2014
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ServiceStack.Authorization.Test
{
    using System.Linq;

    using NUnit.Framework;

    using ServiceStack.Auth;
    using ServiceStack.Data;
    using ServiceStack.Host;
    using ServiceStack.OrmLite;
    using ServiceStack.Testing;

    /// <summary>
    /// The LightSpeed ORM ServiceStack Auth manage roles test.
    /// </summary>
    [TestFixture]
    public class RoleTestWithIAuthRepository : AuthorizationTestBase
    {
        /// <summary>
        /// The role name for testing.
        /// </summary>
        private const string TestRoleName = "TestRole";

        /// <summary>
        /// The permission name for testing.
        /// </summary>
        private const string TestPermissionName = "TestPermission";

        /// <summary>
        /// Check role assignment in UserAuth table test.
        /// </summary>
        [Test]
        public void CheckRoleAssignmentInUserAuthTable()
        {
            using (var appHost = new BasicAppHost
            {
                ConfigureContainer = container =>
                {
                    container.Register<IDbConnectionFactory>(DbConnFactory);
                    container.Register<IAuthRepository>(c => new OrmLiteAuthRepository(c.Resolve<IDbConnectionFactory>()));
                }
            }.Init())
            {
                // Arrange
                UserAuth userAuth;
                var newRegistration = CreateNewUserRegistration();
                var request = new BasicRequest(newRegistration);
                var response = (RegisterResponse)appHost.ExecuteService(newRegistration, request);
                var authRepo = appHost.Resolve<IAuthRepository>();

                // Test #1: Check role and permission assignment
                // ---------------------------------------------
                // Act
                using (var db = DbConnFactory.Open())
                {
                    // Hydrate userAuth
                    userAuth = db.SingleById<UserAuth>(response.UserId);
                }

                var assignRoleRequest =
                    new AssignRoles
                    {
                        UserName = userAuth.UserName,
                        Roles = { TestRoleName },
                        Permissions = { TestPermissionName },
                    };

                userAuth = (UserAuth)authRepo.GetUserAuthByUserName(assignRoleRequest.UserName);
                authRepo.AssignRoles(userAuth, assignRoleRequest.Roles, assignRoleRequest.Permissions);

                // Assert: 
                // Check UserAuth to contain roles and permissions
                Assert.That(authRepo.GetRoles(userAuth).FirstOrDefault(), Is.EqualTo(TestRoleName));
                Assert.That(authRepo.GetPermissions(userAuth).FirstOrDefault(), Is.EqualTo(TestPermissionName));

                // Test #2: Check role and permission un-assignment
                // ------------------------------------------------
                // Act
                using (var db = DbConnFactory.Open())
                {
                    // Hydrate userAuth
                    userAuth = db.SingleById<UserAuth>(response.UserId);
                }

                var unassignRolesRequest =
                    new UnAssignRoles
                    {
                        UserName = userAuth.UserName,
                        Roles = { TestRoleName },
                        Permissions = { TestPermissionName },
                    };
                
                userAuth = (UserAuth)authRepo.GetUserAuthByUserName(assignRoleRequest.UserName);
                authRepo.UnAssignRoles(userAuth, unassignRolesRequest.Roles, unassignRolesRequest.Permissions);

                // Assert:
                // Check UserAuth not to contain roles and permissions above
                Assert.That(authRepo.GetRoles(userAuth).FirstOrDefault(), Is.Null);
                Assert.That(authRepo.GetPermissions(userAuth).FirstOrDefault(), Is.Null);
            }
        }

        /// <summary>
        /// Check role assignment in UserRole table test.
        /// </summary>
        [Test]
        public void CheckRoleAssignmentInUserRoleTable()
        {
            using (var appHost = new BasicAppHost
            {
                ConfigureContainer = container =>
                {
                    container.Register<IDbConnectionFactory>(DbConnFactory);
                    container.Register<IAuthRepository>(c =>
                        new OrmLiteAuthRepository(c.Resolve<IDbConnectionFactory>())
                            {
                                UseDistinctRoleTables = true
                            });
                }
            }.Init())
            {
                // Arrange
                UserAuth userAuth;
                var newRegistration = CreateNewUserRegistration();
                var request = new BasicRequest(newRegistration);
                var response = (RegisterResponse)appHost.ExecuteService(newRegistration, request);
                var authRepo = appHost.Resolve<IAuthRepository>();

                // Test #1: Check role and permission assignment
                // ---------------------------------------------
                // Act
                using (var db = DbConnFactory.Open())
                {
                    // Hydrate userAuth
                    userAuth = db.SingleById<UserAuth>(response.UserId);
                }

                var assignRoleRequest =
                    new AssignRoles
                    {
                        UserName = userAuth.UserName,
                        Roles = { TestRoleName },
                        Permissions = { TestPermissionName },
                    };

                userAuth = (UserAuth)authRepo.GetUserAuthByUserName(assignRoleRequest.UserName);
                authRepo.AssignRoles(userAuth, assignRoleRequest.Roles, assignRoleRequest.Permissions);

                // Assert: 
                // Check UserAuth to contain roles and permissions
                Assert.That(authRepo.GetRoles(userAuth).FirstOrDefault(), Is.EqualTo(TestRoleName));
                Assert.That(authRepo.GetPermissions(userAuth).FirstOrDefault(), Is.EqualTo(TestPermissionName));

                // Test #2: Check role and permission un-assignment
                // ------------------------------------------------
                // Act
                using (var db = DbConnFactory.Open())
                {
                    // Hydrate userAuth
                    userAuth = db.SingleById<UserAuth>(response.UserId);
                }

                var unassignRolesRequest =
                    new UnAssignRoles
                    {
                        UserName = userAuth.UserName,
                        Roles = { TestRoleName },
                        Permissions = { TestPermissionName },
                    };

                userAuth = (UserAuth)authRepo.GetUserAuthByUserName(assignRoleRequest.UserName);
                authRepo.UnAssignRoles(userAuth, unassignRolesRequest.Roles, unassignRolesRequest.Permissions);

                // Assert:
                // Check UserAuth not to contain roles and permissions above
                Assert.That(authRepo.GetRoles(userAuth).FirstOrDefault(), Is.Null);
                Assert.That(authRepo.GetPermissions(userAuth).FirstOrDefault(), Is.Null);
            }
        }
    }
}
