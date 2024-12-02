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
using Core.Enums;

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
            // Arrange
            var query = new GetContactInfoQuery();
            _contactInfoRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Contact, bool>>>())).ReturnsAsync(new Contact
            {
                Id = Guid.NewGuid(),
                HotelId = Guid.NewGuid(),
                Type = ContactType.Email,
                Value = "test@example.com",
                CreateDate = DateTime.Now
            });

            var handler = new GetContactInfoQueryHandler(_contactInfoRepository.Object, _mediator.Object);

            // Act
            var result = await handler.Handle(query, new System.Threading.CancellationToken());

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            var contact = (Contact)result.Data;
            contact.Id.Should().NotBeEmpty();
            contact.HotelId.Should().NotBeEmpty();
            contact.Type.Should().Be(ContactType.Email);
            contact.Value.Should().Be("test@example.com");
        }

        [Test]
        public async Task ContactInfo_GetQueries_Success()
        {
            // Arrange
            var query = new GetContactInfoesQuery();
            _contactInfoRepository.Setup(x => x.GetListAsync(It.IsAny<Expression<Func<Contact, bool>>>()))
                        .ReturnsAsync(new List<Contact>
                        {
                            new Contact
                            {
                                Id = Guid.NewGuid(),
                                HotelId = Guid.NewGuid(),
                                Type = ContactType.PhoneNumber,
                                Value = "123-456-7890",
                                CreateDate = DateTime.Now
                            }
                        });

            var handler = new GetContactInfoesQueryHandler(_contactInfoRepository.Object, _mediator.Object);

            // Act
            var result = await handler.Handle(query, new System.Threading.CancellationToken());

            // Assert
            result.Success.Should().BeTrue();
            ((List<Contact>)result.Data).Count.Should().BeGreaterThan(0);
        }

        [Test]
        public async Task ContactInfo_CreateCommand_Success()
        {
            Contact existingContact = null;
            // Arrange
            var command = new CreateContactInfoCommand
            {
                HotelId = Guid.NewGuid(),
                Type = ContactType.PhoneNumber,
                Value = "123-456-7890"
            };

            _contactInfoRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Contact, bool>>>()))
                        .ReturnsAsync(existingContact);

            _contactInfoRepository.Setup(x => x.Add(It.IsAny<Contact>())).Returns(new Contact
            {
                Id = Guid.NewGuid(),
                HotelId = command.HotelId,
                Type = command.Type,
                Value = command.Value,
                CreateDate = DateTime.Now
            });

            var handler = new CreateContactInfoCommandHandler(_contactInfoRepository.Object, _mediator.Object);
            var result = await handler.Handle(command, new System.Threading.CancellationToken());

            _contactInfoRepository.Verify(x => x.SaveChangesAsync());
            result.Success.Should().BeTrue();
            result.Message.Should().Be(Messages.Added);
        }

        [Test]
        public async Task ContactInfo_CreateCommand_NameAlreadyExist()
        {
            // Arrange
            var command = new CreateContactInfoCommand
            {
                HotelId = Guid.NewGuid(),
                Type = ContactType.Email,
                Value = "existing@example.com"
            };

            _contactInfoRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Contact, bool>>>()))
                        .ReturnsAsync(new Contact
                        {
                            Id = Guid.NewGuid(),
                            HotelId = command.HotelId,
                            Type = command.Type,
                            Value = command.Value
                        });

            var handler = new CreateContactInfoCommandHandler(_contactInfoRepository.Object, _mediator.Object);
            var result = await handler.Handle(command, new System.Threading.CancellationToken());

            result.Success.Should().BeFalse();
            result.Message.Should().Be(Messages.NameAlreadyExist);
        }

        [Test]
        public async Task ContactInfo_UpdateCommand_Success()
        {
            // Arrange
            var command = new UpdateContactInfoCommand
            {
                Id = Guid.NewGuid(),
                Value = "new-email@example.com",
                Type = ContactType.Email
            };

            _contactInfoRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Contact, bool>>>()))
                        .ReturnsAsync(new Contact
                        {
                            Id = command.Id,
                            HotelId = Guid.NewGuid(),
                            Type = command.Type,
                            Value = "old-email@example.com",
                            CreateDate = DateTime.Now
                        });

            _contactInfoRepository.Setup(x => x.Update(It.IsAny<Contact>())).Returns(new Contact());

            var handler = new UpdateContactInfoCommandHandler(_contactInfoRepository.Object, _mediator.Object);
            var result = await handler.Handle(command, new System.Threading.CancellationToken());

            _contactInfoRepository.Verify(x => x.SaveChangesAsync());
            result.Success.Should().BeTrue();
            result.Message.Should().Be(Messages.Updated);
        }

        [Test]
        public async Task ContactInfo_UpdateCommand_Failure()
        {
            // Arrange
            var command = new UpdateContactInfoCommand
            {
                Id = Guid.NewGuid(), // Non-existing contact
                Value = "new-email@example.com",
                Type = ContactType.Email
            };

            _contactInfoRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Contact, bool>>>()))
                        .ReturnsAsync((Contact)null); // No existing contact

            var handler = new UpdateContactInfoCommandHandler(_contactInfoRepository.Object, _mediator.Object);
            var result = await handler.Handle(command, new System.Threading.CancellationToken());

            result.Success.Should().BeFalse();
            result.Message.Should().Be(Messages.ContactNotFound);  
        }

        [Test]
        public async Task ContactInfo_DeleteCommand_Success()
        {
            // Arrange
            var command = new DeleteContactInfoCommand
            {
                Id = Guid.NewGuid()
            };

            _contactInfoRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Contact, bool>>>()))
                        .ReturnsAsync(new Contact
                        {
                            Id = command.Id,
                            HotelId = Guid.NewGuid(),
                            Type = ContactType.PhoneNumber,
                            Value = "123-456-7890",
                            CreateDate = DateTime.Now
                        });

            _contactInfoRepository.Setup(x => x.Delete(It.IsAny<Contact>()));

            var handler = new DeleteContactInfoCommandHandler(_contactInfoRepository.Object, _mediator.Object);
            var result = await handler.Handle(command, new System.Threading.CancellationToken());

            _contactInfoRepository.Verify(x => x.SaveChangesAsync());
            result.Success.Should().BeTrue();
            result.Message.Should().Be(Messages.Deleted);
        }

        [Test]
        public async Task ContactInfo_DeleteCommand_Failure()
        {
            var command = new DeleteContactInfoCommand
            {
                Id = Guid.NewGuid() 
            };

            _contactInfoRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Contact, bool>>>()))
                        .ReturnsAsync((Contact)null); 

            var handler = new DeleteContactInfoCommandHandler(_contactInfoRepository.Object, _mediator.Object);
            var result = await handler.Handle(command, new System.Threading.CancellationToken());

            result.Success.Should().BeFalse();
            result.Message.Should().Be(Messages.ContactNotFound); 
        }

        [Test]
        public async Task ContactInfo_CreateCommand_ValidationFailure()
        {
            // Arrange
            var command = new CreateContactInfoCommand
            {
                HotelId = Guid.NewGuid(),
                Type = (ContactType)999, // Invalid enum value
                Value = string.Empty // Invalid value
            };

            var handler = new CreateContactInfoCommandHandler(_contactInfoRepository.Object, _mediator.Object);
            var result = await handler.Handle(command, new System.Threading.CancellationToken());

            result.Success.Should().BeFalse();
            result.Message.Should().Be(Messages.ValidationFailed); 
        }
    }
}
