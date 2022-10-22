using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using PaPl.SKS.DataAccess.Entities;
using PaPl.SKS.DataAccess.Interfaces.Exceptions;
using PaPl.SKS.DataAccess.Repository;
using PaPl.SKS.DataAccess.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaPl.SKS.DataAccess.Test
{
    class ParcelRepositoryTest
    { 
        public SqlParcelRepository parcelRepo;
        private  ILogger<SqlParcelRepository> testLogger;

        /// <summary>
        /// "However, we never try to mock DbContext or IQueryable. Doing so is difficult, cumbersome, and fragile. Don't do it."
        ///     -Mike O. Soft
        ///     (for further information: https://docs.microsoft.com/en-us/ef/core/testing/)
        /// </summary>

        public ParcelRepositoryTest()
        {
            testLogger = new NullLogger<SqlParcelRepository>();
            var options = new DbContextOptionsBuilder<SQLDataContext>()
           .UseInMemoryDatabase(databaseName: "TestingDatabase")
           .Options;
            var inMemoryDBContext = new SQLDataContext(options);

            parcelRepo = new SqlParcelRepository(inMemoryDBContext, testLogger);
            
        }

        [Test]
        public void Alpha_GetAllParcels_ListIsNull_Exception_Test()
        {
            ILogger<SqlParcelRepository> testLogger = new NullLogger<SqlParcelRepository>();
            var options = new DbContextOptionsBuilder<SQLDataContext>()
                .UseInMemoryDatabase(databaseName: "TestingDatabase")
                .Options;
            var inMemoryDBContext = new SQLDataContext(options);

            SqlParcelRepository repo = new(inMemoryDBContext, testLogger);

            Assert.Throws<DataException>(() => repo.GetAllParcels());

        }

        [Test]
        public void Create_Test()
        {

            ILogger<SqlParcelRepository> testLogger = new NullLogger<SqlParcelRepository>();
            var options = new DbContextOptionsBuilder<SQLDataContext>()
                .UseInMemoryDatabase(databaseName: "TestingDatabase")
                .Options;
            var inMemoryDBContext = new SQLDataContext(options);
            SqlParcelRepository repo = new(inMemoryDBContext, testLogger);

            Parcel parcelAddParcel = new()
            {
                TrackingId = "validTrackingId123",
            };

            repo.Create(parcelAddParcel);

            //Quering for data
            var resultParcel = inMemoryDBContext.Parcel
                .Single(b => b.TrackingId == "validTrackingId123");

            Assert.IsNotNull(resultParcel);
        }
        [Test]
        public void Create_Throws_Exception_Test()
        {

            ILogger<SqlParcelRepository> testLogger = new NullLogger<SqlParcelRepository>();
            var options = new DbContextOptionsBuilder<SQLDataContext>()
                .UseInMemoryDatabase(databaseName: "TestingDatabase")
                .Options;
            var inMemoryDBContext = new SQLDataContext(options);
            SqlParcelRepository repo = new(inMemoryDBContext, testLogger);

            Parcel parcelAddParcel = new();

            Assert.Throws<DataException>(() => repo.Create(parcelAddParcel));
        }

        [Test]
        public void Delete_ParcelNULL_ThrowsDataException_Test()
        {
            ILogger<SqlParcelRepository> testLogger = new NullLogger<SqlParcelRepository>();
            var options = new DbContextOptionsBuilder<SQLDataContext>()
                .UseInMemoryDatabase(databaseName: "TestingDatabase")
                .Options;
            var inMemoryDBContext = new SQLDataContext(options);
            SqlParcelRepository repo = new(inMemoryDBContext, testLogger);

            Assert.Throws<DataException>(() => repo.Delete("noID"));

        }


        [Test]
        public void Update_NoData_Exception_Test()
        {
            ILogger<SqlParcelRepository> testLogger = new NullLogger<SqlParcelRepository>();
            var options = new DbContextOptionsBuilder<SQLDataContext>()
                .UseInMemoryDatabase(databaseName: "TestingDatabase")
                .Options;
            var inMemoryDBContext = new SQLDataContext(options);
            SqlParcelRepository repo = new(inMemoryDBContext, testLogger);

            Assert.Throws<DataException>(() => repo.Update(new Parcel(), null));

        }

        [Test]
        public void GetAllParcels_Test()
        {
            ILogger<SqlParcelRepository> testLogger = new NullLogger<SqlParcelRepository>();
            var options = new DbContextOptionsBuilder<SQLDataContext>()
                .UseInMemoryDatabase(databaseName: "TestingDatabase")
                .Options;
            var inMemoryDBContext = new SQLDataContext(options);

            SqlParcelRepository repo = new(inMemoryDBContext, testLogger);

            Parcel parcel1 = new()
            {
                TrackingId = "validCode878"
            };

            Parcel parcel2 = new()
            {
                TrackingId = "validCode7979"
            };

            inMemoryDBContext.Parcel.Add(parcel1);
            inMemoryDBContext.Parcel.Add(parcel2);

            inMemoryDBContext.SaveChanges();

            var list = inMemoryDBContext.Parcel.ToList();

            Assert.AreEqual(list.Count, repo.GetAllParcels().Count());

        }

        
        [Test]
        public void GetParcelById_Test()
        {

            ILogger<SqlParcelRepository> testLogger = new NullLogger<SqlParcelRepository>();
            var options = new DbContextOptionsBuilder<SQLDataContext>()
                .UseInMemoryDatabase(databaseName: "TestingDatabase")
                .Options;
            var inMemoryDBContext = new SQLDataContext(options);

            SqlParcelRepository repo = new(inMemoryDBContext, testLogger);

            Parcel parcelAddParcel = new()
            {
                TrackingId = "validTrackingId123",
            };

            inMemoryDBContext.Parcel.Add(parcelAddParcel);

            Assert.IsNotNull(repo.GetParcelById("validTrackingId123"));

        }

        [Test]
        public void GetParcelById_NoData_Exception_Test()
        {

            ILogger<SqlParcelRepository> testLogger = new NullLogger<SqlParcelRepository>();
            var options = new DbContextOptionsBuilder<SQLDataContext>()
                .UseInMemoryDatabase(databaseName: "TestingDatabase")
                .Options;
            var inMemoryDBContext = new SQLDataContext(options);

            SqlParcelRepository repo = new(inMemoryDBContext, testLogger);

            Assert.Throws<DataException>(() => repo.GetParcelById("noParcel"));

        }
    }

}
