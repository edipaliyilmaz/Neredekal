
using Business.Handlers.ContactInfoes.Queries;
using DataAccess.Abstract;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static Business.Handlers.ContactInfoes.Queries.GetContactInfoQuery;
using Entities.Concrete;
using static Business.Handlers.ContactInfoes.Queries.GetContactInfoesQuery;
using static Business.Handlers.ContactInfoes.Commands.CreateContactInfoCommand;
using Business.Handlers.ContactInfoes.Commands;
using Business.Constants;
using static Business.Handlers.ContactInfoes.Commands.UpdateContactInfoCommand;
using static Business.Handlers.ContactInfoes.Commands.DeleteContactInfoCommand;
using MediatR;
using System.Linq;
using FluentAssertions;


namespace Tests.Business.HandlersTest
{
    [TestFixture]
    public class ContactInfoHandlerTests
    {
        Mock<IContactInfoRepository> _contactInfoRepository;
        Mock<IMediator> _mediator;
        [SetUp]
        public void Setup()
        {
            _contactInfoRepository = new Mock<IContactInfoRepository>();
            _mediator = new Mock<IMediator>();
        }

        [Test]
        public async Task ContactInfo_GetQuery_Success()
        {
            //Arrange
            var query = new GetContactInfoQuery();

            _contactInfoRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Contact, bool>>>())).ReturnsAsync(new Contact()
//propertyler buraya yazılacak
//{																		
//ContactInfoId = 1,
//ContactInfoName = "Test"
//}
);

            var handler = new GetContactInfoQueryHandler(_contactInfoRepository.Object, _mediator.Object);

            //Act
            var x = await handler.Handle(query, new System.Threading.CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            //x.Data.ContactInfoId.Should().Be(1);

        }

        [Test]
        public async Task ContactInfo_GetQueries_Success()
        {
            //Arrange
            var query = new GetContactInfoesQuery();

            _contactInfoRepository.Setup(x => x.GetListAsync(It.IsAny<Expression<Func<Contact, bool>>>()))
                        .ReturnsAsync(new List<Contact> { new Contact() { /*TODO:propertyler buraya yazılacak ContactInfoId = 1, ContactInfoName = "test"*/ } });

            var handler = new GetContactInfoesQueryHandler(_contactInfoRepository.Object, _mediator.Object);

            //Act
            var x = await handler.Handle(query, new System.Threading.CancellationToken());

            //Asset
            x.Success.Should().BeTrue();
            ((List<Contact>)x.Data).Count.Should().BeGreaterThan(1);

        }

        [Test]
        public async Task ContactInfo_CreateCommand_Success()
        {
            Contact rt = null;
            //Arrange
            var command = new CreateContactInfoCommand();
            //propertyler buraya yazılacak
            //command.ContactInfoName = "deneme";

            _contactInfoRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Contact, bool>>>()))
                        .ReturnsAsync(rt);

            _contactInfoRepository.Setup(x => x.Add(It.IsAny<Contact>())).Returns(new Contact());

            var handler = new CreateContactInfoCommandHandler(_contactInfoRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            _contactInfoRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Added);
        }

        [Test]
        public async Task ContactInfo_CreateCommand_NameAlreadyExist()
        {
            //Arrange
            var command = new CreateContactInfoCommand();
            //propertyler buraya yazılacak 
            //command.ContactInfoName = "test";

            _contactInfoRepository.Setup(x => x.Query())
                                           .Returns(new List<Contact> { new Contact() { /*TODO:propertyler buraya yazılacak ContactInfoId = 1, ContactInfoName = "test"*/ } }.AsQueryable());

            _contactInfoRepository.Setup(x => x.Add(It.IsAny<Contact>())).Returns(new Contact());

            var handler = new CreateContactInfoCommandHandler(_contactInfoRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.NameAlreadyExist);
        }

        [Test]
        public async Task ContactInfo_UpdateCommand_Success()
        {
            //Arrange
            var command = new UpdateContactInfoCommand();
            //command.ContactInfoName = "test";

            _contactInfoRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Contact, bool>>>()))
                        .ReturnsAsync(new Contact() { /*TODO:propertyler buraya yazılacak ContactInfoId = 1, ContactInfoName = "deneme"*/ });

            _contactInfoRepository.Setup(x => x.Update(It.IsAny<Contact>())).Returns(new Contact());

            var handler = new UpdateContactInfoCommandHandler(_contactInfoRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            _contactInfoRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Updated);
        }

        [Test]
        public async Task ContactInfo_DeleteCommand_Success()
        {
            //Arrange
            var command = new DeleteContactInfoCommand();

            _contactInfoRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Contact, bool>>>()))
                        .ReturnsAsync(new Contact() { /*TODO:propertyler buraya yazılacak ContactInfoId = 1, ContactInfoName = "deneme"*/});

            _contactInfoRepository.Setup(x => x.Delete(It.IsAny<Contact>()));

            var handler = new DeleteContactInfoCommandHandler(_contactInfoRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            _contactInfoRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Deleted);
        }
    }
}

