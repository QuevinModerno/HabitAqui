using Microsoft.AspNetCore.Identity;
using TPpweb.Models;

namespace TPpweb.Data
{
    public enum Roles
    {
        Administrador,
        Gestor,
        Cliente,
        Funcionario
    }
    public static class Inicializacao
    {
        public static async Task CriaDadosIniciais(UserManager<Person>
       userManager, RoleManager<IdentityRole> roleManager)
        {
            //Adicionar default Roles 
            await roleManager.CreateAsync(new IdentityRole(Roles.Administrador.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Gestor.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Cliente.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Funcionario.ToString()));
            //Adicionar Default User - Admin
            var defaultUser = new Person
            {
                UserName = "admin@admin.com",
                Email = "admin@admin.com",
                Nome = "Administrador",
                Morada="ISEC",
                Passe="Admin_123",
                DataNascimento= new DateTime(2010, 12, 12),
                BI ="59464658"

            };
            var user = await userManager.FindByEmailAsync(defaultUser.Email);
            if (user == null)
            {
                var sw = await userManager.CreateAsync(defaultUser, defaultUser.Passe);

                await userManager.AddToRoleAsync(defaultUser,
                 Roles.Administrador.ToString());
                
            }
        }
    }
}
