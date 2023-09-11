# LearningUnitTesting
- C#
- .NET 7, .NET Framework 4.7.2
- xUnit, MSTest
- FakeItEasy, Moq

> Learning to do unit tests, using old project copies for testing:
>   - [.NET 7 WebAPI and MVC projects](https://github.com/kovac031/AutoMapper-Core)
>   - [.NET Framework WebAPI and MVC projects](https://github.com/kovac031/AutoMapper-EF)

## Challenges and learning points
- using tutorials and finding examples to learn from
- xUnit and MSTest differences
- FakeItEasy and Moq differences
- resolving Entity Framework issues
  - occassional conflicts with AutoMapper
  - occassional conflicts with async methods
- test naming convention
- optimizing test files structure
- learning what makes sense to test and where
- utilizing helper classes
- DRY practices
  - calling GetFakeStudents() from outside

## Comparing
> xUnit and MSTest both require DI-like setup for tests to work

> FakeItEasy and Moq syntax slightly different

.NET 7 MVC controller test initialization, using xUnit and FakeItEasy:

```C#
public class ListWithParamsTests
    {
        private readonly StudentController _controller;
        private readonly IService _service;
        private readonly IMapper _mapper;

        private readonly ITestOutputHelper _output;

        public ListWithParamsTests(ITestOutputHelper output)
        {
            _service = A.Fake<IService>(); //FakeItEasy
            _mapper = A.Fake<IMapper>();
            _controller = new StudentController(_service, _mapper); // SUT, system under test
            _output = output;
        }
...
```

.NET Framework MVC controller test initialization, using MSTest and Moq:

```C#
[TestClass]
public class ControllerTests
    {
        private DefaultController _controller;
        private Mock<IService> _service;
        private IMapper _mapper;

        [TestInitialize] 
        public void Setup()
        {
            _service = new Mock<IService>();

            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            _mapper = config.CreateMapper();

            _controller = new DefaultController(_service.Object, _mapper); 
        }
...
```

.NET 7 WebApi controller test initialization, using xUnit and FakeItEasy:

```C#
public class StudentControllerTests
    {
        private readonly StudentController _controller;
        private readonly IService _service;
        public StudentControllerTests()
        {
            _service = A.Fake<IService>(); //FakeItEasy
            _controller = new StudentController(_service); // SUT, system under test
        }
...
```

.NET Framework WebApi controller test initialization, using MSTest and Moq:

```C#
[TestClass]
public class GetAllAsyncTests
    {
        private DefaultController _controller;
        private Mock<IService> _service;

        [TestInitialize]
        public void Setup()
        {
            _service = new Mock<IService>();
            _controller = new DefaultController(_service.Object);

            // http fore za webapi controller, toga nema u .net 7 verziji
            _controller.Request = new HttpRequestMessage();
            _controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
        }
...
```

