using Microsoft.AspNetCore.Identity;

namespace MinimalApiPeliculas.Servicios
{
    public class ServicioUsuarios : IServicioUsuarios
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;

        public ServicioUsuarios(IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<IdentityUser?> ObtenerUsuario()
        {
            var emailClaim = _httpContextAccessor.HttpContext!
                .User.Claims.Where(x => x.Type == "email").FirstOrDefault();

            if (emailClaim is null) { return null; }

            var email = emailClaim.Value;
            return await _userManager.FindByEmailAsync(email);
        }
    }
}
