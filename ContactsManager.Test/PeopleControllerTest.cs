using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using AutoFixture;
using FluentAssertions;
using ContactsManager.Models;
using ContactsManager.Controllers;
using ContactsManager.ServiceContracts;
using ContactsManager.ServiceContracts.DTOs;
using ContactsManager.ServiceContracts.Enums;

namespace ContactsManager.Test
{
    public class PeopleControllerTest
    {
        private readonly IPeopleService _peopleService;
        private readonly ICountriesService _countriesService;

        private readonly Mock<IPeopleService> _mockPeopleService;
        private readonly Mock<ICountriesService> _mockCountriesService;

        private readonly Mock<ILogger<PeopleController>> _mockLoggerPeopleController;
        private readonly ILogger<PeopleController> _loggerPeopleController;

        private readonly Fixture _fixture;

        public PeopleControllerTest()
        {
            _fixture = new Fixture();

            _mockPeopleService = new Mock<IPeopleService>();
            _mockCountriesService = new Mock<ICountriesService>();
            _mockLoggerPeopleController = new Mock<ILogger<PeopleController>>();

            _peopleService = _mockPeopleService.Object;
            _countriesService = _mockCountriesService.Object;
            _loggerPeopleController = _mockLoggerPeopleController.Object;
        }

        #region Index
        [Fact]
        public async Task Index_ReturnViewWithPeopleList()
        {
            // Arrange
            List<PersonResponse> people = _fixture.Create<List<PersonResponse>>();

            PeopleController peopleController = new PeopleController(_peopleService, _countriesService, _loggerPeopleController);

            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            peopleController.TempData = tempData;
            peopleController.TempData["NewUser"] = "";
            peopleController.TempData["ErrorDelete"] = "";

            _mockPeopleService.Setup(m => m
                .GetFilteredPeople(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(people);

            _mockPeopleService.Setup(m => m
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

            PeopleController peopleController = new PeopleController(_peopleService, _countriesService, _loggerPeopleController);

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

            PeopleController peopleController = new PeopleController(_peopleService, _countriesService, _loggerPeopleController);

            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            peopleController.TempData = tempData;

            _mockPeopleService.Setup(m => m
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

            _mockPeopleService.Setup(m => m
                .GetPersonByPersonId(It.IsAny<Guid>()))
                .ReturnsAsync(person.ToPersonResponse());

            PeopleController peopleController = new PeopleController(_peopleService, _countriesService, _loggerPeopleController);

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

            _mockPeopleService.Setup(m => m
                .GetPersonByPersonId(It.IsAny<Guid>()))
                .ReturnsAsync(null as PersonResponse);

            PeopleController peopleController = new PeopleController(_peopleService, _countriesService, _loggerPeopleController);

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

            _mockPeopleService.Setup(m => m
                .UpdatePerson(It.IsAny<PersonUpdateRequest>()))
                .ReturnsAsync(personUpdateRequest.ToPerson().ToPersonResponse());

            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            PeopleController peopleController = new PeopleController(_peopleService, _countriesService, _loggerPeopleController);
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

            _mockPeopleService.Setup(m => m
                .GetPersonByPersonId(It.IsAny<Guid>()))
                .ReturnsAsync(personResponse);

            PeopleController peopleController = new PeopleController(_peopleService, _countriesService, _loggerPeopleController);

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
            _mockPeopleService.Setup(m => m
                .GetPersonByPersonId(It.IsAny<Guid>()))
                .ReturnsAsync(null as PersonResponse);

            PeopleController peopleController = new PeopleController(_peopleService, _countriesService, _loggerPeopleController);

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

            _mockPeopleService.Setup(m => m
                .GetPersonByPersonId(It.IsAny<Guid>()))
                .ReturnsAsync(personResponse);

            _mockPeopleService.Setup(m => m
                .DeletePerson(It.IsAny<Guid>()))
                .ReturnsAsync(true);

            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            PeopleController peopleController = new PeopleController(_peopleService, _countriesService, _loggerPeopleController);
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

            _mockPeopleService.Setup(m => m
                .GetPersonByPersonId(It.IsAny<Guid>()))
                .ReturnsAsync(personResponse);

            _mockPeopleService.Setup(m => m
                .DeletePerson(It.IsAny<Guid>()))
                .ReturnsAsync(false);

            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            PeopleController peopleController = new PeopleController(_peopleService, _countriesService, _loggerPeopleController);
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
