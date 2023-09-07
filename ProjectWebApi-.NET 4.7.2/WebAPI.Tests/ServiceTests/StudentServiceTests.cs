using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Project.WebAPI.Controllers;
using Service.Common;
using System;
using System.Net.Http;
using System.Web.Http.Hosting;
using System.Web.Http;
using Repository.Common;
using Service;

namespace WebAPI.Tests.ServiceTests
{
    [TestClass]
    public class StudentServiceTests
    {
        private StudentService _service;
        private Mock<IRepository> _repository;

        [TestInitialize]
        public void Setup()
        {
            _repository = new Mock<IRepository>();
            _service = new StudentService(_repository.Object);
        }
        //-------------------------------------------------------------------


    }
}
