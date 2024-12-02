using Business.Handlers.Hotels.Queries;
using DataAccess.Abstract;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static Business.Handlers.Hotels.Queries.GetHotelQuery;
using Entities.Concrete;
using static Business.Handlers.Hotels.Queries.GetHotelsQuery;
using static Business.Handlers.Hotels.Commands.CreateHotelCommand;
using Business.Handlers.Hotels.Commands;
using Business.Constants;
using static Business.Handlers.Hotels.Commands.UpdateHotelCommand;
using static Business.Handlers.Hotels.Commands.DeleteHotelCommand;
using MediatR;
using FluentAssertions;
using System.Linq;
using Entities.Dtos;

namespace Tests.Business.HandlersTest
{
    [TestFixture]
    public class HotelHandlerTests
    {
        Mock<IHotelRepository> _hotelRepository;
        Mock<IMediator> _mediator;

        [SetUp]
        public void Setup()
        {
            _hotelRepository = new Mock<IHotelRepository>();
            _mediator = new Mock<IMediator>();
        }

        [Test]
        public async Task Hotel_GetQuery_Success()
        {
            // Arrange
            var query = new GetHotelQuery();

            _hotelRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Hotel, bool>>>())).ReturnsAsync(new Hotel
            {
                Id = Guid.NewGuid(),
                ManagerFirstName = "John",
                ManagerLastName = "Doe",
                CompanyName = "Test Hotel Inc.",
                Contacts = new List<Contact>(),
                CreateDate = DateTime.Now
            });

            var handler = new GetHotelQueryHandler(_hotelRepository.Object, _mediator.Object);

            // Act
            var result = await handler.Handle(query, new System.Threading.CancellationToken());

            // Assert
            result.Success.Should().BeTrue();
            result.Data.ManagerFirstName.Should().Be("John");
            result.Data.CompanyName.Should().Be("Test Hotel Inc.");
        }

        [Test]
        public async Task Hotel_GetQueries_Success()
        {
            // Arrange
            var query = new GetHotelsQuery();

            _hotelRepository.Setup(x => x.GetListAsync(It.IsAny<Expression<Func<Hotel, bool>>>()))
                        .ReturnsAsync(new List<Hotel>
                        {
                            new Hotel { Id = Guid.NewGuid(), ManagerFirstName = "John", ManagerLastName = "Doe", CompanyName = "Hotel 1", Contacts = new List<Contact>(), CreateDate = DateTime.Now },
                            new Hotel { Id = Guid.NewGuid(), ManagerFirstName = "Jane", ManagerLastName = "Smith", CompanyName = "Hotel 2", Contacts = new List<Contact>(), CreateDate = DateTime.Now }
                        });

            var handler = new GetHotelsQueryHandler(_hotelRepository.Object, _mediator.Object);

            // Act
            var result = await handler.Handle(query, new System.Threading.CancellationToken());

            // Assert
            result.Success.Should().BeTrue();
            ((List<Hotel>)result.Data).Count.Should().BeGreaterThan(1);
        }

        [Test]
        public async Task Hotel_CreateCommand_Success()
        {
            // Arrange
            var command = new CreateHotelCommand
            {
                ManagerFirstName = "John",
                ManagerLastName = "Doe",
                CompanyName = "New Hotel",
                Contacts = new List<ContactDto>()
            };

            _hotelRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Hotel, bool>>>()))
                        .ReturnsAsync((Hotel)null);

            _hotelRepository.Setup(x => x.Add(It.IsAny<Hotel>())).Returns(new Hotel());

            var handler = new CreateHotelCommandHandler(_hotelRepository.Object, _mediator.Object);
            var result = await handler.Handle(command, new System.Threading.CancellationToken());

            _hotelRepository.Verify(x => x.SaveChangesAsync());
            result.Success.Should().BeTrue();
            result.Message.Should().Be(Messages.Added);
        }

        [Test]
        public async Task Hotel_CreateCommand_NameAlreadyExist()
        {
            // Arrange
            var command = new CreateHotelCommand
            {
                CompanyName = "Existing Hotel"
            };

            _hotelRepository.Setup(x => x.Query())
                .Returns(new List<Hotel> { new Hotel { Id = Guid.NewGuid(), CompanyName = "Existing Hotel", ManagerFirstName = "John", ManagerLastName = "Doe", Contacts = new List<Contact>(), CreateDate = DateTime.Now } }.AsQueryable());

            var handler = new CreateHotelCommandHandler(_hotelRepository.Object, _mediator.Object);
            var result = await handler.Handle(command, new System.Threading.CancellationToken());

            result.Success.Should().BeFalse();
            result.Message.Should().Be(Messages.NameAlreadyExist);
        }

        [Test]
        public async Task Hotel_UpdateCommand_Success()
        {
            // Arrange
            var command = new UpdateHotelCommand
            {
                Id = Guid.NewGuid(),
                ManagerFirstName = "Updated",
                ManagerLastName = "Manager",
                CompanyName = "Updated Hotel"
            };

            _hotelRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Hotel, bool>>>()))
                        .ReturnsAsync(new Hotel { Id = command.Id, ManagerFirstName = "Old", ManagerLastName = "Manager", CompanyName = "Old Hotel", Contacts = new List<Contact>(), CreateDate = DateTime.Now });

            _hotelRepository.Setup(x => x.Update(It.IsAny<Hotel>())).Returns(new Hotel());

            var handler = new UpdateHotelCommandHandler(_hotelRepository.Object, _mediator.Object);
            var result = await handler.Handle(command, new System.Threading.CancellationToken());

            _hotelRepository.Verify(x => x.SaveChangesAsync());
            result.Success.Should().BeTrue();
            result.Message.Should().Be(Messages.Updated);
        }

        [Test]
        public async Task Hotel_DeleteCommand_Success()
        {
            // Arrange
            var command = new DeleteHotelCommand
            {
                Id = Guid.NewGuid()
            };

            _hotelRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Hotel, bool>>>()))
                        .ReturnsAsync(new Hotel { Id = command.Id, ManagerFirstName = "Manager", ManagerLastName = "One", CompanyName = "Test Hotel", Contacts = new List<Contact>(), CreateDate = DateTime.Now });

            _hotelRepository.Setup(x => x.Delete(It.IsAny<Hotel>()));

            var handler = new DeleteHotelCommandHandler(_hotelRepository.Object, _mediator.Object);
            var result = await handler.Handle(command, new System.Threading.CancellationToken());

            _hotelRepository.Verify(x => x.SaveChangesAsync());
            result.Success.Should().BeTrue();
            result.Message.Should().Be(Messages.Deleted);
        }
    }
}
