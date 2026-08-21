using Fluxi.Domain.Invoices.Entities;
using Fluxi.Domain.Invoices.Enums;
using Fluxi.Domain.Invoices.Exceptions;

namespace Fluxi.Domain.Tests.Invoices;

[Trait(TestTraits.Category, TestTraits.UnitCategory)]
[Trait(TestTraits.Layer, TestTraits.DomainLayer)]
[Trait(TestTraits.Feature, TestTraits.InvoicesFeature)]
public sealed class InvoiceTests
{
    #region Tests

    [Fact]
    public void Create_WithValidData_ShouldCreateInvoice()
    {
        // Arrange
        Guid accountId = Guid.NewGuid();
        const string reference = "2026-08";
        DateOnly closingDate = new(2026, 8, 10);
        DateOnly dueDate = new(2026, 8, 20);

        // Act
        Invoice invoice = Invoice.Create(accountId, reference, closingDate, dueDate);

        // Assert
        Assert.NotEqual(Guid.Empty, invoice.Id);
        Assert.Equal(accountId, invoice.AccountId);
        Assert.Equal(reference, invoice.Reference);
        Assert.Equal(closingDate, invoice.ClosingDate);
        Assert.Equal(dueDate, invoice.DueDate);
        Assert.Equal(InvoiceStatus.Open, invoice.Status);
        Assert.Null(invoice.TotalAmount);
    }


    [Fact]
    public void Create_ShouldInitializeRelatedTransactions()
    {
        // Arrange

        // Act
        Invoice invoice = CreateInvoice();

        // Assert
        Assert.Null(invoice.Account);
        Assert.Empty(invoice.Transactions);
    }

    [Fact]
    public void Create_WithEmptyAccountId_ShouldThrow()
    {
        // Arrange
        Action action = () => Invoice.Create(
            Guid.Empty,
            "2026-08",
            new DateOnly(2026, 8, 10),
            new DateOnly(2026, 8, 20));

        // Act & Assert
        Assert.Throws<InvoiceValidationException>(action);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WithInvalidReference_ShouldThrow(string? reference)
    {
        // Arrange
        Action action = () => Invoice.Create(
            Guid.NewGuid(),
            reference!,
            new DateOnly(2026, 8, 10),
            new DateOnly(2026, 8, 20));

        // Act & Assert
        Assert.Throws<InvoiceValidationException>(action);
    }

    [Fact]
    public void Create_WithDefaultClosingDate_ShouldThrow()
    {
        // Arrange
        Action action = () => Invoice.Create(
            Guid.NewGuid(),
            "2026-08",
            default,
            new DateOnly(2026, 8, 20));

        // Act & Assert
        Assert.Throws<InvoiceValidationException>(action);
    }

    [Fact]
    public void Create_WithDefaultDueDate_ShouldThrow()
    {
        // Arrange
        Action action = () => Invoice.Create(
            Guid.NewGuid(),
            "2026-08",
            new DateOnly(2026, 8, 10),
            default);

        // Act & Assert
        Assert.Throws<InvoiceValidationException>(action);
    }

    [Fact]
    public void Create_WithDueDateBeforeClosingDate_ShouldThrow()
    {
        // Arrange
        Action action = () => Invoice.Create(
            Guid.NewGuid(),
            "2026-08",
            new DateOnly(2026, 8, 20),
            new DateOnly(2026, 8, 10));

        // Act & Assert
        Assert.Throws<InvoiceValidationException>(action);
    }

    [Fact]
    public void Close_FromOpen_ShouldSetStatusToClosed()
    {
        // Arrange
        Invoice invoice = CreateInvoice();

        // Act
        invoice.Close();

        // Assert
        Assert.Equal(InvoiceStatus.Closed, invoice.Status);
    }

    [Fact]
    public void Close_FromClosed_ShouldThrow()
    {
        // Arrange
        Invoice invoice = CreateInvoice();
        invoice.Close();

        // Act
        Action action = invoice.Close;

        // Assert
        Assert.Throws<InvoiceValidationException>(action);
    }

    [Fact]
    public void Close_FromPaid_ShouldThrow()
    {
        // Arrange
        Invoice invoice = CreateInvoice();
        invoice.Close();
        invoice.Pay();

        // Act
        Action action = invoice.Close;

        // Assert
        Assert.Throws<InvoiceValidationException>(action);
    }

    [Fact]
    public void Pay_FromClosed_ShouldSetStatusToPaid()
    {
        // Arrange
        Invoice invoice = CreateInvoice();
        invoice.Close();

        // Act
        invoice.Pay();

        // Assert
        Assert.Equal(InvoiceStatus.Paid, invoice.Status);
    }

    [Fact]
    public void Pay_FromOpen_ShouldThrow()
    {
        // Arrange
        Invoice invoice = CreateInvoice();

        // Act
        Action action = invoice.Pay;

        // Assert
        Assert.Throws<InvoiceValidationException>(action);
    }

    [Fact]
    public void Pay_FromPaid_ShouldThrow()
    {
        // Arrange
        Invoice invoice = CreateInvoice();
        invoice.Close();
        invoice.Pay();

        // Act
        Action action = invoice.Pay;

        // Assert
        Assert.Throws<InvoiceValidationException>(action);
    }

    [Fact]
    public void UpdateTotalAmount_WithValidValue_ShouldUpdate()
    {
        // Arrange
        Invoice invoice = CreateInvoice();

        // Act
        invoice.UpdateTotalAmount(1250.75m);

        // Assert
        Assert.Equal(1250.75m, invoice.TotalAmount);
    }

    [Fact]
    public void UpdateTotalAmount_WithNegativeValue_ShouldThrow()
    {
        // Arrange
        Invoice invoice = CreateInvoice();

        // Act
        Action action = () => invoice.UpdateTotalAmount(-1m);

        // Assert
        Assert.Throws<InvoiceValidationException>(action);
    }

    #endregion

    #region Test Helpers

    private static Invoice CreateInvoice()
    {
        return Invoice.Create(
            Guid.NewGuid(),
            "2026-08",
            new DateOnly(2026, 8, 10),
            new DateOnly(2026, 8, 20));
    }

    #endregion
}
