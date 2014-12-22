// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorizationTestBase.cs" company="ServiceStack.Authorization.Test">
//   Copyright (c) ServiceStack.Authorization.Test contributors 2014
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ServiceStack.Authorization.Test
{
    using System;
    using System.IO;

    using NUnit.Framework;

    using ServiceStack.Auth;
    using ServiceStack.Data;
    using ServiceStack.OrmLite;
    using ServiceStack.OrmLite.Sqlite;

    /// <summary>
    /// The ServiceStack Authz test base class.
    /// </summary>
    public class AuthorizationTestBase
    {
        /// <summary>
        /// The test email domain.
        /// </summary>
        public const string EmailDomain = "test.com";

        /// <summary>
        /// Gets the database connection string.
        /// </summary>
        public static string DbConnStr { get; private set; }

        /// <summary>
        /// Gets the database connection factory.
        /// </summary>
        public static IDbConnectionFactory DbConnFactory { get; private set; }

        /// <summary>
        /// Test fixture setup.
        /// Run once before all tests in the fixture is run.
        /// </summary>
        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            // Set database connection string
            DbConnStr =
                string.Format(
                    "Data Source={0};Version=3;",
                    Path.GetFullPath(string.Format("{0}/Data/ss_authz.sqlite", TestContext.CurrentContext.WorkDirectory)));
            
            // Set database connection factory
            DbConnFactory =
                new OrmLiteConnectionFactory(
                    DbConnStr,
                    new SqliteOrmLiteDialectProvider());

            // Drop and recreate authz tables
            using (var db = DbConnFactory.Open())
            {
                db.DropTable<UserAuthRole>();
                db.DropTable<UserAuthDetails>();
                db.DropTable<UserAuth>();

                db.CreateTable<UserAuth>();
                db.CreateTable<UserAuthDetails>();
                db.CreateTable<UserAuthRole>();
            }
        }

        /// <summary>
        /// Test setup.
        /// Runs everytime before each test is run.
        /// </summary>
        [SetUp]
        public void TestSetup()
        {
        }

        /// <summary>
        /// Test fixture teardown.
        /// Run once after all tests in the fixture finished running.
        /// </summary>
        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
        }

        /// <summary>
        /// Test teardown.
        /// Runs everytime after each test is finished.
        /// </summary>
        [TearDown]
        public void TestTearDown()
        {
        }

        /// <summary>
        /// Create a new user registration.
        /// </summary>
        /// <param name="autoLogin">True to enable auto login.</param>
        /// <returns>The <see cref="Register"/>.</returns>
        public static Register CreateNewUserRegistration(bool? autoLogin = null)
        {
            var userId = Environment.TickCount % 10000;
            var newUserRegistration =
                new Register
                {
                    UserName = "UserName" + userId,
                    DisplayName = "DisplayName" + userId,
                    Email = string.Format("user{0}@{1}", userId, EmailDomain),
                    FirstName = "FirstName" + userId,
                    LastName = "LastName" + userId,
                    Password = "Password" + userId,
                    AutoLogin = autoLogin,
                };

            return newUserRegistration;
        }
    }
}
