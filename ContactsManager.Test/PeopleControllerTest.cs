using AutoFixture;
using ContactsManager.Controllers;
using ContactsManager.Models;
using ContactsManager.ServiceContracts;
using ContactsManager.ServiceContracts.DTOs;
using ContactsManager.ServiceContracts.Enums;
using ContactsManager.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;

namespace ContactsManager.Test
{
    public class PeopleControllerTest
    {
        private readonly IPeopleAdderService _peopleAdderService;
        private readonly IPeopleGetterService _peopleGetterService;
        private readonly IPeopleDeleterService _peopleDeleterService;
        private readonly IPeopleUpdaterService _peopleUpdaterService;
        private readonly IPeopleSorterService _peopleSorterService;
        private readonly ICountriesService _countriesService;

        private readonly Mock<IPeopleAdderService> _mockPeopleAdderService;
        private readonly Mock<IPeopleGetterService> _mockPeopleGetterService;
        private readonly Mock<IPeopleDeleterService> _mockPeopleDeleterService;
        private readonly Mock<IPeopleUpdaterService> _mockPeopleUpdaterService;
        private readonly Mock<IPeopleSorterService> _mockPeopleSorterService;
        private readonly Mock<ICountriesService> _mockCountriesService;

        private readonly PeopleController peopleController;

        private readonly Mock<ILogger<PeopleController>> _mockLoggerPeopleController;
        private readonly ILogger<PeopleController> _loggerPeopleController;

        private readonly Fixture _fixture;

        public PeopleControllerTest()
        {
            _fixture = new Fixture();

            _mockPeopleAdderService = new Mock<IPeopleAdderService>();
            _mockPeopleGetterService = new Mock<IPeopleGetterService>();
            _mockPeopleDeleterService = new Mock<IPeopleDeleterService>();
            _mockPeopleUpdaterService = new Mock<IPeopleUpdaterService>();
            _mockPeopleSorterService = new Mock<IPeopleSorterService>();

            _mockCountriesService = new Mock<ICountriesService>();
            _mockLoggerPeopleController = new Mock<ILogger<PeopleController>>();

            _peopleAdderService = _mockPeopleAdderService.Object;
            _peopleGetterService = _mockPeopleGetterService.Object;
            _peopleDeleterService = _mockPeopleDeleterService.Object;
            _peopleUpdaterService = _mockPeopleUpdaterService.Object;
            _peopleSorterService = _mockPeopleSorterService.Object;

            _countriesService = _mockCountriesService.Object;
            _loggerPeopleController = _mockLoggerPeopleController.Object;

            peopleController = new PeopleController(_peopleAdderService, _peopleGetterService, _peopleDeleterService, _peopleUpdaterService, _peopleSorterService, _countriesService, _loggerPeopleController);
        }

        #region Index
        [Fact]
        public async Task Index_ReturnViewWithPeopleList()
        {
            // Arrange
            List<PersonResponse> people = _fixture.Create<List<PersonResponse>>();

            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());peopleController.TempData = tempData;
            peopleController.TempData["NewUser"] = "";
            peopleController.TempData["ErrorDelete"] = "";

