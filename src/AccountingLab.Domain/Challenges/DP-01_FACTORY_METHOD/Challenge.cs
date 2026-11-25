namespace AccountingLab.Domain.Challenges.DP01FactoryMethod;

public sealed record VoucherCreationRequest(
    Guid CompanyId,
    string Kind,
    decimal Amount,
    string Currency);

public sealed record VoucherDraft(
    Guid Id,
    Guid CompanyId,
    string Kind,
    decimal Amount,
    string Currency);

public interface IVoucherCreationChallenge
{
    VoucherDraft Create(VoucherCreationRequest request);
}

// Contrato de creación
public abstract class VoucherFactory
{
    public abstract VoucherDraft Create(VoucherCreationRequest request);
}

// Fábrica de ingreso
public sealed class IncomeVoucherFactory : VoucherFactory
{
    public override VoucherDraft Create(VoucherCreationRequest request)
    {
        return new VoucherDraft(
            Guid.NewGuid(), request.CompanyId, "ingreso",
            request.Amount, request.Currency);
    }
}

// Fábrica de egreso
public sealed class ExpenseVoucherFactory : VoucherFactory
{
    public override VoucherDraft Create(VoucherCreationRequest request)
    {
        return new VoucherDraft(
            Guid.NewGuid(), request.CompanyId, "egreso",
            request.Amount, request.Currency);
    }
}

// Fábrica de ajuste
public sealed class AdjustmentVoucherFactory : VoucherFactory
{
    public override VoucherDraft Create(VoucherCreationRequest request)
    {
        return new VoucherDraft(
            Guid.NewGuid(), request.CompanyId, "ajuste",
            request.Amount, request.Currency);
    }
}

// Fábrica de traslado
public sealed class TransferVoucherFactory : VoucherFactory
{
    public override VoucherDraft Create(VoucherCreationRequest request)
    {
        return new VoucherDraft(
            Guid.NewGuid(), request.CompanyId, "traslado",
            request.Amount, request.Currency);
    }
}

public sealed class VoucherCreationChallenge : IVoucherCreationChallenge
{
    public VoucherDraft Create(VoucherCreationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.CompanyId == Guid.Empty)
            throw new ArgumentException("La empresa es obligatoria.");

        if (string.IsNullOrWhiteSpace(request.Kind))
            throw new ArgumentException("El tipo de comprobante es obligatorio.");

        if (string.IsNullOrWhiteSpace(request.Currency))
            throw new ArgumentException("La moneda es obligatoria.");

        if (request.Amount < 0)
            throw new ArgumentException("El importe no puede ser negativo.");

        VoucherFactory factory = request.Kind.Trim().ToLowerInvariant() switch
        {
            "ingreso" => new IncomeVoucherFactory(),
            "egreso" => new ExpenseVoucherFactory(),
            "ajuste" => new AdjustmentVoucherFactory(),
            "traslado" => new TransferVoucherFactory(),
            _ => throw new ArgumentException("Tipo de comprobante no admitido.")
        };

        return factory.Create(request);
    }
}
