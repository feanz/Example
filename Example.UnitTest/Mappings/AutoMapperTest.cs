using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Web.Mappings;
using AutoMapper;


namespace Example.UnitTest.Mappings
{
    [TestClass]
    public class AutoMapperTest
    {
        [TestInitialize]
        public void AutoMapperSetup()
        {
            AutoMapperConfiguration.ConfigureMappings();
        }

        [TestMethod]
        public void Mapping_are_Valid()
        {
            Mapper.AssertConfigurationIsValid();
        }
    }
}