            _mockPeopleGetterService.Setup(m => m
                .GetFilteredPeople(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(people);

            _mockPeopleSorterService.Setup(m => m
                .GetSortedPeople(It.IsAny<List<PersonResponse>>(), It.IsAny<string>(), It.IsAny<SortOrderOptions>()))
                .Returns(people);

            // Act
            var result = await peopleController.Index(_fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<SortOrderOptions>());

            // Assert
            ViewResult? viewResult = result.Should().BeOfType<ViewResult>().Which;
            //ViewResult? viewResult = Assert.IsType<ViewResult>(result);

            viewResult.ViewData.Model.Should().BeAssignableTo<IEnumerable<PersonResponse>>();
            viewResult.ViewData.Model.Should().Be(people);
        }
        #endregion

        #region Create
        [Fact]
        public async Task Create_ReturnCreateView()
        {
            // Arrange
            List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();

            _mockCountriesService.Setup(m => m
                .GetCountries())
                .ReturnsAsync(countries);

            // Act
            var result = await peopleController.Create();

            // Assert
            ViewResult viewResult = result.Should().BeOfType<ViewResult>().Which;
            
            viewResult.ViewData.TryGetValue("Countries", out var listOfCountries);
            listOfCountries.Should().BeAssignableTo<IEnumerable<SelectListItem>?>();
        }

        [Fact]
        public async Task Create_IfNoModelErrors_ToRedirectToIndexView()
        {
            // Arrange
            PersonAddRequest personAddRequest = _fixture.Create<PersonAddRequest>();
            PersonResponse personResponse = personAddRequest.ToPerson().ToPersonResponse();

            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            peopleController.TempData = tempData;

            _mockPeopleAdderService.Setup(m => m
                .AddPerson(It.IsAny<PersonAddRequest>()))
                .ReturnsAsync(personResponse);

            // Act
            var result = await peopleController.Create(personAddRequest);

            // Assert
            RedirectToActionResult? redirectResult = result.Should().BeOfType<RedirectToActionResult>().Which;

            redirectResult.ActionName.Should().Be(nameof(peopleController.Index));
            redirectResult.ControllerName.Should().Be("People");
        }
        #endregion

        #region Edit
        [Fact]
        public async Task Edit_ReturnEditView()
        {
            // Arrange
            List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();
            Person person = _fixture.Build<Person>()
                .With(p => p.Gender, GenderOptions.Other.ToString())
                .Create();

            _mockCountriesService.Setup(m => m
                .GetCountries())
                .ReturnsAsync(countries);

            _mockPeopleGetterService.Setup(m => m
                .GetPersonByPersonId(It.IsAny<Guid>()))
                .ReturnsAsync(person.ToPersonResponse());

            // Act
            var result = await peopleController.Edit(person.PersonId);

            // Assert
            ViewResult viewResult = result.Should().BeOfType<ViewResult>().Which;

            viewResult.ViewData.Model.Should().BeAssignableTo<PersonUpdateRequest?>();
        }

        [Fact]
        public async Task Edit_RedirectIndexView()
        {
            // Arrange
            List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();

            _mockCountriesService.Setup(m => m
                .GetCountries())
                .ReturnsAsync(countries);

            _mockPeopleGetterService.Setup(m => m
                .GetPersonByPersonId(It.IsAny<Guid>()))
                .ReturnsAsync(null as PersonResponse);

            // Act
            var result = await peopleController.Edit(Guid.NewGuid());

            // Assert
            RedirectToActionResult redirectResult = result.Should().BeOfType<RedirectToActionResult>().Which;

            redirectResult.ActionName.Should().Be(nameof(peopleController.Index));
            redirectResult.ControllerName.Should().Be("People");
        }

        [Fact]
        public async Task Edit_IfNoModelErrors_ToRedirectIndexView()
        {
            // Arrange
            PersonUpdateRequest personUpdateRequest = _fixture.Create<PersonUpdateRequest>();
            List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();

            _mockPeopleUpdaterService.Setup(m => m
                .UpdatePerson(It.IsAny<PersonUpdateRequest>()))
                .ReturnsAsync(personUpdateRequest.ToPerson().ToPersonResponse());

            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            peopleController.TempData = tempData;

            // Act
            IActionResult? result = await peopleController.Edit(personUpdateRequest);

            // Assert
            RedirectToActionResult redirectResult = result.Should().BeOfType<RedirectToActionResult>().Which;

            redirectResult.ActionName.Should().Be(nameof(peopleController.Index));
            redirectResult.ControllerName.Should().Be("People");
        }
        #endregion

        #region Delete
        [Fact]
        public async Task Delete_ReturnDeleteView()
        {
            // Arrange
            PersonResponse personResponse = _fixture.Create<PersonResponse>();

            _mockPeopleGetterService.Setup(m => m
                .GetPersonByPersonId(It.IsAny<Guid>()))
                .ReturnsAsync(personResponse);

            // Act
            var result = await peopleController.DeleteView(personResponse.PersonId);

            // Assert
            PartialViewResult partialViewResult = result.Should().BeOfType<PartialViewResult>().Which;

            partialViewResult.Model.Should().NotBeNull();
            partialViewResult.Model.Should().BeAssignableTo<PersonResponse>();
            partialViewResult.Model.Should().Be(personResponse);
            partialViewResult.ViewName.Should().Be("_Delete");
        }

        [Fact]
        public async Task Delete_RedirectIndexView()
        {
            // Arrange
            _mockPeopleGetterService.Setup(m => m
                .GetPersonByPersonId(It.IsAny<Guid>()))
                .ReturnsAsync(null as PersonResponse);

            // Act
            var result = await peopleController.DeleteView(Guid.NewGuid());

            // Assert
            RedirectToActionResult redirectResult = result.Should().BeOfType<RedirectToActionResult>().Which;

            redirectResult.ActionName.Should().Be(nameof(peopleController.Index));
            redirectResult.ControllerName.Should().Be("People");
        }

        [Fact]
        public async Task Delete_ProperPersonId_RedirectIndexView()
        {
            // Arrange
            PersonResponse personResponse = _fixture.Create<PersonResponse>();

            _mockPeopleGetterService.Setup(m => m
                .GetPersonByPersonId(It.IsAny<Guid>()))
                .ReturnsAsync(personResponse);

            _mockPeopleDeleterService.Setup(m => m
                .DeletePerson(It.IsAny<Guid>()))
                .ReturnsAsync(true);

            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            peopleController.TempData = tempData;

            // Act
            var result = await peopleController.DeleteUser(personResponse.PersonId);

            // Assert
            RedirectToActionResult redirectResult = result.Should().BeOfType<RedirectToActionResult>().Which;

            peopleController.TempData["NewUser"].Should().Be($"{personResponse?.PersonName} was successfully deleted.");
            peopleController.TempData["ErrorDelete"].Should().BeNull();
            redirectResult.ActionName.Should().Be(nameof(peopleController.Index));
            redirectResult.ControllerName.Should().Be("People");
        }

        [Fact]
        public async Task Delete_WrongPersonId_RedirectIndexView()
        {
            // Arrange
            PersonResponse personResponse = _fixture.Create<PersonResponse>();

            _mockPeopleGetterService.Setup(m => m
                .GetPersonByPersonId(It.IsAny<Guid>()))
                .ReturnsAsync(personResponse);

            _mockPeopleDeleterService.Setup(m => m
                .DeletePerson(It.IsAny<Guid>()))
                .ReturnsAsync(false);

            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            peopleController.TempData = tempData;

            // Act
            var result = await peopleController.DeleteUser(personResponse.PersonId);

            // Assert
            RedirectToActionResult redirectResult = result.Should().BeOfType<RedirectToActionResult>().Which;

            peopleController.TempData["ErrorDelete"].Should().Be("Ocurrs an error while deleting.");
            peopleController.TempData["NewUser"].Should().BeNull();
            redirectResult.ActionName.Should().Be(nameof(peopleController.Index));
            redirectResult.ControllerName.Should().Be("People");
        }
        #endregion
    }
}
