
using AccountingLab.Domain.Education;

namespace AccountingLab.Domain.Challenges.DP05Singleton;

// ======================================================
// 1. MODELO DEL REGISTRO CONTABLE
// ======================================================

public sealed record GlobalAccountingRegistry(
    string Version,
    int MonetaryScale,
    string Scope);

// ======================================================
// 2. CONTRATO DEL DESAFÍO
// ======================================================

public interface IGlobalRegistryChallenge
{
    GlobalAccountingRegistry Acquire();
}

// ======================================================
// 3. IMPLEMENTACIÓN SINGLETON
// ======================================================

public sealed class GlobalRegistryChallenge : IGlobalRegistryChallenge
{
    // Lazy garantiza una única instancia y su inicialización segura
    // cuando varias tareas acceden simultáneamente.

    private static readonly Lazy<GlobalAccountingRegistry> _instance =
        new Lazy<GlobalAccountingRegistry>(
            () => new GlobalAccountingRegistry(
                Version: "DEMO-1",
                MonetaryScale: 4,
                Scope: "process"),
            LazyThreadSafetyMode.ExecutionAndPublication);

    public GlobalAccountingRegistry Acquire()
    {
        return _instance.Value;
    }
}
