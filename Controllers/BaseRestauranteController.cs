using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

public abstract class BaseRestauranteController : ControllerBase
{
    protected int? GetRestauranteIdDesdeToken()
    {
        var idClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

        if (idClaim != null && int.TryParse(idClaim.Value, out int restauranteId))
        {
            return restauranteId;
        }

        return null;
    }
}
