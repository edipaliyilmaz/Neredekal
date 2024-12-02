using Business.Handlers.Reports.Queries;
using DataAccess.Abstract;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static Business.Handlers.Reports.Queries.GetReportQuery;
using Entities.Concrete;
using static Business.Handlers.Reports.Queries.GetReportsQuery;
using static Business.Handlers.Reports.Commands.CreateReportCommand;
using Business.Handlers.Reports.Commands;
using Business.Constants;
using static Business.Handlers.Reports.Commands.UpdateReportCommand;
using static Business.Handlers.Reports.Commands.DeleteReportCommand;
using MediatR;
using FluentAssertions;
using Core.Enums;
using System.Linq;

namespace Tests.Business.HandlersTest
{
    [TestFixture]
    public class ReportHandlerTests
    {
        Mock<IReportRepository> _reportRepository;
        Mock<IMediator> _mediator;

        [SetUp]
        public void Setup()
        {
            _reportRepository = new Mock<IReportRepository>();
            _mediator = new Mock<IMediator>();
        }

        [Test]
        public async Task Report_GetQuery_Success()
        {
            // Arrange
            var query = new GetReportQuery();

            _reportRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Report, bool>>>())).ReturnsAsync(new Report
            {
                Id = Guid.NewGuid(),
                Status = ReportStatus.Preparing, // Örnek: Preparing durumu
                CreateDate = DateTime.Now
            });

            var handler = new GetReportQueryHandler(_reportRepository.Object, _mediator.Object);

            // Act
            var x = await handler.Handle(query, new System.Threading.CancellationToken());

            // Assert
            x.Success.Should().BeTrue();
            x.Data.Status.Should().Be(ReportStatus.Preparing); // Değiştirildi: ReportStatus.Preparing
        }

        [Test]
        public async Task Report_GetQueries_Success()
        {
            // Arrange
            var query = new GetReportsQuery();

            _reportRepository.Setup(x => x.GetListAsync(It.IsAny<Expression<Func<Report, bool>>>()))
                        .ReturnsAsync(new List<Report>
                        {
                            new Report { Id = Guid.NewGuid(), Status = ReportStatus.Preparing, CreateDate = DateTime.Now },
                            new Report { Id = Guid.NewGuid(), Status = ReportStatus.Completed, CreateDate = DateTime.Now }
                        });

            var handler = new GetReportsQueryHandler(_reportRepository.Object, _mediator.Object);

            // Act
            var x = await handler.Handle(query, new System.Threading.CancellationToken());

            // Assert
            x.Success.Should().BeTrue();
            ((List<Report>)x.Data).Count.Should().BeGreaterThan(1);
        }

        [Test]
        public async Task Report_CreateCommand_Success()
        {
            Report rt = null;
            // Arrange
            var command = new CreateReportCommand();

            _reportRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Report, bool>>>()))
                        .ReturnsAsync(rt);

            _reportRepository.Setup(x => x.Add(It.IsAny<Report>())).Returns(new Report
            {
                Id = Guid.NewGuid(),
                Status = ReportStatus.Preparing, // Başlangıç durumu olarak Preparing
                CreateDate = DateTime.Now
            });

            var handler = new CreateReportCommandHandler(_reportRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            _reportRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Added);
        }

        [Test]
        public async Task Report_CreateCommand_NameAlreadyExist()
        {
            // Arrange
            var command = new CreateReportCommand();

            _reportRepository.Setup(x => x.Query())
                                           .Returns(new List<Report>
                                           {
                                               new Report { Id = Guid.NewGuid(), Status = ReportStatus.Preparing, CreateDate = DateTime.Now }
                                           }.AsQueryable());

            _reportRepository.Setup(x => x.Add(It.IsAny<Report>())).Returns(new Report());

            var handler = new CreateReportCommandHandler(_reportRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            x.Success.Should().BeFalse();
            x.Message.Should().Be(Messages.NameAlreadyExist);
        }

        [Test]
        public async Task Report_UpdateCommand_Success()
        {
            // Arrange
            var command = new UpdateReportCommand();

            _reportRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Report, bool>>>()))
                        .ReturnsAsync(new Report
                        {
                            Id = Guid.NewGuid(),
                            Status = ReportStatus.Preparing, // Önceki durum
                            CreateDate = DateTime.Now
                        });

            _reportRepository.Setup(x => x.Update(It.IsAny<Report>())).Returns(new Report
            {
                Id = Guid.NewGuid(),
                Status = ReportStatus.Completed, // Güncellenmiş durum
                CreateDate = DateTime.Now
            });

            var handler = new UpdateReportCommandHandler(_reportRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            _reportRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Updated);
        }

        [Test]
        public async Task Report_DeleteCommand_Success()
        {
            // Arrange
            var command = new DeleteReportCommand();

            _reportRepository.Setup(x => x.GetAsync(It.IsAny<Expression<Func<Report, bool>>>()))
                        .ReturnsAsync(new Report
                        {
                            Id = Guid.NewGuid(),
                            Status = ReportStatus.Completed, // Durum Completed
                            CreateDate = DateTime.Now
                        });

            _reportRepository.Setup(x => x.Delete(It.IsAny<Report>()));

            var handler = new DeleteReportCommandHandler(_reportRepository.Object, _mediator.Object);
            var x = await handler.Handle(command, new System.Threading.CancellationToken());

            _reportRepository.Verify(x => x.SaveChangesAsync());
            x.Success.Should().BeTrue();
            x.Message.Should().Be(Messages.Deleted);
        }
    }
}
