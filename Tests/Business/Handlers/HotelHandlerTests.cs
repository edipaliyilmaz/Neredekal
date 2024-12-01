
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
using System.Linq;
using FluentAssertions;


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
            //Arrange
            var query = new GetHotelQuery();

            _hotelRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Hotel, bool>>>())).ReturnsAsync(new Hotel()
//propertyler buraya yazılacak
//{																		
//HotelId = 1,
//HotelName = "Test"
//}
);

            var handler = new GetHotelQueryHandler(_hotelRepository.Object, _mediator.Object);

            //Act
            var x = await handler.Handle(query, new System.Threading.CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            //x.Data.HotelId.Should().Be(1);

        }

        [Test]
        public async Task Hotel_GetQueries_Success()
        {
            //Arrange
            var query = new GetHotelsQuery();

            _hotelRepository.Setup(x => x.GetListAsync(It.IsAny<Expression<Func<Hotel, bool>>>()))
                        .ReturnsAsync(new List<Hotel> { new Hotel() { /*TODO:propertyler buraya yazılacak HotelId = 1, HotelName = "test"*/ } });

            var handler = new GetHotelsQueryHandler(_hotelRepository.Object, _mediator.Object);

            //Act
            var x = await handler.Handle(query, new System.Threading.CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            ((List<Hotel>)x.Data).Count.Should().BeGreaterThan(1);

        }

        [Test]
        public async Task Hotel_CreateCommand_Success()
        {
            Hotel rt = null;
            //Arrange
            var command = new CreateHotelCommand();
            //propertyler buraya yazılacak
            //command.HotelName = "deneme";

            _hotelRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Hotel, bool>>>()))
                        .ReturnsAsync(rt);

            _hotelRepository.Setup(x => x.Add(It.IsAny<Hotel>())).Returns(new Hotel());

            var handler = new CreateHotelCommandHandler(_hotelRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            _hotelRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Added);
        }

        [Test]
        public async Task Hotel_CreateCommand_NameAlreadyExist()
        {
            //Arrange
            var command = new CreateHotelCommand();
            //propertyler buraya yazılacak 
            //command.HotelName = "test";

            _hotelRepository.Setup(x => x.Query())
                                           .Returns(new List<Hotel> { new Hotel() { /*TODO:propertyler buraya yazılacak HotelId = 1, HotelName = "test"*/ } }.AsQueryable());

            _hotelRepository.Setup(x => x.Add(It.IsAny<Hotel>())).Returns(new Hotel());

            var handler = new CreateHotelCommandHandler(_hotelRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.NameAlreadyExist);
        }

        [Test]
        public async Task Hotel_UpdateCommand_Success()
        {
            //Arrange
            var command = new UpdateHotelCommand();
            //command.HotelName = "test";

            _hotelRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Hotel, bool>>>()))
                        .ReturnsAsync(new Hotel() { /*TODO:propertyler buraya yazılacak HotelId = 1, HotelName = "deneme"*/ });

            _hotelRepository.Setup(x => x.Update(It.IsAny<Hotel>())).Returns(new Hotel());

            var handler = new UpdateHotelCommandHandler(_hotelRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            _hotelRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Updated);
        }

        [Test]
        public async Task Hotel_DeleteCommand_Success()
        {
            //Arrange
            var command = new DeleteHotelCommand();

            _hotelRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Hotel, bool>>>()))
                        .ReturnsAsync(new Hotel() { /*TODO:propertyler buraya yazılacak HotelId = 1, HotelName = "deneme"*/});

            _hotelRepository.Setup(x => x.Delete(It.IsAny<Hotel>()));

            var handler = new DeleteHotelCommandHandler(_hotelRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            _hotelRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Deleted);
        }
    }
}

