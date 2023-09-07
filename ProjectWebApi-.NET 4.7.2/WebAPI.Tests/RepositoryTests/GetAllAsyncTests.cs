using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Repository.Common;
using Service;
using System;

namespace WebAPI.Tests.RepositoryTests
{
    [TestClass]
    public class GetAllAsyncTests
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
