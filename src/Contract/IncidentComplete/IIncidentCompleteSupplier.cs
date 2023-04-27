using System;
using System.Threading;
using System.Threading.Tasks;

namespace GGroupp.Internal.Support;

public interface IIncidentCompleteSupplier
{
    ValueTask<Result<IncidentCompleteOut, Failure<Unit>>> CompleteIncidentAsync(IncidentCompleteIn input, CancellationToken cancellationToken);
}