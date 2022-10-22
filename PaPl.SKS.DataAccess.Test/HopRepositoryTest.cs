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
    class HopRepositoryTest
    {

        public HopRepositoryTest()
        {
            /*
            testLogger = new NullLogger<HopRepositoryTest>();
            var options = new DbContextOptionsBuilder<SQLDataContext>()
                .UseInMemoryDatabase(databaseName: "TestingDatabase")
                .Options;
            var inMemoryDBContext = new SQLDataContext(options);
            Hop hop1 = new()
            {
                Code = "validCode123",
                HopType = "TestHopType",
                Description = "This is my hop for testing purposes.",
                LocationName = "Schwechat/Wien"
            };

           
            inMemoryDBContext.Hop.Add(hop1);
            hopRepo = new SqlHopRepository(inMemoryDBContext, testLogger);


            Configure In MemoryDatabase like:
            var options = new DbContextOptionsBuilder<SQLDataContext>()
                .UseInMemoryDatabase(databaseName: "TestingDatabase")
                .Options;
            var inMemoryDBContext = new SQLDataContext(options);

            hopRepo = new SqlHopRepository(inMemoryDBContext);


            Add Test Context like:
            inMemoryDBContext.Hop.Add(hop1);
            inMemoryDBContext.Hop.Add(hop2);
            parcelTestRepo = new DataAccess.Sql.SqlHopRepository(inMemoryDBContext);
            */
        }

        [Test]
        public void Alpha_GetAllHops_NoDataException_Test()
        {
            ILogger<SqlHopRepository> testLogger = new NullLogger<SqlHopRepository>();
            var options = new DbContextOptionsBuilder<SQLDataContext>()
                .UseInMemoryDatabase(databaseName: "TestingDatabase")
                .Options;
            var inMemoryDBContext = new SQLDataContext(options);

            SqlHopRepository repo = new(inMemoryDBContext, testLogger);

            Assert.Throws<DataException>(() => repo.GetAllHops());
        }



        [Test]
        public void Create_Test()
        {
            ILogger<SqlHopRepository> testLogger = new NullLogger<SqlHopRepository>();
            var options = new DbContextOptionsBuilder<SQLDataContext>()
                .UseInMemoryDatabase(databaseName: "TestingDatabase")
                .Options;
            var inMemoryDBContext = new SQLDataContext(options);
            SqlHopRepository repo = new(inMemoryDBContext, testLogger);

            Hop hop1 = new()
            {
                Code = "validCode124",
                HopType = "TestHopType",
                Description = "This is my hop for testing purposes.",
                LocationName = "Schwechat/Wien"
            };

            repo.Create(hop1);

            //Quering for data
            var resultHop = inMemoryDBContext.Hop
                .Single(b => b.Code == "validCode124");

            Assert.IsNotNull(resultHop);
        }



        [Test]
        public void Create_Throws_Exception_Test()
        {

            ILogger<SqlHopRepository> testLogger = new NullLogger<SqlHopRepository>();
            var options = new DbContextOptionsBuilder<SQLDataContext>()
                .UseInMemoryDatabase(databaseName: "TestingDatabase")
                .Options;
            var inMemoryDBContext = new SQLDataContext(options);
            SqlHopRepository repo = new(inMemoryDBContext, testLogger);

            Hop hop = new();

            Assert.Throws<DataException>(() => repo.Create(hop));
        }

        [Test]
        public void Delete_ParcelNULL_ThrowsDataException_Test()
        {
            ILogger<SqlHopRepository> testLogger = new NullLogger<SqlHopRepository>();
            var options = new DbContextOptionsBuilder<SQLDataContext>()
                .UseInMemoryDatabase(databaseName: "TestingDatabase")
                .Options;
            var inMemoryDBContext = new SQLDataContext(options);
            SqlHopRepository repo = new(inMemoryDBContext, testLogger);

            Assert.Throws<DataException>(() => repo.Delete("noID"));

        }


        [Test]
        public void Update_NoData_Exception_Test()
        {
            ILogger<SqlHopRepository> testLogger = new NullLogger<SqlHopRepository>();
            var options = new DbContextOptionsBuilder<SQLDataContext>()
                .UseInMemoryDatabase(databaseName: "TestingDatabase")
                .Options;
            var inMemoryDBContext = new SQLDataContext(options);
            SqlHopRepository repo = new(inMemoryDBContext, testLogger);

            Assert.Throws<DataException>(() => repo.Update(new Hop()));

        }

        [Test]
        public void GetHopbyCode_Test()
        {

            ILogger<SqlHopRepository> testLogger = new NullLogger<SqlHopRepository>();
            var options = new DbContextOptionsBuilder<SQLDataContext>()
                .UseInMemoryDatabase(databaseName: "TestingDatabase")
                .Options;
            var inMemoryDBContext = new SQLDataContext(options);

            SqlHopRepository repo = new(inMemoryDBContext, testLogger);

            Hop hop1 = new()
            {
                Code = "validCode123",
                HopType = "TestHopType",
                Description = "This is my hop for testing purposes.",
                LocationName = "Schwechat/Wien"
            };

            inMemoryDBContext.Hop.Add(hop1);

            Assert.IsNotNull(repo.GetHopByCode("validCode123"));
        }

        [Test]
        public void GetHopbyCode_NoDataException_Test()
        {

            ILogger<SqlHopRepository> testLogger = new NullLogger<SqlHopRepository>();
            var options = new DbContextOptionsBuilder<SQLDataContext>()
                .UseInMemoryDatabase(databaseName: "TestingDatabase")
                .Options;
            var inMemoryDBContext = new SQLDataContext(options);

            SqlHopRepository repo = new(inMemoryDBContext, testLogger);

            Assert.Throws<DataException>(() => repo.GetHopByCode("noHop"));
        }

        [Test]
        public void GetRootHop_Works()
        {
            Hop hop = new Warehouse()
            {
                HopType = "Warehouse",
                Code = "CODE",
                NextHops = new List<WarehouseNextHops>()
                {
                    new WarehouseNextHops()
                    {
                        Hop = new Warehouse()
                        {
                            Code = "CODI",
                            HopType = "Warehouse",
                            NextHops = new List<WarehouseNextHops>()
                            {
                                new WarehouseNextHops()
                                {
                                    Hop = new Hop()
                                    {
                                        Code = "CODO",
                                        HopType = "Truck"
                                    }
                                }
                            }
                        }
                    }
                }
            };
            ILogger<SqlHopRepository> testLogger = new NullLogger<SqlHopRepository>();
            var options = new DbContextOptionsBuilder<SQLDataContext>()
                .UseInMemoryDatabase(databaseName: "TestingDatabase")
                .Options;
            var inMemoryDBContext = new SQLDataContext(options);

            SqlHopRepository repo = new(inMemoryDBContext, testLogger);

            repo.Create(hop);


            Hop rootHop = repo.GetRootHop(hop);

            Console.WriteLine(rootHop.HopType);
            Assert.NotNull(rootHop);
        }

        [Test]
        public void GetHops_Works()
        {
            ILogger<SqlHopRepository> testLogger = new NullLogger<SqlHopRepository>();
            var options = new DbContextOptionsBuilder<SQLDataContext>()
                .UseInMemoryDatabase(databaseName: "TestingDatabase")
                .Options;
            var inMemoryDBContext = new SQLDataContext(options);

            SqlHopRepository repo = new(inMemoryDBContext, testLogger);

            Hop hop = new Warehouse()
            {
                HopType = "Warehouse",
                Level = 0,
                Code = "CODU",
                NextHops = new List<WarehouseNextHops>()
                {
                    new WarehouseNextHops()
                    {
                        Hop = new Warehouse()
                        {
                            Code = "CODA",
                            HopType = "Warehouse",
                            NextHops = new List<WarehouseNextHops>()
                            {
                                new WarehouseNextHops()
                                {
                                    Hop = new Hop()
                                    {
                                        Code = "CODY",
                                        HopType = "Truck"
                                    }
                                }
                            }
                        }
                    }
                }
            };

            repo.Create(hop);

            Hop roothop = repo.GetHops();
            Console.WriteLine(roothop.HopType);
            Assert.NotNull(roothop);
        }
    }
}
