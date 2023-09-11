# LearningUnitTesting
- C#
- .NET 7, .NET Framework 4.7.2
- xUnit, MSTest
- FakeItEasy, Moq

> Learning to do unit tests, using old project copies for testing:
>   - [.NET 7 WebAPI and MVC projects](https://github.com/kovac031/AutoMapper-Core)
>   - [.NET Framework WebAPI and MVC projects](https://github.com/kovac031/AutoMapper-EF)

## Challenges and learning points
- learning how to write tests for CRUD operations
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
- wrote 124 tests overall, did not test all parameters

## Comparing initial setup
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

## Comparing syntax
> syntax for calling the system under test different

xUnit has:

```C#
List<StudentDTO> fakeStudents = GetFakeStudents();
A.CallTo(() => _service.GetAllAsync()).Returns(fakeStudents);
```

MSTest has:

```C#
List<StudentDTO> fakeStudents = GetFakeStudents();
_service.Setup(x => x.GetAllAsync(null)).ReturnsAsync(fakeStudents);
```

## Including custom information in test result
> adding custom information to test result or debugging different syntax

xUnit uses ITestOutputHelper which requires injecting first:

```C#
_output.WriteLine($"Passed from repository - FirstName: {student.FirstName}");
```

MSTest is simpler, just uses Console.WriteLine, no DI:

```C#
Console.WriteLine($"Passed from repository - FirstName: {student.FirstName}");
```

## DbSet in repository
> Unit tests in the repository require mocking a DbSet, to be used when calling the mock DBcontext

xUnit, FakeItEasy:

```C#
private DbSet<T> CreateFakeDbSet<T>(IQueryable<T> data) where T : class
        {
            IQueryable<T> queryable = data;

            DbSet<T> fakeDbSet = A.Fake<DbSet<T>>(d => d.Implements(typeof(IQueryable<T>))
                                                      .Implements(typeof(IAsyncEnumerable<T>)));
            A.CallTo(() => ((IQueryable<T>)fakeDbSet).Provider).Returns(new TestAsyncQueryProvider<T>(queryable.Provider));
            A.CallTo(() => ((IQueryable<T>)fakeDbSet).Expression).Returns(queryable.Expression);
            A.CallTo(() => ((IQueryable<T>)fakeDbSet).ElementType).Returns(queryable.ElementType);
            A.CallTo(() => ((IQueryable<T>)fakeDbSet).GetEnumerator()).Returns(data.GetEnumerator());
            //
            A.CallTo(() => ((IAsyncEnumerable<T>)fakeDbSet).GetAsyncEnumerator(A<CancellationToken>._)).Returns(new TestAsyncEnumerator<T>(data.GetEnumerator()));

            return fakeDbSet;
        }
```

MSTest, Moq:

```C#
public static Mock<DbSet<T>> CreateMockDbSet<T>(List<T> sourceList) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(sourceList.AsQueryable().Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(sourceList.AsQueryable().Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(sourceList.AsQueryable().ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(sourceList.AsQueryable().GetEnumerator());
            return mockSet;
        }
```

## How to organize tests
> the goal is to keep things clean and easily readable

What I did in WebAPI projects:
![scr1](https://github.com/kovac031/LearningUnitTesting/blob/main/WebAPIs-tests-structure.jpg)

> on the left, each method gets its own tests class
>   - more file clutter, but more neatly organized

> on the right, all tests are within their layer tests class
>   - less file clutter, but one class can end up with hundreds of lines of code, which is not easy to navigate

What I did in MVC projects:
![scr2](https://github.com/kovac031/LearningUnitTesting/blob/main/MVCs-controller-test-structure.jpg)
> I focused on writing controller tests, as I did service and repository layer tests in the WebAPI projects

> on the left, each method gets its own tests class
>   - easier to read and navigate

> on the right, all tests are within the ControllerTests class
>   - less file clutter, but all 15 tests - all testing different methods - are in the same file, which is not ideal
